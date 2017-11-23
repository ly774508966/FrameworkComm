using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.Editor
{
    public class CreateScriptFromTemplate : EditorWindow
    {
        private class TemplateInfo
        {
            public string menuName;
            public Dictionary<string, string> replacements;
            public Dictionary<string, string> specialKeys;
            public string wholeTemplate;
            public int priority;
        }

        private Dictionary<string, TemplateInfo> _templates;
        private string[] _menuItems;
        private string _createPath;
        private int _selected = 0;

        [MenuItem("Assets/Create/C# Script From Template", false, 50)]
        public static void CreateEditorScript()
        {
            CreateScriptFromTemplate window = GetWindow<CreateScriptFromTemplate>();
            window.wantsMouseMove = true;
            window.titleContent = new GUIContent("C# Script");
            window.minSize = new Vector2(360f, 150f);
            window.Show();
            window.Focus();
            window._createPath = window.GetProjectBrowserPath();
            window.ReadTemplates();
        }

        private string GetProjectBrowserPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            return path;
        }

        void OnGUI()
        {
            if (_templates == null || _menuItems == null)
            {
                ReadTemplates();
            }

            TemplateInfo info = _templates[_menuItems[_selected]];

            EditorGUILayout.BeginVertical();
            _selected = EditorGUILayout.Popup("Template", _selected, _menuItems, GUILayout.Width(350));

            List<string> keys = new List<string>(info.replacements.Keys);
            foreach (string key in keys)
            {
                if (key == "Year")
                {
                    continue;
                }

                info.replacements[key] = EditorGUILayout.TextField(key, info.replacements[key], GUILayout.Width(350));
            }

            GUILayout.Label("Creating file " + _createPath + "/" + info.replacements["ClassName"] + info.specialKeys["EXTENSION"]);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(info.replacements["ClassName"]));
            if (GUILayout.Button("Create"))
            {
                CreateScript(info);
                Close();
                _templates = null;
                _menuItems = null;
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void ReadTemplates()
        {
            List<string> paths = new List<string>();

            string[] dirs = Directory.GetDirectories(Application.dataPath, "ScriptTemplates", SearchOption.AllDirectories);

            foreach (string dir in dirs)
            {
                paths.AddRange(Directory.GetFiles(dir, "*.st"));
            }

            _templates = new Dictionary<string, TemplateInfo>();

            foreach (string template in paths)
            {
                TemplateInfo info = ParseTemplate(template);

                if (info != null)
                {
                    _templates.Add(info.menuName, info);
                }
            }

            _menuItems = _templates.Keys.ToArray();
            _menuItems = _menuItems.OrderBy(s => _templates[s].priority).ToArray();
        }

        private TemplateInfo ParseTemplate(string path)
        {
            TemplateInfo temp = new TemplateInfo();

            string template = File.ReadAllText(path);
            if (string.IsNullOrEmpty(template))
                return null;

            string pattern = @"&&(\w+) *= *(.?[\w/# ]+)&&\n?";
            Regex reg = new Regex(pattern, RegexOptions.Multiline);
            Match match = reg.Match(template);

            temp.specialKeys = new Dictionary<string, string>();

            while (match.Success)
            {
                string key = match.Groups[1].Value.ToUpper();
                string value = match.Groups[2].Value;

                if (!temp.specialKeys.ContainsKey(key))
                {
                    temp.specialKeys.Add(key, value);
                }

                template = template.Replace(match.Groups[0].Value, "");
                match = match.NextMatch();
            }

            if (!temp.specialKeys.ContainsKey("EXTENSION"))
            {
                temp.specialKeys.Add("EXTENSION", ".cs");
            }

            pattern = @"##(\w+)##";
            reg = new Regex(pattern, RegexOptions.Multiline);
            match = reg.Match(template);

            temp.replacements = new Dictionary<string, string>();
            temp.replacements.Add("ClassName", "");

            while (match.Success)
            {
                string key = match.Groups[1].Value;

                if (!temp.replacements.ContainsKey(key))
                {
                    temp.replacements.Add(key, "");
                    if (key == "Year")
                    {
                        temp.replacements[key] = System.DateTime.Now.Year.ToString();
                    }
                    else if (key == "Month")
                    {
                        temp.replacements[key] = System.DateTime.Now.Month.ToString();
                    }
                    else if (key == "Day")
                    {
                        temp.replacements[key] = System.DateTime.Now.Day.ToString();
                    }
                }

                match = match.NextMatch();
            }

            if (!temp.specialKeys.TryGetValue("MENUNAME", out temp.menuName))
            {
                temp.menuName = Path.GetFileNameWithoutExtension(path);
            }

            temp.priority = 0;

            string priorityString;
            if (temp.specialKeys.TryGetValue("PRIORITY", out priorityString))
            {
                int priority;
                if (int.TryParse(priorityString, out priority))
                {
                    temp.priority = priority;
                }
            }

            temp.wholeTemplate = template;

            return temp;
        }

        private void CreateScript(TemplateInfo info)
        {
            string className = Path.GetFileNameWithoutExtension(info.replacements["ClassName"]);
            string template = info.wholeTemplate;
            string ext = info.specialKeys["EXTENSION"];

            foreach (KeyValuePair<string, string> pairs in info.replacements)
            {
                template = template.Replace("##" + pairs.Key + "##", pairs.Value);
            }

            string finalPath = Path.Combine(_createPath, className + ext.ToLower());

            if (File.Exists(finalPath))
            {
                Debug.LogError("File already exists: " + finalPath);
            }
            else
            {
                File.WriteAllText(finalPath, template, System.Text.Encoding.UTF8);
                AssetDatabase.Refresh();
            }
        }
    }
}