using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrameGraph
{
    public class FrameGraphCompiler
    {
        public class Result
        {
            // 输入的资源不应该是未写过数据的资源
            public const int ERROR_INVALID_INPUT_RESOURCE = 1;

            // 多个PASS同时对一个资源进行写入
            public const int ERROR_MULIT_PASS_WRITE_RESOURCE = ERROR_INVALID_INPUT_RESOURCE + 1;

            // RenderGraph是有向图环路
            public const int ERROR_RENDERGRAPH_RING = ERROR_MULIT_PASS_WRITE_RESOURCE + 1;

            // 资源的生命周期与拓扑序列不符
            public const int ERROR_RESOURCE_LIFE_TIME = ERROR_RENDERGRAPH_RING + 1;

            public class PassGraph
            {
                public Dictionary<int, HashSet<int>> adjList = new Dictionary<int, HashSet<int>>();

                /// <summary>
                /// 对邻接表进行拓扑排序。成功生成拓扑序列返回true，存在循环图返回false
                /// </summary>
                /// <param name="sortedList"></param>
                /// <returns></returns>
                public bool TopoSort(out List<int> sortedList)
                {
                    sortedList = new List<int>();
                    Dictionary<int, int> inDegreeMap = new Dictionary<int, int>();

                    // 统计Pass深度
                    foreach (var pass in adjList.Keys)
                    {
                        inDegreeMap.Add(pass, 0);
                    }

                    foreach (var sets in adjList.Values)
                    {
                        foreach (var pass in sets)
                        {
                            inDegreeMap[pass]++;
                        }
                    }

                    // 先找到深度为0的Pass（表示是入口）
                    Stack<int> stack = new Stack<int>();
                    foreach (var kvp in inDegreeMap)
                    {
                        if (kvp.Value == 0)
                        {
                            stack.Push(kvp.Key);
                        }
                    }

                    // 循环移除深度为0的Pass
                    while (stack.Count > 0)
                    {
                        int passId = stack.Pop();
                        sortedList.Add(passId);
                        foreach (var childPassId in adjList[passId])
                        {
                            if (--inDegreeMap[childPassId] == 0)
                            {
                                stack.Push(childPassId);
                            }
                        }
                    }

                    return sortedList.Count == adjList.Count;
                }
            }

            public int error = 0;

            public PassGraph passGraph = new PassGraph();

            public ResInfo[] res2Info;
            public List<int> sortedPasses = new List<int>();
        }

        /// <summary>
        /// 资源信息类
        /// </summary>
        [Serializable]
        public class ResInfo
        {
            /// <summary>
            /// 第一个写入该资源的Pass
            /// </summary>
            public int firstPass = PassNode.NO_PASS;
            
            /// <summary>
            /// 最后一个使用该资源的Pass
            /// </summary>
            public int lastPass = PassNode.NO_PASS;
            
            /// <summary>
            /// 所有只读该资源的Pass集合
            /// </summary>
            public List<int> readers = new List<int>();
            
            /// <summary>
            /// 所有读写该资源的Pass集合
            /// </summary>
            public List<int> writers = new List<int>();
            
            /// <summary>
            /// 使用该资源的Pass的执行顺序
            /// </summary>
            public List<int> trimmedSortedList = new List<int>();
        }

        /// <summary>
        /// 编辑有向图，获得Pass的拓扑序列
        /// </summary>
        /// <param name="fg"></param>
        /// <returns></returns>
        public Result Compile(FrameGraph fg)
        {
            Result rst = new Result();

            // 创建Pass的邻接表，配置每个资源的读写Pass
            rst.res2Info = new ResInfo[fg.resourceCount];
            var res2Info = rst.res2Info;    // 资源信息数组，下标为资源序号
            var passNodesIter = fg.GetPassNodes();
            // 扫描所有Pass
            while (passNodesIter.MoveNext())
            {
                var passNode = passNodesIter.Current;

                // 创建Pass的邻接表，以便之后填充数据
                rst.passGraph.adjList.Add(passNode.id, new HashSet<int>());

                // 扫描Pass的只读资源列表，将Pass写入每个资源的只读集合内
                for (int i = 0, cnt = passNode.inputReses.Length; i < cnt; i++)
                {
                    var res = passNode.inputReses[i];
                    if (res != ResourceNode.NO_RESOURCE)
                    {
                        ResInfo resInfo = res2Info[res];
                        if (resInfo == null)
                        {
                            resInfo = new ResInfo();
                            res2Info[res] = resInfo;
                        }

                        resInfo.readers.Add(passNode.id);

                        // 检测这个Pass读取的资源来自于哪个前驱Pass，如果不存在前驱Pass则进行提示并停止编译（只读的资源必须事先被写入数据，否则无意义）
                        if (passNode.inputResRefPasses[i] == PassNode.NO_PASS)
                        {
                            Debug.LogError(string.Format("输入的资源不应该是未写过数据的资源[Pass:{0}, Resource:{1}]", passNode.name, fg.GetResourceNode(res).name));
                            rst.error = Result.ERROR_INVALID_INPUT_RESOURCE;
                            return rst;
                        }
                    }
                }

                // 扫描Pass的读写资源列表，将Pass写入每个资源的读写集合内
                for (int i = 0, cnt = passNode.outputReses.Length; i < cnt; i++)
                {
                    var res = passNode.outputReses[i];
                    if (res != ResourceNode.NO_RESOURCE)
                    {
                        ResInfo resInfo = res2Info[res];
                        if (resInfo == null)
                        {
                            resInfo = new ResInfo();
                            res2Info[res] = resInfo;
                        }

                        resInfo.writers.Add(passNode.id);

                        // 检测这个Pass读写的资源来自于哪个前驱Pass，如果没有前驱Pass则表示该Pass是第一个写入该资源
                        if (passNode.outputResRefPasses[i] == PassNode.NO_PASS)
                        {
                            // 如遇到多个Pass都声称自己是第一个写入资源的Pass，则编译停止提示错误（因为无法评估执行顺序）
                            if (resInfo.firstPass == PassNode.NO_PASS)
                            {
                                resInfo.firstPass = passNode.id;
                            }
                            else
                            {
                                Debug.LogError(string.Format("资源存在多个初始写入Pass[Resource:{0}]", fg.GetResourceNode(res).name));
                                rst.error = Result.ERROR_MULIT_PASS_WRITE_RESOURCE;
                                return rst;
                            }
                        }
                    }
                }

            }

            // 为每个Pass填充邻接表的数据
            passNodesIter = fg.GetPassNodes();
            // 扫描所有Pass
            while (passNodesIter.MoveNext())
            {
                var passNode = passNodesIter.Current;

                // 扫描Pass的前驱Pass，将该Pass标记为前驱Pass的后继
                foreach (var pass in passNode.inputResRefPasses)
                {
                    if (pass != PassNode.NO_PASS)
                        rst.passGraph.adjList[pass].Add(passNode.id);
                }
                foreach (var pass in passNode.outputResRefPasses)
                {
                    if (pass != PassNode.NO_PASS)
                        rst.passGraph.adjList[pass].Add(passNode.id);
                }
            }

            // 生成Pass的拓扑序列
            if (!rst.passGraph.TopoSort(out rst.sortedPasses))
            {
                Debug.LogError("RenderGraph是有向图环路");
                rst.error = Result.ERROR_RENDERGRAPH_RING;
                return rst;
            }

            // 根据每个资源的只读Pass和读写Pass，将这些Pass排序为每个资源生成使用它的Pass顺序序列。即获得每个Pass的生命周期
            var res2SortedList = new Dictionary<int, List<int>>();  // 资源和使用它的Pass映射，Pass列表的顺序为执行顺序
            var resReaderSets = new HashSet<int>();                 // 保存资源只读它的Pass，用于逻辑的临时集合
            var resWriterSets = new HashSet<int>();                 // 保存资源读写它的Pass，用于逻辑的临时集合
            for (int i = 0; i < res2Info.Length; i++)
            {
                var res = i;
                var info = res2Info[res];
                if (info == null)
                    continue;

                res2SortedList.Add(res, new List<int>());

                foreach (var reader in info.readers)
                {
                    resReaderSets.Add(reader);
                }
                foreach (var writer in info.writers)
                {
                    resWriterSets.Add(writer);
                }
                
                Stack<int> stack = new Stack<int>();
                stack.Push(info.firstPass);

                while (stack.Count > 0 /*|| resWriterSets.Count > 0 || resReaderSets.Count > 0*/)
                {
                    
                    // if (stack.Count == 0)
                    // {
                    //     // 这部分为了处理由于读写或只读Pass之间不是紧密相连导致没有被记录   
                    //     int frontPassIndex = 999; 
                    //     
                    //     foreach (var writePass in resWriterSets)
                    //     {
                    //         // 找到拓扑序列排在最前面的Pass
                    //         int currPassIndex = rst.sortedPasses.IndexOf(writePass);
                    //         if (currPassIndex < frontPassIndex)
                    //         {
                    //             frontPassIndex = currPassIndex;
                    //         }
                    //     }
                    //     foreach (var readPass in resReaderSets)
                    //     {
                    //         // 找到拓扑序列排在最前面的Pass
                    //         int currPassIndex = rst.sortedPasses.IndexOf(readPass);
                    //         if (currPassIndex < frontPassIndex)
                    //         {
                    //             frontPassIndex = currPassIndex;
                    //         }
                    //     }
                    //     stack.Push(rst.sortedPasses[frontPassIndex]);
                    // }
                    
                    int pass = stack.Pop();
                    if (!res2SortedList[res].Contains(pass))
                    {
                        res2SortedList[res].Add(pass);  // 栈顶的Pass即为先执行的Pass
                        resReaderSets.Remove(pass);
                        resWriterSets.Remove(pass);
                        
                        // 如果当前Pass后继里又有只读Pass，又有读写Pass。优先插入只读再插入读写
                        // 扫描读写集合，找到当前Pass的后继插入到堆栈等待后续逻辑处理
                        bool findWrite = false;
                        foreach (var writer in resWriterSets)
                        {
                            // 找到的资源在这个Writer中的序号
                            int index = 0;
                            var writerNode = fg.GetPassNode(writer);
                            while (writerNode.outputReses[index] != res)
                            {
                                index++;
                            }

                            // 检测该序号的前驱是当前的Pass
                            // 这里必定会找到至少一个读写Pass的前驱是当前Pass，如果没找到说明存在同时多个多写，会被之前的逻辑过滤掉
                            if (writerNode.outputResRefPasses[index] == pass)
                            {
                                // 不能存在多个后继同时对一个资源进行写入，无法区分顺序
                                if (findWrite)
                                {
                                    Debug.LogError(string.Format("不能存在多个后继同时对一个资源进行写入，无法区分顺序[Resource:{0}]", fg.GetResourceNode(res).name));
                                    rst.error = Result.ERROR_MULIT_PASS_WRITE_RESOURCE;
                                    return rst;
                                }

                                stack.Push(writer);
                                findWrite = true;
                            }
                        }

                        
                        // 扫描只读集合，找到当前Pass的后继插入到堆栈等待后续逻辑处理
                        foreach (var reader in resReaderSets)
                        {
                            // 找到的资源在这个Reader中的序号
                            int index = 0;
                            var readerNode = fg.GetPassNode(reader);
                            while (readerNode.inputReses[index] != res)
                            {
                                index++;
                            }

                            // 检测该序号的前驱是当前的Pass
                            if (readerNode.inputResRefPasses[index] == pass)
                            {
                                stack.Push(reader);
                            }
                        }
                    }
                }
            }

            // 使用每个资源的生命周期验证Pass的拓扑序列是否正确
            foreach (var kvp in res2SortedList)
            {
                var res = kvp.Key;
                var resSortedList = kvp.Value;

                // 扫描拓扑序列，将当前资源生命周期内存在的Pass加入到临时列表里
                foreach (var pass in rst.sortedPasses)
                {
                    if (resSortedList.Contains(pass))
                    {
                        res2Info[res].trimmedSortedList.Add(pass);
                        res2Info[res].lastPass = pass;
                    }
                }

                // 如果使用资源的Pass是只读，则顺序可以不一致。若为读写，则必须保持一致
                var p = 0;
                while (p < resSortedList.Count)
                {
                    // 当顺序不一样且非只读时，验证失败
                    var s1 = resSortedList[p];
                    var s2 = res2Info[res].trimmedSortedList[p];
                    if (s1 != s2 && (!res2Info[res].readers.Contains(s1) || !res2Info[res].readers.Contains(s2)))
                    {
                        Debug.LogError(string.Format("资源的生命周期与拓扑序列不符[Resource:{0}]", fg.GetResourceNode(res).name));
                        rst.error = Result.ERROR_RESOURCE_LIFE_TIME;
                        return rst;
                    }

                    p++;
                }
            }


            return rst;
        }
    }
}