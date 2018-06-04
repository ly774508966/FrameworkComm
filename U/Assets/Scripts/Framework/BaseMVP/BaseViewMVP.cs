using UnityEngine;

/// <summary>
/// UI组件基类，提供延时刷新的功能
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class BaseViewMVP : MonoBehaviour
    {
        protected bool bWaitToUpdate = true;

        /// <summary>
        /// UI属性变更时可调用该方法，使界面无效，下一帧会调用UpdateView刷新
        /// </summary>
        public void InvalidView()
        {
            bWaitToUpdate = true;
        }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        /// <summary>
        /// UI延时刷新
        /// </summary>
        protected virtual void UpdateView()
        {
            bWaitToUpdate = false;
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
    }
}
