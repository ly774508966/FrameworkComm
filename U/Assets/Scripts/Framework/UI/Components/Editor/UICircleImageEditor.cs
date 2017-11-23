using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Framework.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(UICircleImage), true)]
    public class UICircleImageEditor : ImageEditor
    {
        SerializedProperty _fillPercent;
        SerializedProperty _fill;
        SerializedProperty _thickness;
        SerializedProperty _segements;

        protected override void OnEnable()
        {
            base.OnEnable();
            _fillPercent = serializedObject.FindProperty("_fillPercent");
            _fill = serializedObject.FindProperty("_fill");
            _thickness = serializedObject.FindProperty("_thickness");
            _segements = serializedObject.FindProperty("_segements");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(_fillPercent, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(_fill, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(_thickness, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(_segements, new GUILayoutOption[0]);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
