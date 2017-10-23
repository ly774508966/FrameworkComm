using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIScrollFanPage : BaseView, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public RectTransform content;

        public UISimpleObjectPool objectPool;

        public float deltaEulerAngle = 0f;

        public float inscribedPolygonWidth = 0f;
        public float inscribedPolygonHeight = 0f;

        public float scrollSmoothing = 8f;
        public float nextFanThreshold = 0.3f;

        private float _radius = 0f;

        private float _screenRatio = 1f;

        private float _targetEulerAngle = 0f;

        private float _beginDragEulerAngle = 0f;
        private float _beginDragTime = 0f;

        private bool _isAutoScrolling = false;

        private List<GameObject> _fanPageList = new List<GameObject>();

        private int _currentIndex = 0;
        private int currentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; CalculatePageEnabled(); }
        }

        private float deltaRadianAngle
        {
            get { return Mathf.Deg2Rad * deltaEulerAngle; }
        }

        private object[] _data = null;
        public object[] data
        {
            get { return _data; }
            set { _data = value; InvalidView(); }
        }

        protected override void Awake()
        {
            if (content != null)
            {
                CalculateScreenRatio();
                CalculateRadius();
                CalculateContentRect();
            }
        }

        protected override void Update()
        {
            if (_isAutoScrolling)
            {
                if (Mathf.Abs(content.localEulerAngles.z - _targetEulerAngle) <= 0.001f)
                {
                    _isAutoScrolling = false;
                    content.localEulerAngles = new Vector3(0f, 0f, _targetEulerAngle);
                }
                else
                {
                    content.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(content.localEulerAngles.z, _targetEulerAngle, Time.deltaTime * scrollSmoothing));
                }
            }
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            RemovePages();
            AddPages();
        }

        void AddPages()
        {
            _fanPageList.Clear();

            int length = _data != null ? _data.Length : 0;
            for (int i = 0; i < length; i++)
            {
                object pageData = _data[i];
                GameObject pageGo = objectPool.GetObject();
                pageGo.transform.SetParent(content, false);

                _fanPageList.Add(pageGo);

                UIBaseCell baseCell = pageGo.GetComponent<UIBaseCell>();
                baseCell.data = pageData;
                baseCell.index = i;
            }

            StartCoroutine(CalculatePagePositionAndAngle());
        }

        void RemovePages()
        {
            while (content.childCount > 0)
            {
                GameObject toRemove = content.transform.GetChild(0).gameObject;
                UIBaseCell baseCell = toRemove.GetComponent<UIBaseCell>();
                baseCell.data = null;
                baseCell.index = 0;
                objectPool.ReturnObject(toRemove);
            }
        }

        void CalculateScreenRatio()
        {
            CanvasScaler scaler = GetComponentInParent<CanvasScaler>();
            if (scaler != null)
            {
                _screenRatio = Screen.width / scaler.referenceResolution.x;
            }
        }

        IEnumerator CalculatePagePositionAndAngle()
        {
            yield return null;

            int length = _fanPageList != null ? _fanPageList.Count : 0;
            for (int i = 0; i < length; i++)
            {
                RectTransform rect = _fanPageList[i].GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(_radius * Mathf.Sin(deltaRadianAngle * i), _radius * Mathf.Cos(deltaRadianAngle * i));
                    rect.localEulerAngles = new Vector3(0f, 0f, -deltaEulerAngle * i);
                }
            }

            CalculatePageEnabled();
        }

        void CalculatePageEnabled()
        {
            int length = _fanPageList != null ? _fanPageList.Count : 0;
            for (int i = 0; i < length; i++)
            {
                _fanPageList[i].SetActive(i >= _currentIndex - 1 && i <= _currentIndex + 1);
            }
        }

        void CalculateRadius()
        {
            if (deltaEulerAngle > 0f &&
                inscribedPolygonWidth > 0f &&
                inscribedPolygonHeight > 0f)
            {
                _radius = inscribedPolygonWidth / (2f * Mathf.Tan(deltaRadianAngle / 2f)) + inscribedPolygonHeight / 2f;
            }
            else
            {
                _radius = 0f;
            }
        }

        void CalculateContentRect()
        {
            if (content != null && _radius > 0f)
            {
                content.anchorMin = content.anchorMax = content.pivot = new Vector2(0.5f, 0.5f);
                content.anchoredPosition = new Vector2(0f, -_radius);
                content.localEulerAngles = Vector2.zero;
                content.sizeDelta = new Vector2(_radius * 2f + inscribedPolygonHeight, _radius * 2f + inscribedPolygonHeight);
            }
        }

        bool CheckDraggable()
        {
            return _radius > 0 && _fanPageList != null && _fanPageList.Count > 1;
        }

        public void SetPage(int index)
        {
            if (_data != null && index >= 0 && index < _data.Length)
            {
                currentIndex = index;
                _targetEulerAngle = _currentIndex * deltaEulerAngle;
                content.localEulerAngles = new Vector3(0f, 0f, _targetEulerAngle);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CheckDraggable())
            {
                _isAutoScrolling = false;
                _beginDragTime = Time.unscaledTime;
                _beginDragEulerAngle = content.localEulerAngles.z;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CheckDraggable() && !_isAutoScrolling)
            {
                float dragOffset = Mathf.Clamp((eventData.position.x - eventData.pressPosition.x) / _screenRatio, -inscribedPolygonWidth, inscribedPolygonWidth);
                float deltaAngle = Mathf.Clamp01(Mathf.Abs(dragOffset) / inscribedPolygonWidth) * deltaEulerAngle;
                content.localEulerAngles = new Vector3(0f, 0f, dragOffset > 0 ? _beginDragEulerAngle - deltaAngle : _beginDragEulerAngle + deltaAngle);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (CheckDraggable() && !_isAutoScrolling)
            {
                float speed = (eventData.position.x - eventData.pressPosition.x) / ((Time.unscaledTime - _beginDragTime) * 1000.0f);

                if (speed >= nextFanThreshold && currentIndex > 0)
                {
                    _targetEulerAngle = --currentIndex * deltaEulerAngle;
                }
                else if (speed <= -nextFanThreshold && currentIndex < _fanPageList.Count - 1)
                {
                    _targetEulerAngle = ++currentIndex * deltaEulerAngle;
                }

                _isAutoScrolling = true;

                _beginDragEulerAngle = 0f;
                _beginDragTime = 0f;
            }
        }
    }
}
