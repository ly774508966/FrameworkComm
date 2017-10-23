using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Dynamic Horizontal TabPage Component
/// 1.tab node tree: ScrollRect / Mask / ToggleGroup(Content) / Toggle(tabitem) ...
/// 2.Page node tree: ScrollRect / Mask / ... / ...
/// by zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UITabPageList : BaseView
    {
        public UISimpleObjectPool tabPool;
        public UISimpleObjectPool pagePool;
        public Transform tabContentPanel;
        public Transform pageContentPanel;
        public Button arrowPrevious;
        public Button arrowNext;

        // tab count we can see in panel
        public int tabSizeInPanel;
        // scroll speed
        public float scrollSmoothing = 10.0f;

        private ToggleGroup _tabToggleGroup;
        private ScrollRect _tabScrollRect;
        private ScrollRect _pageScrollRect;
        
        private object[] _datas;
        public object[] datas
        {
            set { _datas = value; InvalidView(); }
        }
        
        public int dataLength
        {
            get { return _datas != null ? _datas.Length : 0; }
        }

        // default tab index, support jump to any tab page
        private int _jumpIndex = -1;
        public int JumpIndex
        {
            get { return _jumpIndex; }
            set { _jumpIndex = value; }
        }

        private bool _isTabHorizontalScrolling = false;
        private bool _isPageHorizontalScrolling = false;
        private float _targetTabHorizontalPosition = 0.0f;
        private float _targetPageHorizontalPosition = 0.0f;

        private int _currentPageIndex = 0;
        private bool _init = false;

        protected override void Start()
        {
            // tab ToggleGroup
            _tabToggleGroup = tabContentPanel.GetComponent<ToggleGroup>();
            _tabToggleGroup.allowSwitchOff = false;

            // tab ScrollRect
            _tabScrollRect = tabContentPanel.GetComponentInParent<ScrollRect>();
            _tabScrollRect.onValueChanged.AddListener(TabRectChange);
            _tabScrollRect.vertical = false;
            _tabScrollRect.horizontal = true;

            // page ScrollRect
            _pageScrollRect = pageContentPanel.GetComponentInParent<ScrollRect>();
            _pageScrollRect.vertical = false;   // disable vertical scroll temporarily
            _pageScrollRect.horizontal = false;

            // arrow click event & default hide arrow
            arrowPrevious.onClick.AddListener(TabPrevious);
            arrowNext.onClick.AddListener(TabNext);
            arrowPrevious.gameObject.SetActive(false);
            arrowNext.gameObject.SetActive(false);
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            RemoveItems();
            AddItems();
        }

        protected override void Update()
        {
            base.Update();

            // tab scroll
            if (_isTabHorizontalScrolling)
            {
                if (Mathf.Abs(_tabScrollRect.horizontalNormalizedPosition - _targetTabHorizontalPosition) <= 0.001f)    // Magic number based on what "feels right"
                {
                    _tabScrollRect.horizontalNormalizedPosition = _targetTabHorizontalPosition;
                    _isTabHorizontalScrolling = false;
                }
                else
                {
                    _tabScrollRect.horizontalNormalizedPosition = Mathf.Lerp(_tabScrollRect.horizontalNormalizedPosition, _targetTabHorizontalPosition, Time.deltaTime * scrollSmoothing);
                }
            }

            // page scroll
            if (_isPageHorizontalScrolling)
            {
                if (Mathf.Abs(_pageScrollRect.horizontalNormalizedPosition - _targetPageHorizontalPosition) <= 0.001f)
                {
                    _pageScrollRect.horizontalNormalizedPosition = _targetPageHorizontalPosition;
                    _isPageHorizontalScrolling = false;
                }
                else
                {
                    _pageScrollRect.horizontalNormalizedPosition = Mathf.Lerp(_pageScrollRect.horizontalNormalizedPosition, _targetPageHorizontalPosition, Time.deltaTime * scrollSmoothing);
                }
            }
        }

        void RemoveItems()
        {
            while (tabContentPanel.childCount > 0)
            {
                GameObject toRemove = tabContentPanel.GetChild(0).gameObject;
                UIBaseCell baseCell = toRemove.GetComponent<UIBaseCell>();
                baseCell.data = null;
                baseCell.index = 0;
                tabPool.ReturnObject(toRemove);
            }

            while (pageContentPanel.childCount > 0)
            {
                GameObject toRemove = pageContentPanel.GetChild(0).gameObject;
                UIBaseCell baseCell = toRemove.GetComponent<UIBaseCell>();
                baseCell.data = null;
                baseCell.index = 0;
                pagePool.ReturnObject(toRemove);
            }
        }

        void AddItems()
        {
            int length = dataLength;
            for (int i = 0; i < length; i++)
            {
                GameObject tabGo = tabPool.GetObject();
                tabGo.name = i.ToString();
                tabGo.transform.SetParent(tabContentPanel, false);
                GameObject pageGo = pagePool.GetObject();
                pageGo.name = i.ToString();
                pageGo.transform.SetParent(pageContentPanel, false);

                Toggle tabToggle = tabGo.GetComponent<Toggle>();
                if (tabToggle != null)
                {
                    tabToggle.group = _tabToggleGroup;
                    tabToggle.onValueChanged.AddListener(TabOnChange);
                    tabToggle.isOn = !_init && i == 0 ? true : false;
                }

                UIBaseCell tabCell = tabGo.GetComponent<UIBaseCell>();
                UIBaseCell pageCell = pageGo.GetComponent<UIBaseCell>();
                tabCell.data = _datas[i];
                tabCell.index = i;
                pageCell.data = _datas[i];
                pageCell.index = i;
            }

            // jump to specific tab
            if (_jumpIndex >= 0 && !_init)
            {
                iTweenUtils.CreateTimeout(gameObject, delegate ()
                {
                    SetTab(_jumpIndex);
                    ScrollToTab(_jumpIndex);
                    _jumpIndex = -1;
                }, 0.5f, "UITabPageList_ScrollToTab");
            }

            // forbid scroll if tab count less than tabSizeInPanel
            if (length <= tabSizeInPanel)
                _tabScrollRect.enabled = false;
            else
                _tabScrollRect.enabled = true;

            _tabScrollRect.content.anchoredPosition = Vector2.zero;
            _pageScrollRect.content.anchoredPosition = Vector2.zero;

            _init = length > 0;
        }

        void TabOnChange(bool active)
        {
            if (active)
            {
                IEnumerator toggles = _tabToggleGroup.ActiveToggles().GetEnumerator();
                while (toggles.MoveNext())
                {
                    Toggle toggle = toggles.Current as Toggle;
                    if (toggle.isOn)
                    {
                        ScrollToPage(int.Parse(toggle.gameObject.name));
                        break;
                    }
                }
            }
        }
        
        void TabRectChange(Vector2 position)
        {
            if (dataLength <= tabSizeInPanel)
            {
                arrowPrevious.gameObject.SetActive(false);
                arrowNext.gameObject.SetActive(false);
                return;
            }

            int scope = dataLength - tabSizeInPanel;
            float left = 1.0f / (scope * 2.0f);
            float right = 1.0f - left;

            arrowPrevious.gameObject.SetActive(position.x >= left);
            arrowNext.gameObject.SetActive(position.x <= right);
        }

        void ScrollToTab(int index)
        {
            if (dataLength <= tabSizeInPanel)
                return;

            if (index >= 0 && index < dataLength)
            {
                _targetTabHorizontalPosition = Mathf.Clamp01(index / (float)(dataLength - tabSizeInPanel));
                _isTabHorizontalScrolling = true;
            }
        }

        void ScrollToPage(int index)
        {
            if (dataLength <= 1)
                return;

            if (index >= 0 && index < dataLength)
            {
                _targetPageHorizontalPosition = Mathf.Clamp01(index / (float)(dataLength - 1));
                _isPageHorizontalScrolling = true;

                _currentPageIndex = index;
            }
        }

        void SetTab(int index)
        {
            if (index >= 0 && index < tabContentPanel.childCount)
            {
                tabContentPanel.GetChild(index).GetComponent<Toggle>().isOn = true;
            }
        }

        void TabPrevious()
        {
            if (_currentPageIndex > 0)
            {
                SetTab(--_currentPageIndex);

                if (_tabScrollRect.horizontalNormalizedPosition > 0.0f)
                {
                    _targetTabHorizontalPosition = Mathf.Clamp01(_tabScrollRect.horizontalNormalizedPosition - 1.0f / (dataLength - tabSizeInPanel));
                    _isTabHorizontalScrolling = true;
                }
            }
        }

        void TabNext()
        {
            if (_currentPageIndex < tabContentPanel.childCount)
            {
                SetTab(++_currentPageIndex);

                if (_tabScrollRect.horizontalNormalizedPosition < 1.0f)
                {
                    _targetTabHorizontalPosition = Mathf.Clamp01(_tabScrollRect.horizontalNormalizedPosition + 1.0f / (dataLength - tabSizeInPanel));
                    _isTabHorizontalScrolling = true;
                }
            }
        }

        protected override void OnDestroy()
        {
            _datas = null;
            iTweenUtils.ClearTimeout(gameObject, "UITabPageList_ScrollToTab");
            base.OnDestroy();
        }
    }
}
