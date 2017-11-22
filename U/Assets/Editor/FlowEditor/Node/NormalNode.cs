using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
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

        public string sDescription;

        public override void OnDrawProperty()
        {
            base.OnDrawProperty();

            sDescription = EditorGUILayout.TextField("Description", sDescription);
        }
    }
}