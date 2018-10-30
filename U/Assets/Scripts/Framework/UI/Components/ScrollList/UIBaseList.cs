using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base list, and cell script need to be derived from UIBaseCell
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class UIBaseList : BaseViewMVP
    {
        public UISimpleObjectPool objectPool;

        public Transform contentPanel;

        protected object[] _datas;

        protected List<UIBaseCell> _cells = new List<UIBaseCell>();

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
                InvalidView();
            }
        }

        public void Refresh()
        {
            int length = _datas != null ? _datas.Length : 0;
            for (int i = 0; i < length; i++)
            {
                _cells[i].InvalidView();
            }
        }

        public void RefreshCell(int index, object data)
        {
            if (index >= 0 && index < _cells.Count)
            {
                _cells[index].data = data;
                _cells[index].index = index;
            }
        }

        public void RefreshCell(int index)
        {
            if (index >= 0 && index < _cells.Count)
            {
                _cells[index].InvalidView();
            }
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            RemoveCells();
            AddCells();
        }

        protected virtual void OnDataChanged() { }

        void AddCells()
        {
            int length = _datas != null ? _datas.Length : 0;
            for (int i = 0; i < length; i++)
            {
                GameObject cellGo = objectPool.GetObject();
                cellGo.transform.SetParent(contentPanel, false);

                UIBaseCell baseCell = cellGo.GetComponent<UIBaseCell>();
                if (baseCell != null)
                {
                    baseCell.data = _datas[i];
                    baseCell.index = i;
                }

                _cells.Add(baseCell);
            }
        }

        void RemoveCells()
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

            _cells.Clear();
        }
    }
}
