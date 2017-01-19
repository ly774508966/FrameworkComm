using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Tab Page 通用组件， render脚本要派生自TabPageRender抽象类
    /// by zhenhaiwang
    /// </summary>
    public class TabPageList : FBaseController
    {
        public delegate void RenderEvent(string name, object data);
        public event RenderEvent renderEvent;
        // 小箭头
        public UI2DSprite previous;
        public UI2DSprite next;

        // 标签
        public int tabPanelSize;        // 当前Panel能够显示的最大标签数
        public UITable tabTable;
        public GameObject tabItemPrefab;

        // 页面
        public UITable pageTable;
        public GameObject pageItemPrefab;

        // 引用
        private UIScrollView _tabScrollView;
        private UIPanel _tabScrollPanel;
        private UIDragScrollView _pageDragScrollView;   // Panel父节点静态碰撞体
        private UICenterOnChild _centerOnChild;

        private int _groupId = 1;
        private int _startIndex = 0;
        private int _endIndex = 0;

        // tab & page 数据
        private object[] _data;
        public object[] data
        {
            set { _data = value; InvalidView(); }
        }

        // 首次刷新打开的默认tab索引，支持直接跳转到某个标签页面
        private int _tabId = 0;
        public int tabId
        {
            set { _tabId = value; }
        }

        // tab & page 缓存器
        private FRenderCache<UIToggle> _tabCache;
        private FRenderCache<GameObject> _pageCache;

        // tab & page 当前索引
        private int _currentTabIndex = -1;

        // render宽度
        private float _renderWidth = 0.0f;

        protected override void InitUI()
        {
            base.InitUI();

            //初始化对象池
            _tabCache = new FRenderCache<UIToggle>(tabTable, tabItemPrefab);
            _pageCache = new FRenderCache<GameObject>(pageTable, pageItemPrefab);

            //默认隐藏小箭头
            if (previous) previous.gameObject.SetActive(false);
            if (next) next.gameObject.SetActive(false);

            if (tabTable)
            {
                GameObject tabTableObj = tabTable.gameObject;

                _tabScrollView = tabTableObj.GetComponentInParent<UIScrollView>();
                if (_tabScrollView)
                {
                    _tabScrollView.onMomentumMove += OnTabDragMove;
                    _tabScrollView.onDragFinished += OnTabDragMove;
                }

                _tabScrollPanel = tabTableObj.GetComponentInParent<UIPanel>();
            }

            if (pageTable)
            {
                GameObject pageTableObj = pageTable.gameObject;
                _centerOnChild = pageTableObj.GetComponent<UICenterOnChild>();
                if (_centerOnChild) _centerOnChild.onCenter = PageOnCenter;
                _pageDragScrollView = pageTableObj.GetComponentInParent<UIDragScrollView>();
            }
        }

        protected override void UpdateView()
        {
            base.UpdateView();

            int length = _data != null ? _data.Length : 0;
            if (length <= 0)
            {
                _currentTabIndex = -1;
                ClearData();
                return;
            }

            for (int i = 0; i < length; i++)
            {
                UIToggle tabtoggle;
                GameObject pagerender;

                tabtoggle = _tabCache.PushRender(i);
                pagerender = _pageCache.PushRender(i);

                // 重置页面 Vertical ScrollView
                ResetScrollView(pagerender, true);

                // 配置标签Id和回调
                tabtoggle.group = _groupId;
                EventDelegate.Add(tabtoggle.onChange, TabOnChange);

                // 首次刷新，默认选中第一个标签
                if (i == 0)
                {
                    tabtoggle.startsActive = true;
                    UIWidget widget = tabtoggle.GetComponent<UIWidget>();
                    if (widget) _renderWidth = widget.width;
                    SetTargetScrollView(pagerender);
                }

                // 初始化 tab & page renders 数据
                TabPageRender tRender = tabtoggle.GetComponent<TabPageRender>();
                TabPageRender pRender = pagerender.GetComponent<TabPageRender>();
                tRender.data = _data[i];
                tRender.owner = this;
                pRender.data = _data[i];
                pRender.owner = this;
            }

            // 滚动到具体的标签页面，延迟0.5s
            if (_tabId > 0)
                FUtil.SetTimeout(gameObject, delegate () { MoveToTab(_tabId); }, 0.5f, "GoToTab");

            // 删除多余的标签和页面
            _tabCache.Release();
            _pageCache.Release();

            // 重置标签位置和小箭头状态
            ResetScrollView(_tabScrollView.gameObject, false);

            // 若标签数量未铺满标签面板，则禁止拖动
            if (_tabScrollView && length <= tabPanelSize)
                _tabScrollView.enabled = false;
            else
                _tabScrollView.enabled = true;

            // Table自适应
            tabTable.Reposition();
            pageTable.Reposition();
        }

        void MoveToTab(int index = -1)
        {
            _currentTabIndex = index;

            if (index >= 0)
                SetTab(index);
        }

        void ResetScrollView(GameObject scvObj, bool isVertical)
        {
            UIPanel panel = scvObj.GetComponent<UIPanel>();
            UIScrollView scrollview = scvObj.GetComponent<UIScrollView>();
            Transform trs = scvObj.transform;

            if (panel == null || scrollview == null)
                return;

            scrollview.ResetPosition();
            scrollview.RestrictWithinBounds(false, !isVertical, isVertical);

            panel.clipOffset = Vector3.zero;

            if (isVertical)
            {
                trs.localPosition = new Vector3(trs.localPosition.x, 0.0f, trs.localPosition.z);
            }
            else
            {
                trs.localPosition = new Vector3(0.0f, trs.localPosition.y, trs.localPosition.z);
                // 计算标签索引，以及是否显示标签小箭头
                UpdateTabsIndex();
                SetTabArrowStatus();
            }
        }

        void OnTabDragMove()
        {
            UpdateTabsIndex();
            SetTabArrowStatus();
        }

        void TabOnChange()
        {
            UIToggle toggle = UIToggle.current;
            if (toggle.value)
            {
                // 切换新页面，前一个页面复位
                if (_centerOnChild != null)
                {
                    GameObject lastpage = _centerOnChild.centeredObject;
                    if (lastpage != null)
                    {
                        SpringPanel spring = lastpage.GetComponent<SpringPanel>();
                        if (spring == null)
                            spring = lastpage.AddComponent<SpringPanel>();
                        spring.target = new Vector3(lastpage.transform.localPosition.x, 0.0f, lastpage.transform.localPosition.z);
                        spring.strength = 15.0f;
                        spring.enabled = true;
                    }
                }
                // 滚动到新标签
                for (int i = 0; i < _tabCache.size; i++)
                {
                    if (_tabCache.Render(i) == toggle)
                    {
                        _currentTabIndex = i;
                        break;
                    }
                }
                // 滚动到新页面
                GameObject page = _pageCache.Render(_currentTabIndex);
                if (_centerOnChild != null && page != _centerOnChild.centeredObject)
                {
                    _centerOnChild.CenterOn(page.transform);
                }
            }
        }

        void PageOnCenter(GameObject obj)
        {
            SetTargetScrollView(obj);
        }

        void SetTargetScrollView(GameObject obj)
        {
            // 页面静态拖拽碰撞体，位置恒定，尺寸恒等于Panel尺寸，不受动态render高度影响
            UIScrollView targetScv = obj.GetComponent<UIScrollView>();
            if (_pageDragScrollView && targetScv) _pageDragScrollView.scrollView = targetScv;
        }

        void OnTabFront()
        {
            int index = _currentTabIndex;
            if (index >= 0)
                index--;
            else
                return;

            if (index < 0)
                index = 0;

            SetTab(index);
        }

        void OnTabNext()
        {
            int index = _currentTabIndex;
            if (index >= 0)
                index++;
            else
                return;

            if (index > _tabCache.size - 1)
                index = _tabCache.size - 1;

            SetTab(index);
        }

        void SetTab(int index)
        {
            if (index < 0 || index > _tabCache.size - 1)
                return;

            SetTabArrowStatus();
            ScrollToTab(index);

            UIToggle toggle = _tabCache.Render(index);
            if (!toggle.value)
            {
                toggle.value = true;
                _currentTabIndex = index;
            }
        }

        void ScrollToTab(int index)
        {
            if (_tabScrollPanel == null || !_tabScrollView.enabled || _renderWidth <= 0.0f || _tabCache.size <= 0)
                return;

            UpdateTabsIndex();

            // 滚动ScrollView，scrollWheelFactor = 0.1f
            if (index > _endIndex)
                _tabScrollView.Scroll((index - _endIndex));
            else if (index < _startIndex)
                _tabScrollView.Scroll(-(_startIndex - index));
            else
            {
                if (index == 0)
                    _tabScrollView.Scroll(-1.0f);
                else if (index == _tabCache.size - 1)
                    _tabScrollView.Scroll(1.0f);
            }
        }

        void SetTabArrowStatus()
        {
            previous.gameObject.SetActive(_startIndex > 0);
            next.gameObject.SetActive(_endIndex < _tabCache.size - 1 && _tabCache.size > tabPanelSize);
        }

        void UpdateTabsIndex()
        {
            // 计算当前未被遮挡的标签，起止render索引
            int startIndex = (int)((_tabScrollPanel.finalClipRegion.x + (_renderWidth / 2.0f)) / _renderWidth);
            if (startIndex > _tabCache.size - tabPanelSize)
                startIndex = _tabCache.size - tabPanelSize;
            else if (startIndex < 0)
                startIndex = 0;

            int endIndex = (startIndex + tabPanelSize - 1);
            if (endIndex > _tabCache.size - 1)
                endIndex = _tabCache.size - 1;
            else if (endIndex < 0)
                endIndex = 0;

            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public virtual void TriggerRenderEvent(string name, object data)
        {
            if (renderEvent != null) renderEvent(name, data);
        }

        public int GetGroupId()
        {
            return ++_groupId;
        }

        void ClearData()
        {
            _data = null;
            if (_tabCache != null)
                _tabCache.Clear();
            if (_pageCache != null)
                _pageCache.Clear();
        }

        protected override void OnDestroy()
        {
            ClearData();
            FUtil.ClearTimeout(gameObject, "GoToTab");
            base.OnDestroy();
        }
    }
}
