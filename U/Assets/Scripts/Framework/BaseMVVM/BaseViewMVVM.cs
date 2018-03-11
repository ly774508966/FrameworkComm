using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public abstract class BaseViewMVVM<T> : MonoBehaviour, IView<T> where T : BaseViewModel, new()
    {
        public bool autoBindViewModel = false;

        private bool _viewModelbinded = false;
        private T _viewModel;

        public T viewModel { get { return _viewModel; } }

        public void Bind(T viewModel)
        {
            if (!Equals(_viewModel, viewModel))
            {
                _viewModel = viewModel;

                if (!_viewModelbinded)
                {
                    _viewModelbinded = true;
                    OnListenViewModel();
                }

                _viewModel.OnInit();
            }
        }

        public void UnBind()
        {
            if (_viewModel != null)
            {
                _viewModel.OnDestroy();
                _viewModel = null;
            }
        }

        private void Awake()
        {
            OnPreInit();
        }

        private void Start()
        {
            OnInit();

            if (autoBindViewModel)
            {
                Bind(new T());
            }
        }

        private void OnDestroy()
        {
            UnBind();
            OnClear();
        }

        protected virtual void OnPreInit() { }

        protected virtual void OnInit() { }

        protected virtual void OnClear() { }

        protected virtual void OnListenViewModel() { }
    }
}