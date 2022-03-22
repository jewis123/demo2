using GraphProcessor;

namespace FrameGraph
{
    public static class BaseNodeExtensions
    {
        public static string GetValidName(this BaseNode node)
        {
            return string.IsNullOrEmpty(node.GetCustomName()) ? node.name : node.GetCustomName();
        }
    }
}