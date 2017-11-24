using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 流式编辑器
/// @zhenhaiwang
/// </summary>
namespace Framework.Editor
{
    public class FlowEditorWindow : EditorWindow
    {
        // Path
        private const string sGraphFilePath = "Assets/Resources/Flow/";
        private const string sResourcePath = "Assets/Editor/FlowEditor/__Resources/";
        // Window
        private const float fWindowMinWidth = 1280f;
        private const float fWindowMinHeight = 720f;
        // Splitter
        private const float fSplitterWidth = 4f;
        // Inspector
        private const float fInspectorMinWidth = 250f;
        // MiniMap
        private const float fMiniMapScale = 0.1f;
        // Texture2D
        private const float fLinkIconWidth = 16f;
        private static Texture2D texLinkin;
        private static Texture2D texLinkout;
        private static Texture2D texUnlink;
        // GUIStyle
        private static GUISkin windowSkin;
        private static GUIStyle iconButtonStyle;
        private static Color nodeSelectedColor = Color.red;
        // Rect
        private Rect _rectMain = new Rect(0f, 0f, fWindowMinWidth - fInspectorMinWidth - fSplitterWidth, fWindowMinHeight);
        private Rect _rectInspector = new Rect(fWindowMinWidth - fInspectorMinWidth, 0f, fInspectorMinWidth, fWindowMinHeight);
        private Rect _rectSplitter = new Rect(fWindowMinWidth - fInspectorMinWidth - fSplitterWidth, 0f, fSplitterWidth, fWindowMinHeight);

        private Vector2 _inspectorScroll = Vector2.zero;

        private float _fSplitterX;
        private float _fPreWindowWidth;

        private bool _bMainDragging = false;
        private bool _bSplitterDragging = false;

        private FlowGraph _curFlowGraph;
        private FlowNode _curSelectFlowNode;
        private FlowNode _curLinkingFlowNode;

        private string _curAssetPath = "";
        private Object _curSelectAsset;

