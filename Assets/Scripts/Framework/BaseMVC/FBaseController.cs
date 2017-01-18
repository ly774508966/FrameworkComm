using UnityEngine;

namespace Framework
{
    /// <summary>
    /// UI组件基类，延时刷新
    /// </summary>
    public class FBaseController : MonoBehaviour
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

        protected virtual void OnEnable()
        {
            InvalidView();
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