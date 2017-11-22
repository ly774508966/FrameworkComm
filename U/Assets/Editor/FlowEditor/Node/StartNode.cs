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

        public string sDescription;

        public override void OnDrawProperty()
        {
            base.OnDrawProperty();

            sDescription = EditorGUILayout.TextField("Description", sDescription);
        }
    }
}