        [MenuItem("Window/Flow Editor", priority = 3)]
        public static FlowEditorWindow Open()
        {
            FlowEditorWindow window = GetWindow<FlowEditorWindow>("Flow Editor", true);
            window.minSize = new Vector2(fWindowMinWidth, fWindowMinHeight);
            window.wantsMouseMove = true;
            return window;
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

        void OnLostFocus()
        {
            _curLinkingFlowNode = null;
            _bMainDragging = false;
            _bSplitterDragging = false;
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

        #region Handle
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
                                        GUI.FocusControl("");
                                        Event.current.Use();
                                    }
                                    else if (_rectSplitter.Contains(Event.current.mousePosition))
                                    {
                                        _bSplitterDragging = true;
                                        Event.current.Use();
                                    }
                                    else if (_rectInspector.Contains(Event.current.mousePosition))
                                    {
                                        GUI.FocusControl("");
                                        Event.current.Use();
                                    }
                                }
                                else if (Event.current.button == 1)
                                {
                                    GUI.FocusControl("");

                                    if (_rectMain.Contains(Event.current.mousePosition))
                                    {
                                        HandlePopMenu();
                                        Event.current.Use();
                                        Repaint();
                                    }
                                    else if (_rectInspector.Contains(Event.current.mousePosition))
                                    {
                                        Event.current.Use();
                                    }
                                }
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        {
                            if (_bMainDragging)
                            {
                                _curFlowGraph.graphOffset += new Vector2(Event.current.delta.x, Event.current.delta.y);
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

            for (FlowNodeType type = FlowNodeType.Start; type < FlowNodeType.Count; type++)
            {
                if (type == FlowNodeType.End) menu.AddSeparator("");

                menu.AddItem(new GUIContent(type.ToString()), false, HandleClickMenuItem, new object[] { type, Event.current.mousePosition });

                if (type == FlowNodeType.Start) menu.AddSeparator("");
            }

            menu.ShowAsContext();
        }

        void HandleClickMenuItem(object args)
        {
            object[] argArray = args as object[];
            FlowNodeType type = (FlowNodeType)argArray[0];
            Vector2 mousePosition = (Vector2)argArray[1];
            FlowNode.CreateFromGraph(_curFlowGraph, type, _curFlowGraph.NodeNextID, new Vector2(mousePosition.x, mousePosition.y));
        }
        #endregion

        #region Draw
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
                DrawMiniMap();

                if (_curFlowGraph.NodeCount > 0)
                {
                    Handles.BeginGUI();

                    foreach (FlowNode node in _curFlowGraph.NodeList)
                    {
                        if (node == null)
                        {
                            Debug.Log("[FlowEditorWindow]DrawMain node is null");
                            continue;
                        }

                        if (node.linkList == null)
                        {
                            continue;
                        }

                        FlowNode deleteNode = null;
                        foreach (int linkId in node.linkList)
                        {
                            FlowNode linkNode = _curFlowGraph.GetNode(linkId);
                            Rect nodeRect = node.GetRectInGraph(_curFlowGraph);
                            Rect linkRect = linkNode.GetRectInGraph(_curFlowGraph);
                            if (DrawBezier(new Vector2(nodeRect.x + nodeRect.width, nodeRect.y + fLinkIconWidth / 2f), new Vector2(linkRect.x, linkRect.y + fLinkIconWidth / 2f), Color.yellow))
                            {
                                deleteNode = linkNode;
                            }
                        }

                        if (deleteNode != null)
                        {
                            node.RemoveLinkNode(deleteNode);
                            deleteNode.RemovePreNode(node);
                        }
                    }

                    Handles.EndGUI();
                }

                BeginWindows();

                List<FlowNode> nodeList = _curFlowGraph.NodeList;
                int nodeCount = _curFlowGraph.NodeCount;
                for (int i = 0; i < nodeCount; i++)
                {
                    FlowNode node = nodeList[i];

                    Rect rect = node.GetRectInGraph(_curFlowGraph);
                    Vector2 topLeft = new Vector2(rect.x, rect.y);
                    Vector2 topRight = new Vector2(rect.x + node.NodeWidth, rect.y);
                    Vector2 bottomLeft = new Vector2(rect.x, rect.y + node.NodeHeight);
                    Vector2 bottomRight = new Vector2(rect.x + node.NodeWidth, rect.y + node.NodeHeight);

                    if (_rectMain.Contains(topLeft) ||
                        _rectMain.Contains(topRight) ||
                        _rectMain.Contains(bottomLeft) ||
                        _rectMain.Contains(bottomRight))
                    {
                        if (node == _curSelectFlowNode)
                        {
                            GUI.color = nodeSelectedColor;
                        }
                        else
                        {
                            GUI.color = node.GetColor();
                        }

                        rect = GUI.Window(node.id, rect, DrawNode, node.NodeName);

                        GUI.color = Color.white;
                    }

                    node.SetRectInGraph(_curFlowGraph, rect.position);
                }

                DrawLinking();

                EndWindows();
            }
        }

        void DrawInspector()
        {
            DrawObjectField();

            if (_curFlowGraph == null) return;

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                SaveGraph();
            }

