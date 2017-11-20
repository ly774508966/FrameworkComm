using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class StartNode : FlowNode
    {
        public override string Name
        {
            get { return "StartNode"; }
        }

        public override float NodeWidth
        {
            get { return 150f; }
        }

        public string sDescription { get; set; }

        public override void DrawProperty()
        {
            base.DrawProperty();

            sDescription = EditorGUILayout.TextField("Description", sDescription);
        }
    }
}