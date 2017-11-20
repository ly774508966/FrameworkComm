using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class EndNode : FlowNode
    {
        public override string Name
        {
            get { return "EndNode"; }
        }

        public override float NodeWidth
        {
            get { return 120f; }
        }

        public string sDescription { get; set; }

        public override void DrawProperty()
        {
            base.DrawProperty();

            sDescription = EditorGUILayout.TextField("Description", sDescription);
        }
    }
}