using UnityEngine;

/// <summary>
/// UI组件基类，提供延时刷新的功能
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class BaseView : MonoBehaviour
    {
        protected bool bWaitToUpdate = true;

        /// <summary>
        /// 界面的属性变更时可调用该方法，使界面无效，下一帧会调用UpdateView刷新，提升性能
        /// </summary>
        public void InvalidView()
        {
            bWaitToUpdate = true;
        }

        protected virtual void Awake() { }

        void Start()
        {
            InitView();
        }

        protected virtual void FixedUpdate()
        {
            if (bWaitToUpdate)
            {
                bWaitToUpdate = false;
                UpdateView();
                bWaitToUpdate = false;
            }
        }

        protected virtual void Update()
        {
            if (bWaitToUpdate)
            {
                bWaitToUpdate = false;
                UpdateView();
                bWaitToUpdate = false;
            }
        }

        protected virtual void LateUpdate()
        {
            if (bWaitToUpdate)
            {
                bWaitToUpdate = false;
                UpdateView();
                bWaitToUpdate = false;
            }
        }

        protected virtual void OnDestroy() { }

        /// <summary>
        /// View初始化
        /// </summary>
        protected virtual void InitView() { }

        /// <summary>
        /// 界面延时刷新，提升性能
        /// </summary>
        protected virtual void UpdateView()
        {
            bWaitToUpdate = false;
        }
    }
}
