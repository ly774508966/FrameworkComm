using UnityEngine;
using UnityEditor;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    [CustomEditor(typeof(FlowGraph))]
    public class FlowGraphInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Edit"))
            {
                FlowEditorWindow window = FlowEditorWindow.Open();
                window.CreateGraph(target);
            }
        }
    }
}