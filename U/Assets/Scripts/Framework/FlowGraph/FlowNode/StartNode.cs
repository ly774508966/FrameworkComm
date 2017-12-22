using System.Collections;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class StartNode : FlowNode
    {
        public override string NodeName
        {
            get { return "StartNode"; }
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
            FinishExecute();
        }
    }
}