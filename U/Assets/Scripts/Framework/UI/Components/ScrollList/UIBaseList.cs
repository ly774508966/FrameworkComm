using UnityEngine;

/// <summary>
/// Base list, and cell script need to derive from UIBaseCell
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

                OnDataChanged();

                if (_datas == null)
                {
                    UpdateView();
                }
                else
                {
                    InvalidView();
                }
            }
        }

        protected override void UpdateView()
        {
            RemoveItems();
            AddItems();
        }

        protected virtual void OnDataChanged() { }

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
                UIBaseCell baseCell = toRemove.GetComponent<UIBaseCell>();
                if (baseCell != null)
                {
                    baseCell.data = null;
                    baseCell.index = 0;
                }
                objectPool.ReturnObject(toRemove);
            }
        }
    }
}
