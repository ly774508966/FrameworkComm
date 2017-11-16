using System.Collections;
using System.Collections.Generic;
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
        // window
        private const float fWindowMinWidth = 1280f;
        private const float fWindowMinHeight = 720f;
        // splitter
        private const float fSplitterWidth = 4f;
        // inspector
        private const float fInspectorMinWidth = 250f;
        // node
        private const float fNodeWidth = 150f;
        private const float fNodeHeight = 50f;
        // rect
        private Rect _rMainRect = new Rect(0f, 0f, fWindowMinWidth - fInspectorMinWidth - fSplitterWidth, fWindowMinHeight);
        private Rect _rInspectorRect = new Rect(fWindowMinWidth - fInspectorMinWidth, 0f, fInspectorMinWidth, fWindowMinHeight);
        private Rect _rSplitterRect = new Rect(fWindowMinWidth - fInspectorMinWidth - fSplitterWidth, 0f, fSplitterWidth, fWindowMinHeight);

        private float _fSplitterX;
        private float _preWindowWidth;

        private bool _bMainDragging = false;
        private bool _bSplitterDragging = false;

        private FlowGraph _graph;
        private FlowNode _curSelectNode;

        private Object _selectObject;

        [MenuItem("Window/Flow Editor", priority = 3)]
        static void Open()
        {
            FlowEditorWindow window = GetWindow<FlowEditorWindow>("Flow Editor", true);
            window.minSize = new Vector2(fWindowMinWidth, fWindowMinHeight);
            window.wantsMouseMove = true;

            //FlowGraph graph = CreateInstance<FlowGraph>();
            //graph.AddNode(FlowNode.Create(FlowNodeType.Start, 1, Rect.zero));
            //graph.AddNode(FlowNode.Create(FlowNodeType.Normal, 2, Rect.zero));
            //graph.AddNode(FlowNode.Create(FlowNodeType.End, 3, Rect.zero));
            //AssetDatabase.CreateAsset(graph, "Assets/Resources/Flow/Graph.asset");
        }

        void OnGUI()
        {
            HandleWindowSizeChanged();

            GUILayout.BeginArea(_rMainRect);
            DrawMain();
            GUILayout.EndArea();

            GUI.Box(_rSplitterRect, GUIContent.none);

            GUILayout.BeginArea(_rInspectorRect);
            DrawInspector();
            GUILayout.EndArea();

            HandleEvents();
        }

        void OnDestroy()
        {
            SaveGraph();
        }

        void HandleWindowSizeChanged()
        {
            float width = position.width;
            float height = position.height;

            if (width != _preWindowWidth)
            {
                MoveSplitter(width - _preWindowWidth);
                _preWindowWidth = width;
            }

            _rMainRect = new Rect(0f, 0f, _fSplitterX, height);
            _rInspectorRect = new Rect(_fSplitterX + fSplitterWidth, 0f, width - _fSplitterX - fSplitterWidth, height);
            _rSplitterRect = new Rect(_fSplitterX, 0f, fSplitterWidth, height);
        }

        void HandleEvents()
        {
            if (Event.current != null)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        {
                            if (Event.current.button == 0)
                            {
                                if (_rMainRect.Contains(Event.current.mousePosition))
                                {
                                    _bMainDragging = true;
                                    Event.current.Use();
                                }
                                else if (_rSplitterRect.Contains(Event.current.mousePosition))
                                {
                                    _bSplitterDragging = true;
                                    Event.current.Use();
                                }
                            }
                            else if (Event.current.button == 1)
                            {
                                if (_rMainRect.Contains(Event.current.mousePosition))
                                {
                                    HandlePopMenu();
                                    Event.current.Use();
                                    Repaint();
                                }
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        {
                            if (_bMainDragging)
                            {
                                // to do
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
                            if (!GUI.changed && _curSelectNode != null)
                            {
                                if (Event.current.keyCode == KeyCode.Delete)
                                {
                                    DeleteNodeInGraph();
                                    Event.current.Use();
                                    Repaint();
                                }

                                if (Event.current.keyCode == KeyCode.Escape)
                                {
                                    _curSelectNode = null;
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
            if (_graph == null) return;

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
            FlowNode.CreateInGraph(_graph, type, _graph.GetNodeCount() + 1, new Rect(mousePosition.x, mousePosition.y, fNodeWidth, fNodeHeight));
        }

        void DrawMain()
        {
            if (_graph == null) return;
        }

        void DrawInspector()
        {
            DrawObjectField();

            if (_graph == null) return;
        }

        void DrawObjectField()
        {
            Object newSelectObject = EditorGUILayout.ObjectField(_selectObject, typeof(Object), false);

            if (newSelectObject != null)
            {
                if (newSelectObject != _selectObject)
                {
                    _graph = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(_selectObject = newSelectObject), typeof(FlowGraph)) as FlowGraph;
                }
            }
            else
            {
                _selectObject = null;
            }
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
            if (_curSelectNode != null)
            {
                _graph.RemoveNode(_curSelectNode);
                DestroyImmediate(_curSelectNode, true);
                _curSelectNode = null;
                return true;
            }
            return false;
        }

        void SaveGraph()
        {
            if (_graph != null)
            {
                EditorUtility.SetDirty(_graph);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}