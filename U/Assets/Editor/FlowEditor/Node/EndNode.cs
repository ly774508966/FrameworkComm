using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
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

        public string sDescription;

        public override void OnDrawProperty()
        {
            base.OnDrawProperty();

            sDescription = EditorGUILayout.TextField("Description", sDescription);
        }
    }
}