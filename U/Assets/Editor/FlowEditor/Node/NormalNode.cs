using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class NormalNode : FlowNode
    {
        public override string Name
        {
            get { return "NormalNode"; }
        }

        public override float NodeWidth
        {
            get { return 180f; }
        }

        public string sDescription { get; set; }

        public override void DrawProperty()
        {
            base.DrawProperty();

            sDescription = EditorGUILayout.TextField("Description", sDescription);
        }
    }
}