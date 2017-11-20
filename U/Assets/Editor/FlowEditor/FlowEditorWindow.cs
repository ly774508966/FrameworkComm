using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 流式编辑器
/// @zhenhaiwang
/// </summary>
namespace Assets.Editor
{
    public class FlowEditorWindow : EditorWindow
    {
        // Path
        private const string sResourcePath = "Assets/Editor/FlowEditor/__Resources/";
        private const string sGraphFilePath = "Assets/Resources/Flow/";
        // Window
        private const float fWindowMinWidth = 1280f;
        private const float fWindowMinHeight = 720f;
        // Splitter
        private const float fSplitterWidth = 4f;
        // Inspector
        private const float fInspectorMinWidth = 250f;
        // Texture2D
        private const float fLinkIconWidth = 16f;
        private static Texture2D texLinkin;
        private static Texture2D texLinkout;
        private static Texture2D texUnlink;
        // GUIStyle
        private static GUISkin windowSkin;
        private static GUIStyle iconButtonStyle;
        // Rect
        private Rect _rectMain = new Rect(0f, 0f, fWindowMinWidth - fInspectorMinWidth - fSplitterWidth, fWindowMinHeight);
        private Rect _rectInspector = new Rect(fWindowMinWidth - fInspectorMinWidth, 0f, fInspectorMinWidth, fWindowMinHeight);
        private Rect _rectSplitter = new Rect(fWindowMinWidth - fInspectorMinWidth - fSplitterWidth, 0f, fSplitterWidth, fWindowMinHeight);

        private float _fSplitterX;
        private float _fPreWindowWidth;

        private bool _bMainDragging = false;
        private bool _bSplitterDragging = false;

        private FlowGraph _curFlowGraph;
        private FlowNode _curSelectFlowNode;
        private FlowNode _curLinkingFlowNode;

        private Object _selectObject;

        [MenuItem("Window/Flow Editor", priority = 3)]
        static void Open()
        {
            FlowEditorWindow window = GetWindow<FlowEditorWindow>("Flow Editor", true);
            window.minSize = new Vector2(fWindowMinWidth, fWindowMinHeight);
            window.wantsMouseMove = true;
        }

        void OnGUI()
        {
            LoadResources();
            SetGUIStyle();

            HandleWindowSizeChanged();

            GUILayout.BeginArea(_rectMain);
            DrawMain();
            GUILayout.EndArea();

            GUI.Box(_rectSplitter, GUIContent.none);

            GUILayout.BeginArea(_rectInspector);
            DrawInspector();
            GUILayout.EndArea();

            HandleEvents();
        }

        void OnDestroy()
        {
            SaveGraph();
        }

        void LoadResources()
        {
            if (texLinkin == null)
            {
                texLinkin = AssetDatabase.LoadAssetAtPath(sResourcePath + "linkin.png", typeof(Texture2D)) as Texture2D;
            }
            if (texLinkout == null)
            {
                texLinkout = AssetDatabase.LoadAssetAtPath(sResourcePath + "linkout.png", typeof(Texture2D)) as Texture2D;
            }
            if (texUnlink == null)
            {
                texUnlink = AssetDatabase.LoadAssetAtPath(sResourcePath + "unlink.png", typeof(Texture2D)) as Texture2D;
            }
        }

        void SetGUIStyle()
        {
            if (windowSkin == null)
            {
                windowSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
                iconButtonStyle = new GUIStyle()
                {
                    name = "ButtonStyle",
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    fixedWidth = 16,
                    fixedHeight = 16,
                    imagePosition = ImagePosition.ImageOnly,
                };
                GUIStyle[] customStyles = windowSkin.customStyles;
                ArrayUtility.Add(ref customStyles, iconButtonStyle);
                windowSkin.customStyles = customStyles;
            }

            GUI.skin = windowSkin;
        }

        void HandleWindowSizeChanged()
        {
            float width = position.width;
            float height = position.height;

            if (width != _fPreWindowWidth)
            {
                MoveSplitter(width - _fPreWindowWidth);
                _fPreWindowWidth = width;
            }

            _rectMain = new Rect(0f, 0f, _fSplitterX, height);
            _rectInspector = new Rect(_fSplitterX + fSplitterWidth, 0f, width - _fSplitterX - fSplitterWidth, height);
            _rectSplitter = new Rect(_fSplitterX, 0f, fSplitterWidth, height);
        }

