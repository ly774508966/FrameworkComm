using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public sealed class UIPopManager : MonoSingleton<UIPopManager>
    {
        public Camera popCamera { get; private set; }
        public Canvas popCanvas { get; private set; }

        private const string LayerName = "UIPop";
        private const int LayerDepth = 1;
        private const int CanvasOrder = 1;
        private const float MaskAlpha = 0.75f;

        private Dictionary<string, UIPopContainer> _popPath2ContainerDict = new Dictionary<string, UIPopContainer>();
        private List<string> _popPathList = new List<string>();

        private void Awake()
        {
            GameObject cameraObject = new GameObject("UIPopCamera");

            DontDestroyOnLoad(cameraObject);

            UnityUtils.SetLayer(cameraObject, LayerMask.NameToLayer(LayerName));

            popCamera = cameraObject.AddComponent<Camera>();
            popCamera.cullingMask &= 1 << LayerMask.NameToLayer(LayerName);
            popCamera.clearFlags = CameraClearFlags.Depth;
            popCamera.orthographic = true;
            popCamera.depth = LayerDepth;

            cameraObject.AddComponent<FlareLayer>();
            cameraObject.AddComponent<GUILayer>();

            GameObject canvasObject = UnityUtils.AddChild(cameraObject);
            canvasObject.name = "Canvas";

            popCanvas = canvasObject.AddComponent<Canvas>();
            popCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            popCanvas.worldCamera = popCamera;
            popCanvas.sortingOrder = CanvasOrder;

            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = FrameworkConfig.UI.ScalerResolution;
            canvasScaler.matchWidthOrHeight = FrameworkConfig.UI.MatchWidthOrHeight;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        public GameObject PopUp(string path, bool modal, bool blur, float alpha = MaskAlpha)
        {
            return _PopUp(path, (GameObject)Resources.Load(path), modal, blur, alpha);
        }

        public T PopUp<T>(string path, bool modal, bool blur, float alpha = MaskAlpha)
        {
            GameObject popObject = PopUp(path, modal, blur, alpha);
            return popObject.GetComponent<T>();
        }

        public void PopUpAsync(string path, bool modal, bool blur, float alpha = MaskAlpha)
        {
            StartCoroutine(_PopUpAsync(path, modal, blur, alpha));
        }

        public void PopBack()
        {
            int popCount = _popPathList.Count;
            if (popCount == 0)
            {
                return;
            }

            UIPopContainer container;

            if (_popPath2ContainerDict.TryGetValue(_popPathList[popCount - 1], out container))
            {
                container.DestroyContainer();
            }
        }

        private GameObject _PopUp(string path, GameObject prefab, bool modal, bool blur, float alpha)
        {
            if (prefab == null)
            {
                Log.Error("Can not load prefab from path ", path);
                return null;
            }

            UIPopContainer container;

            if (_popPath2ContainerDict.TryGetValue(path, out container))
            {
                _UnCache(path);
                _Cache(path, container);

                container.SetTop();

                return container.child;
            }

            container = _CreateContainer(path);

            if (blur)
            {
                container.SetBlur();
            }
            else
            {
                container.SetAlpha(alpha);
            }

            container.SetModal(modal);
            container.DestroyDelegate = () =>
            {
                _UnCache(path);
            };

            _Cache(path, container);

            return container.AddChild(prefab);
        }

        private IEnumerator _PopUpAsync(string path, bool modal, bool blur, float alpha)
        {
            ResourceRequest request = Resources.LoadAsync(path);
            yield return request;
            _PopUp(path, request.asset as GameObject, modal, blur, alpha);
        }

        private UIPopContainer _CreateContainer(string path)
        {
            GameObject popObject = UnityUtils.AddChild(popCanvas.gameObject);
            popObject.name = path;
            return popObject.AddComponent<UIPopContainer>();
        }

        private void _Cache(string path, UIPopContainer container)
        {
            if (!_popPath2ContainerDict.ContainsKey(path))
            {
                _popPath2ContainerDict.Add(path, container);
                _popPathList.Add(path);
            }
        }

        private void _UnCache(string path)
        {
            if (_popPath2ContainerDict.ContainsKey(path))
            {
                _popPath2ContainerDict.Remove(path);
                _popPathList.Remove(path);
            }
        }
    }
}