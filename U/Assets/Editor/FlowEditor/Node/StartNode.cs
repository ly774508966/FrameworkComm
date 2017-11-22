using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class StartNode : FlowNode
    {
        public override string NodeName
        {
            get { return "StartNode"; }
        }

        public override float NodeWidth
        {
            get { return 150f; }
        }

        public override void OnDrawProperty(FlowGraph graph)
        {
            base.OnDrawProperty(graph);
        }
    }
}