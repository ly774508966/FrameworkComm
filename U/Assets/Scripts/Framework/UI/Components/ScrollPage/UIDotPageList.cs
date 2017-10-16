using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIDotPageList : UIScrollPage
    {
        public UISimpleObjectPool pageObjectPool;
        public UISimpleObjectPool dotObjectPool;
        public Transform dotContent;
        private ToggleGroup _dotToggleGroup;

        private object[] _datas;
        public object[] datas
        {
            set { _datas = value; InvalidView(); }
        }

        public int dataLength
        {
            get { return _datas != null ? _datas.Length : 0; }
        }

        protected override void InitView()
        {
            base.InitView();

            _dotToggleGroup = dotContent.GetComponent<ToggleGroup>();
            _dotToggleGroup.allowSwitchOff = false;
        }

        protected override void UpdateView()
        {
            RemoveItems();
            AddItems();
        }

        void RemoveItems()
        {
            while (currentPageCount > 0)
            {
                GameObject toRemove = pageContent.GetChild(0).gameObject;
                pageObjectPool.ReturnObject(toRemove);
            }

            while (currentPageCount > 0)
            {
                GameObject toRemove = dotContent.GetChild(0).gameObject;
                dotObjectPool.ReturnObject(toRemove);
            }
        }

        void AddItems()
        {
            int length = dataLength;
            for (int i = 0; i < length; i++)
            {
                GameObject pageGo = pageObjectPool.GetObject();
                pageGo.name = i.ToString();
                pageGo.transform.SetParent(pageContent, false);
                GameObject dotGo = dotObjectPool.GetObject();
                dotGo.name = i.ToString();
                dotGo.transform.SetParent(dotContent, false);

                Toggle dotToggle = dotGo.GetComponent<Toggle>();
                if (dotToggle != null)
                {
                    dotToggle.group = _dotToggleGroup;
                    dotToggle.isOn = i == 0 ? true : false;
                }

                UIBaseCell pageCell = pageGo.GetComponent<UIBaseCell>();
                pageCell.data = _datas[i];
                pageCell.index = i;
            }

            dotContent.gameObject.SetActive(length > 1);
            pageScrollRect.enabled = length > 1;
            pageScrollRect.content.anchoredPosition = Vector2.zero;
        }

        void SetDotToggle(int index)
        {
            if (index >= 0 && index < dotContent.childCount)
            {
                dotContent.GetChild(index).GetComponent<Toggle>().isOn = true;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            SetDotToggle(currentPageIndex);
        }
    }
}
