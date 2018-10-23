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

        public GameObject PopUp(string prefabPath, bool modal, bool blur = true, float alpha = MaskAlpha)
        {
            return _PopUp(prefabPath, (GameObject)Resources.Load(prefabPath), modal, blur, alpha);
        }

        public T PopUp<T>(string prefabPath, bool modal, bool blur = true, float alpha = MaskAlpha)
        {
            GameObject popObject = PopUp(prefabPath, modal, blur, alpha);
            return popObject.GetComponent<T>();
        }

        public void PopUpAsync(string prefabPath, bool modal, bool blur = true, float alpha = MaskAlpha)
        {
            StartCoroutine(_PopUpAsync(prefabPath, modal, blur, alpha));
        }

        public void PopDown(string prefabPath)
        {
            UIPopContainer container = _FindContainer(prefabPath);

            if (container != null)
            {
                container.RemoveContainer();
            }
        }

        private GameObject _PopUp(string prefabPath, GameObject prefab, bool modal, bool maskBlur, float maskAlpha)
        {
            UIPopContainer container = _FindContainer(prefabPath);

            if (container != null)
            {
                container.SetTop();

                return container.child;
            }

            container = _CreateContainer(prefabPath);
            container.Path = prefabPath;
            container.Modal = modal;
            container.DestroyDelegate = _OnDestroyContainer;

            if (maskBlur)
            {
                container.SetMaskBlur();
            }
            else
            {
                container.SetMaskAlpha(maskAlpha);
            }

            _popContainerDict.Add(prefabPath, container);

            return container.AddChild(prefab);
        }

        private IEnumerator _PopUpAsync(string prefabPath, bool modal, bool maskBlur, float maskAlpha)
        {
            ResourceRequest request = Resources.LoadAsync(prefabPath);
            yield return request;
            _PopUp(prefabPath, request.asset as GameObject, modal, maskBlur, maskAlpha);
        }

        private UIPopContainer _CreateContainer(string prefabPath)
        {
            GameObject popObject = UnityUtils.AddChild(popCanvas.gameObject);
            popObject.name = prefabPath;
            return popObject.AddComponent<UIPopContainer>();
        }

        private UIPopContainer _FindContainer(string prefabPath)
        {
            if (_popContainerDict.ContainsKey(prefabPath))
                return _popContainerDict[prefabPath];
            return null;
        }

        private void _OnDestroyContainer(string prefabPath)
        {
            if (_popContainerDict.ContainsKey(prefabPath))
                _popContainerDict.Remove(prefabPath);
        }
    }
}