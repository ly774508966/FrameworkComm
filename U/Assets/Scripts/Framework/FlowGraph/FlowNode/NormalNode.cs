using UnityEngine;
using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class NormalNode : FlowNode
    {
        public override string NodeName
        {
            get { return "NormalNode"; }
        }

        public override float NodeWidth
        {
            get { return 180f; }
        }

        public override void OnDrawProperty(FlowGraph graph)
        {
            base.OnDrawProperty(graph);
        }
    }
}