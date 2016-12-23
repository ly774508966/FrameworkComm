using UnityEngine;

namespace Framework
{
    /// <summary>
    /// UI组件基类，提供延时刷新功能
    /// </summary>
    public class BaseController : MonoBehaviour
    {
        protected bool isWaitToUpdate = false;

        public void InvalidView()
        {
            isWaitToUpdate = true;
        }

        public void Refresh()
        {
            UpdateView();
            isWaitToUpdate = false;
        }

        protected virtual void Awake()
        {
            InitData();
        }

        protected virtual void Start()
        {
            InitUI();
        }

        protected virtual void FixedUpdate()
        {
            if (isWaitToUpdate)
            {
                isWaitToUpdate = false;
                UpdateView();
                isWaitToUpdate = false;
            }
        }

        protected virtual void Update()
        {
            if (isWaitToUpdate)
            {
                isWaitToUpdate = false;
                UpdateView();
                isWaitToUpdate = false;
            }
        }

        protected virtual void LateUpdate()
        {
            if (isWaitToUpdate)
            {
                isWaitToUpdate = false;
                UpdateView();
                isWaitToUpdate = false;
            }
        }

        protected virtual void OnDestroy() { }

        protected virtual void InitData() { }

        protected virtual void InitUI() { }

        protected virtual void UpdateView() { }
    }
}