using UnityEngine;

/// <summary>
/// 基本列表，Cell脚本需派生自UIBaseCell
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIBaseList : BaseView
    {
        public UISimpleObjectPool objectPool;

        public Transform contentPanel;

        protected object[] _datas;

        public object[] datas
        {
            get
            {
                return _datas;
            }
            set
            {
                _datas = value;
                InvalidView();
            }
        }

        protected override void UpdateView()
        {
            RemoveItems();
            AddItems();
        }

        void AddItems()
        {
            int length = _datas != null ? _datas.Length : 0;
            for (int i = 0; i < length; i++)
            {
                object itemData = _datas[i];
                GameObject itemGo = objectPool.GetObject();
                itemGo.transform.SetParent(contentPanel, false);

                UIBaseCell baseItem = itemGo.GetComponent<UIBaseCell>();
                baseItem.data = itemData;
                baseItem.index = i;
            }
        }

        void RemoveItems()
        {
            while (contentPanel.childCount > 0)
            {
                GameObject toRemove = contentPanel.transform.GetChild(0).gameObject;
                objectPool.ReturnObject(toRemove);
            }
        }
    }
}
