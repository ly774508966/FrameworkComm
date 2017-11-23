using System.Collections;
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

        public override float NodeHeight
        {
            get { return 50f; }
        }

        public override void OnDrawProperty()
        {
            base.OnDrawProperty();
        }

        public override void OnDrawNode()
        {
            base.OnDrawNode();
        }

        public override IEnumerator OnExecute()
        {
            yield return base.OnExecute();
            Log.Debug(string.Format("{0} execute finish, delay {1}s", NodeName, delay));
            FinishExecute();
        }
    }
}