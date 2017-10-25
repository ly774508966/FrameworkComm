using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [AddComponentMenu("UI/Scroll Rect/Loop Vertical Scroll Rect")]
    public class UILoopVerticalScrollRect : UILoopScrollRect
    {
        enum LoadingStage
        {
            None,
            MoveToBottom,
            Loading,
        }

        public bool autoLoading = false;
        public float loadingDistance = 30f;
        public GameObject loadingObject;

        public Action startLoadingAction;

        LoadingStage _updateStage = LoadingStage.None;

        public override void FinishLoading()
        {
            if (_updateStage == LoadingStage.Loading)
            {
                _updateStage = LoadingStage.None;
                if (loadingObject != null)
                {
                    loadingObject.SetActive(false);
                    loadingObject.transform.SetParent(transform, false);
                }
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (_updateStage == LoadingStage.None && autoLoading)
            {
                if (verticalNormalizedPosition > 1.0f && (content.anchoredPosition.y + viewRect.rect.height - content.rect.height) > loadingDistance)
                {
                    _updateStage = LoadingStage.MoveToBottom;
                }
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (_updateStage == LoadingStage.MoveToBottom)
            {
                if (loadingObject != null)
                {
                    loadingObject.transform.SetParent(content, false);
                    loadingObject.transform.SetAsLastSibling();
                    loadingObject.SetActive(true);
                }

                _updateStage = LoadingStage.Loading;

                startLoadingAction.Call();
            }
        }

        protected override float GetSize(RectTransform cell)
        {
            float size = contentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.y;
            }
            else
            {
                size += LayoutUtility.GetPreferredHeight(cell);
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }

        protected override void Awake()
        {
            base.Awake();

            directionSign = -1;

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            {
                Debug.LogError("[UILoopVerticalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateCells(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;

            if (viewBounds.min.y < contentBounds.min.y + 1)
            {
                float size = NewCellAtEnd();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.min.y > contentBounds.min.y + threshold)
            {
                if (_updateStage == LoadingStage.Loading)
                {
                    if (content.childCount > 0)
                    {
                        Transform firstChild = content.GetChild(content.childCount - 1);
                        if (loadingObject == firstChild.gameObject)
                        {
                            loadingObject.SetActive(false);
                            loadingObject.transform.SetParent(transform, false);
                        }
                    }
                }

                float size = DeleteCellAtEnd();
                if (size > 0)
                {
                    changed = true;
                }
            }
            if (viewBounds.max.y > contentBounds.max.y - 1)
            {
                float size = NewCellAtStart();
                if (size > 0)
                {
                    if (threshold < size)
                    {
                        threshold = size * 1.1f;
                    }
                    changed = true;
                }
            }
            else if (viewBounds.max.y < contentBounds.max.y - threshold)
            {
                if (_updateStage == LoadingStage.Loading)
                {
                    if (content.childCount > 0)
                    {
                        Transform firstChild = content.GetChild(0);
                        if (loadingObject == firstChild.gameObject)
                        {
                            loadingObject.SetActive(false);
                            loadingObject.transform.SetParent(transform, false);
                        }
                    }
                }
                float size = DeleteCellAtStart();
                if (size > 0)
                {
                    changed = true;
                }
            }

            return changed;
        }
    }
}