        void HandleEvents()
        {
            if (Event.current != null)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        {
                            if (_curLinkingFlowNode != null)
                            {
                                _curLinkingFlowNode = null;
                                Event.current.Use();
                                Repaint();
                            }
                            else
                            {
                                if (Event.current.button == 0)
                                {
                                    if (_rectMain.Contains(Event.current.mousePosition))
                                    {
                                        _bMainDragging = true;
                                        Event.current.Use();
                                    }
                                    else if (_rectSplitter.Contains(Event.current.mousePosition))
                                    {
                                        _bSplitterDragging = true;
                                        Event.current.Use();
                                    }
                                }
                                else if (Event.current.button == 1)
                                {
                                    if (_rectMain.Contains(Event.current.mousePosition))
                                    {
                                        HandlePopMenu();
                                        Event.current.Use();
                                        Repaint();
                                    }
                                }
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        {
                            if (_bMainDragging)
                            {
                                _curFlowGraph.offset += new Vector2(Event.current.delta.x, Event.current.delta.y);
                                Event.current.Use();
                            }
                            else if (_bSplitterDragging)
                            {
                                MoveSplitter(Event.current.delta.x);
                                Event.current.Use();
                            }
                        }
                        break;
                    case EventType.MouseUp:
                        {
                            _bMainDragging = false;
                            _bSplitterDragging = false;
                            Event.current.Use();
                        }
                        break;
                    case EventType.KeyDown:
                        {
                            if (!GUI.changed && _curSelectFlowNode != null)
                            {
                                if (Event.current.keyCode == KeyCode.Delete)
                                {
                                    DeleteNodeInGraph();
                                    Event.current.Use();
                                    Repaint();
                                }

                                if (Event.current.keyCode == KeyCode.Escape)
                                {
                                    _curSelectFlowNode = null;
                                    Event.current.Use();
                                    Repaint();
                                }
                            }
                        }
                        break;
                }
            }
        }

        void HandlePopMenu()
        {
            if (_curFlowGraph == null) return;

            GenericMenu menu = new GenericMenu();

            for (FlowNodeType type = FlowNodeType.None + 1; type < FlowNodeType.Count; type++)
            {
                menu.AddItem(new GUIContent(type.ToString()), false, HandleClickMenuItem, new object[] { type, Event.current.mousePosition });
            }

            menu.ShowAsContext();
        }

        void HandleClickMenuItem(object args)
        {
            object[] argArray = args as object[];
            FlowNodeType type = (FlowNodeType)argArray[0];
            Vector2 mousePosition = (Vector2)argArray[1];
            FlowNode.CreateInGraph(_curFlowGraph, type, _curFlowGraph.GetNodeCount() + 1, new Vector2(mousePosition.x, mousePosition.y));
        }

        void DrawMain()
        {
            if (_curFlowGraph == null)
            {
                if (GUI.Button(new Rect(_fSplitterX / 2f - 50f, position.height / 2f - 15f, 100f, 30f), "Create"))
                {
                    _curFlowGraph = CreateInstance<FlowGraph>();
                }
            }
            else
            {
                Handles.BeginGUI();
                foreach (FlowNode node in _curFlowGraph.nodeList)
                {
                    if (node == null)
                    {
                        Debug.Log("[FlowEditorWindow]DrawMain node is null");
                        continue;
                    }

                    if (node.linkList == null)
                    {
                        Debug.Log("[FlowEditorWindow]DrawMain node.linkList is null"); continue;
                    }

                    FlowNode deleteNode = null;
                    foreach (int linkId in node.linkList)
                    {
                        FlowNode linkNode = _curFlowGraph.GetNode(linkId);
                        Rect nodeRect = node.GetRectInGraph(_curFlowGraph);
                        Rect linkRect = linkNode.GetRectInGraph(_curFlowGraph);
                        if (DrawBezier(new Vector2(nodeRect.x + node.NodeWidth, nodeRect.y + fLinkIconWidth / 2f), new Vector2(linkRect.x, linkRect.y + fLinkIconWidth / 2f), Color.yellow))
                        {
                            deleteNode = linkNode;
                        }
                    }

                    if (deleteNode != null)
                    {
                        node.RemoveLinkNode(deleteNode);
                    }
                }
                Handles.EndGUI();

                BeginWindows();

                int graphCount = _curFlowGraph.GetNodeCount();
                for (int i = 0; i < graphCount; i++)
                {
                    FlowNode node = _curFlowGraph.nodeList[i];
                    Rect position = node.GetRectInGraph(_curFlowGraph);
                    Vector2 topLeft = new Vector2(position.x, position.y);
                    Vector2 topRight = new Vector2(position.x + node.rect.width, position.y);
                    Vector2 bottomLeft = new Vector2(position.x, position.y + node.rect.height);
                    Vector2 bottomRight = new Vector2(position.x + node.rect.width, position.y + node.rect.height);

                    if (_rectMain.Contains(topLeft) ||
                        _rectMain.Contains(topRight) ||
                        _rectMain.Contains(bottomLeft) ||
                        _rectMain.Contains(bottomRight))
                    {
                        if (node == _curSelectFlowNode)
                        {
                            GUI.color = Color.red;
                        }
                        else
                        {
                            GUI.color = Color.white;
                        }

                        position = GUI.Window(node.id, position, DrawNode, node.Name);

                        GUI.color = Color.white;
                    }

                    node.SetRectInGraph(_curFlowGraph, position);
                }

                DrawLinking();

                EndWindows();
            }
        }

