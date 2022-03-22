using System.Collections.Generic;

namespace FrameGraph
{
    /// <summary>
    /// 操作FrameGraph资产文件的句柄
    /// </summary>
    public interface IFrameGraphAssetHandle
    {
        /// <summary>
        /// 获取按照执行顺序排序后的Pass列表
        /// </summary>
        /// <returns></returns>
        List<int> GetSortedPasses();

        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <returns></returns>
        List<int> GetRenderTargets();


        /// <summary>
        /// 获取指定渲染目标的描述
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RenderTargetInfo GetRenderTargetInfo(int id);

        /// <summary>
        /// 获取Pass的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PassInfo GetPassInfo(int id);

        /// <summary>
        /// 获取指定Pass的ScriptableRenderPass
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        IFrameGraphRenderPass CreateFrameGraphRenderPass(int id, URPContext context, PassInfo pi);

        /// <summary>
        /// 获取RT与最后一次被使用的Pass的映射
        /// </summary>
        /// <returns></returns>
        Dictionary<int, int> GetRenderTarget2LastPass();
    }
}