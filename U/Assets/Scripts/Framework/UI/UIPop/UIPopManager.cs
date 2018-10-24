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

        private const float MaskAlpha = 0.75f;

        private Dictionary<string, UIPopContainer> _popContainerDict = new Dictionary<string, UIPopContainer>();

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

            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = FrameworkConfig.UI.ScalerResolution;
            canvasScaler.matchWidthOrHeight = FrameworkConfig.UI.MatchWidthOrHeight;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        public GameObject PopUp(string path, bool modal, bool blur = true, float alpha = MaskAlpha)
        {
            return _PopUp(path, (GameObject)Resources.Load(path), modal, blur, alpha);
        }

        public T PopUp<T>(string path, bool modal, bool blur = true, float alpha = MaskAlpha)
        {
            GameObject popObject = PopUp(path, modal, blur, alpha);
            return popObject.GetComponent<T>();
        }

        public void PopUpAsync(string path, bool modal, bool blur = true, float alpha = MaskAlpha)
        {
            StartCoroutine(_PopUpAsync(path, modal, blur, alpha));
        }

        public void PopDown(string path)
        {
            UIPopContainer container = _FindContainer(path);

            if (container != null)
            {
                container.RemoveContainer();
            }
        }

        private GameObject _PopUp(string path, GameObject prefab, bool modal, bool blur, float alpha)
        {
            UIPopContainer container = _FindContainer(path);

            if (container != null)
            {
                container.SetTop();

                return container.child;
            }

            container = _CreateContainer(path);
            container.DestroyDelegate = () =>
            {
                _RemoveContainer(path);
            };

            container.SetModal(modal);

            if (blur)
            {
                container.SetMaskBlur();
            }
            else
            {
                container.SetMaskAlpha(alpha);
            }

            _popContainerDict.Add(path, container);

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

        private UIPopContainer _FindContainer(string path)
        {
            if (_popContainerDict.ContainsKey(path))
            {
                return _popContainerDict[path];
            }
            return null;
        }

        private void _RemoveContainer(string path)
        {
            if (_popContainerDict.ContainsKey(path))
            {
                _popContainerDict.Remove(path);
            }
        }
    }
}