        void DrawInspector()
        {
            DrawObjectField();

            if (_curFlowGraph == null) return;

            // to do
        }

        void DrawObjectField()
        {
            Object newSelectObject = EditorGUILayout.ObjectField(_selectObject, typeof(Object), false);

            if (newSelectObject != null)
            {
                if (newSelectObject != _selectObject)
                {
                    _curFlowGraph = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(_selectObject = newSelectObject), typeof(FlowGraph)) as FlowGraph;
                }
            }
            else
            {
                _selectObject = null;
            }
        }

        void DrawNode(int id)
        {
            FlowNode node = _curFlowGraph.GetNode(id);

            if (GUI.Button(new Rect(node.rect.width - fLinkIconWidth, 0f, fLinkIconWidth, fLinkIconWidth), new GUIContent(texLinkout), iconButtonStyle))
            {
                _curLinkingFlowNode = node;
            }

            if (node.type != FlowNodeType.Start)
            {
                if (GUI.Button(new Rect(0f, 0f, fLinkIconWidth, fLinkIconWidth), new GUIContent(texLinkin), iconButtonStyle))
                {
                    if (_curLinkingFlowNode != null)
                    {
                        _curLinkingFlowNode.AddLinkNode(node);
                        _curLinkingFlowNode = null;
                    }
                }
            }

            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                _curSelectFlowNode = node;
                GUI.FocusWindow(id);
            }

            GUI.DragWindow();
        }

        void DrawLinking()
        {
            if (_curLinkingFlowNode == null || Event.current == null) return;

            Rect nodeRect = _curLinkingFlowNode.GetRectInGraph(_curFlowGraph);
            DrawBezier(new Vector2(nodeRect.x + _curLinkingFlowNode.NodeWidth, nodeRect.y + fLinkIconWidth / 2f), Event.current.mousePosition, Color.white);

            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }

        bool DrawBezier(Vector3 startPos, Vector3 endPos, Color color)
        {
            float left = startPos.x < endPos.x ? startPos.x : endPos.x;
            float right = startPos.x > endPos.x ? startPos.x : endPos.x;
            float top = startPos.y < endPos.y ? startPos.y : endPos.y;
            float bottom = startPos.y > endPos.y ? startPos.y : endPos.y;

            Rect bounds = new Rect(left, top, right - left, bottom - top);
            if (bounds.xMin > _rectMain.xMax ||
                bounds.xMax < _rectMain.xMin ||
                bounds.yMin > _rectMain.yMax ||
                bounds.yMax < _rectMain.yMin)
            {
                return false;
            }

            float distance = Mathf.Abs(startPos.x - endPos.x);
            Vector3 startTangent = new Vector3(startPos.x + distance / 2.5f, startPos.y);
            Vector3 endTangent = new Vector3(endPos.x - distance / 2.5f, endPos.y);

            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, null, 2f);

            Rect deleteRect = new Rect(startPos.x + (endPos.x - startPos.x) * 0.5f, startPos.y + (endPos.y - startPos.y) * 0.5f, 16, 16);
            if (GUI.Button(deleteRect, new GUIContent(texUnlink), iconButtonStyle))
            {
                return true;
            }

            return false;
        }

        void MoveSplitter(float deltaX)
        {
            _fSplitterX += deltaX;

            float curWindowWidth = position.width;

            if (_fSplitterX > curWindowWidth - fInspectorMinWidth)
            {
                _fSplitterX = curWindowWidth - fInspectorMinWidth;
            }
            else if (_fSplitterX < curWindowWidth / 2)
            {
                _fSplitterX = curWindowWidth / 2;
            }
        }

        bool DeleteNodeInGraph()
        {
            if (_curSelectFlowNode != null)
            {
                _curFlowGraph.RemoveNode(_curSelectFlowNode);
                DestroyImmediate(_curSelectFlowNode, true);
                _curSelectFlowNode = null;
                return true;
            }
            return false;
        }

        void SaveGraph()
        {
            if (_curFlowGraph != null)
            {
                if (_selectObject == null)
                {
                    string path = EditorUtility.SaveFilePanel(
                        "Save flow graph as asset",
                        sGraphFilePath,
                        "FlowGraph.asset",
                        "asset");

                    if (path.Length > 0)
                    {
                        AssetDatabase.CreateAsset(_curFlowGraph, sGraphFilePath + Path.GetFileName(path));
                    }
                }
                else
                {
                    EditorUtility.SetDirty(_curFlowGraph);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}