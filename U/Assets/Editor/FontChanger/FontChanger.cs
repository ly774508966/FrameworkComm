using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// 字体批量修改工具
/// @zhenhaiwang
/// </summary>
namespace Framework.Editor.Editor
{
    public class FontChanger : EditorWindow
    {
        [MenuItem("Window/Font Changer", priority = 3)]
        private static void ShowWindow()
        {
            FontChanger window = GetWindow<FontChanger>(true, "Window/Font Changer");
            window.minSize = new Vector2(150f, 100f);
            window.Show();
            window.Focus();
        }

        Font _defaultFont;
        Font _targetFont;

        private void OnEnable()
        {
            _defaultFont = new Font("Arial");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            GUILayout.Label("Target Font:");
            _targetFont = _defaultFont = (Font)EditorGUILayout.ObjectField(_defaultFont, typeof(Font), true, GUILayout.MinWidth(100f));

            if (GUILayout.Button("OK"))
            {
                ChangeFont();
            }
        }

        void ChangeFont()
        {
            if (Selection.objects == null || Selection.objects.Length == 0)
                return;

            Object[] labels = Selection.GetFiltered(typeof(Text), SelectionMode.Deep);
            foreach (Object item in labels)
            {
                Text label = (Text)item;
                label.font = _targetFont;
                EditorUtility.SetDirty(item);
            }
        }
    }
}