            _inspectorScroll = GUILayout.BeginScrollView(_inspectorScroll, GUILayout.Width(_rectInspector.width), GUILayout.Height(_rectInspector.height - 30f));
            {
                if (_curSelectFlowNode != null)
                {
                    _curSelectFlowNode.OnDrawProperty();
                }
            }
            GUILayout.EndScrollView();
        }

        void DrawMiniMap()
        {
            if (!_bMainDragging) return;

            Color preColor = GUI.color;
            Color mapColor = GUI.color * new Color(1f, 1f, 1f, 0.05f);

            Vector2 mapCenter = new Vector2(_rectMain.x + _rectMain.width * 0.15f, _rectMain.y + _rectMain.height * 0.15f);
            mapCenter.x -= _rectMain.width * fMiniMapScale / 2f;
            mapCenter.y -= _rectMain.height * fMiniMapScale / 2f;

            GUI.color = mapColor;

            GUI.Box(new Rect(mapCenter.x, mapCenter.y, _rectMain.width * fMiniMapScale, _rectMain.height * fMiniMapScale), "");

            foreach (FlowNode node in _curFlowGraph.NodeList)
            {
                Rect rect = node.GetRectInGraph(_curFlowGraph);
                Vector2 nodeCenter = new Vector2(rect.x, rect.y);
                Vector2 nodeSize = new Vector2(rect.width, rect.height);
                nodeCenter *= fMiniMapScale;
                nodeSize *= fMiniMapScale;
                nodeCenter += mapCenter;
                GUI.color = node.GetColor();
                GUI.Box(new Rect(nodeCenter.x, nodeCenter.y, nodeSize.x, nodeSize.y), GUIContent.none);
            }

            GUI.color = preColor;
        }

        void DrawObjectField()
        {
            Object newSelectAsset = EditorGUILayout.ObjectField(_curSelectAsset, typeof(Object), false);

            if (newSelectAsset != _curSelectAsset)
            {
                if (newSelectAsset != null)
                {
                    CreateGraph(newSelectAsset);
                }
                else
                {
                    ClearGraph();
                }
            }
        }

        void DrawNode(int id)
        {
            FlowNode node = _curFlowGraph.GetNode(id);

            node.OnDrawNode();

            if (node.type != FlowNodeType.End)
            {
                if (GUI.Button(new Rect(node.NodeWidth - fLinkIconWidth, 0f, fLinkIconWidth, fLinkIconWidth), new GUIContent(texLinkout), iconButtonStyle))
                {
                    _curLinkingFlowNode = node;
                }
            }

            if (node.type != FlowNodeType.Start)
            {
                if (GUI.Button(new Rect(0f, 0f, fLinkIconWidth, fLinkIconWidth), new GUIContent(texLinkin), iconButtonStyle))
                {
                    if (_curLinkingFlowNode != null)
                    {
                        _curLinkingFlowNode.AddLinkNode(node);
                        node.AddPreNode(_curLinkingFlowNode);
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
            DrawBezier(new Vector2(nodeRect.x + nodeRect.width, nodeRect.y + fLinkIconWidth / 2f), Event.current.mousePosition, Color.white);

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

            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, null, 4f);

            Rect deleteRect = new Rect(startPos.x + (endPos.x - startPos.x) * 0.5f - fLinkIconWidth / 2f, startPos.y + (endPos.y - startPos.y) * 0.5f - fLinkIconWidth / 2f, fLinkIconWidth, fLinkIconWidth);
            if (GUI.Button(deleteRect, new GUIContent(texUnlink), iconButtonStyle))
            {
                return true;
            }

            return false;
        }
        #endregion

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
                _curSelectFlowNode = null;
                return true;
            }
            return false;
        }

        void ClearGraph()
        {
            _curSelectAsset = null;
            _curFlowGraph = null;
            _curAssetPath = "";
        }

        public void CreateGraph(Object asset)
        {
            _curSelectAsset = asset;
            _curAssetPath = AssetDatabase.GetAssetPath(_curSelectAsset);
            _curFlowGraph = FlowGraph.LoadFromAsset(_curSelectAsset);
        }

        void SaveGraph()
        {
            if (_curFlowGraph != null)
            {
                if (_curSelectAsset == null)
                {
                    string path = EditorUtility.SaveFilePanel(
                        "Save flow graph as asset",
                        sGraphFilePath,
                        "Flow Graph.asset",
                        "asset");

                    if (path.Length > 0)
                    {
                        _curAssetPath = sGraphFilePath + Path.GetFileName(path);
                        _curSelectAsset = _curFlowGraph.Save(_curAssetPath, true);
                    }
                }
                else
                {
                    _curFlowGraph.Save(_curAssetPath, false);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}