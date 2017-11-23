using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class EndNode : FlowNode
    {
        public override string NodeName
        {
            get { return "EndNode"; }
        }

        public override float NodeWidth
        {
            get { return 120f; }
        }

        public override void OnDrawProperty(FlowGraph graph)
        {
            base.OnDrawProperty(graph);
        }
    }
}