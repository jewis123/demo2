

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrameGraph.Example
{
    public class TestDebug
    {
        public static void AssertList(List<int> list1, List<int> list2)
        {
            Assert.IsTrue(list1.Count == list2.Count);
            for (int i = 0, cnt = list1.Count; i < cnt; i++)
            {
                Assert.IsTrue(list1[i] == list2[i]);
            }
        }

        public static void PrintSortedPasses(FrameGraphCompiler.Result crst, FrameGraph fg)
        {
            Debug.Log("================== [TopoSorted Passes] ==================");
            string str = string.Empty;
            for (int i = 0, cnt = crst.sortedPasses.Count; i < cnt; i++)
            {
                var passId = crst.sortedPasses[i];
                str += string.Format(" -> {0}", fg.GetPassNode(passId).name);
            }

            Debug.Log(str);
        }
    }
}