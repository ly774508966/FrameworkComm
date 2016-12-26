using UnityEngine;
using System.Collections;
using System.Text;
using Framework;

namespace TikiAL
{
    public class LogoScene : BaseSceneFadeInOut
    {
        public GameObject buttonGo;
        public UIEventListener restartBtn;
        public UIEventListener continueBtn;
        public UIEventListener quitBtn;
        public GameObject loadingGo;

        private bool _ready = false;

        protected override void InitUI()
        {
            base.InitUI();

            restartBtn.onClick = OnClickRestart;
            continueBtn.onClick = OnClickContinue;
            quitBtn.onClick = OnClickQuit;

            SwitchButton();
        }

        private void OnClickRestart(GameObject go)
        {
            LotteryModel.instance.ResetLottery();
            GotoScene(SceneName.MainScene);
        }

        private void OnClickContinue(GameObject go)
        {
            LotteryModel.instance.SetLotteryFromHistory();
            GotoScene(SceneName.MainScene);
        }

        private void OnClickQuit(GameObject go)
        {
            BackToLastScene();
        }

        private void SwitchButton()
        {
            buttonGo.SetActive(_ready);
            loadingGo.SetActive(!_ready);
        }

        protected override void OnSceneEaseInFinish()
        {
            base.OnSceneEaseInFinish();
            DebugSystemInfo();
            StartLoadRes();
            StartCoroutine(CheckReady());
        }

        private void StartLoadRes()
        {
            ResourceManager.instance.LoadResourcesAsyn(PathConfig.Gift, delegate ()
            {
                ResourceManager.instance.LoadResourcesAsyn(PathConfig.Guest, delegate ()
                {
                    _ready = true;
                });
            });
        }

        private IEnumerator CheckReady()
        {
            Log.Debug("Check() start.");

            while (true)
            {
                if (_ready
                    && GiftModel.instance.ready
                    && GuestModel.instance.ready
                    && LotteryModel.instance.ready)
                {
                    Log.Debug("Check() end, game is ready.");
                    break;
                }

                yield return new WaitForSeconds(1.0f);
            }

            yield return new WaitForSeconds(2.0f);

            SwitchButton();
        }

        #region DebugSystemInfo
        private void DebugSystemInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\n-------------------------SystemInfo-------------------------");
            sb.AppendLine("DeviceModel: " + SystemInfo.deviceModel);
            sb.AppendLine("DeviceName: " + SystemInfo.deviceName);
            sb.AppendLine("DeviceType: " + SystemInfo.deviceType.ToString());
            sb.AppendLine("DeviceUniqueIdentifier: " + SystemInfo.deviceUniqueIdentifier);
            sb.AppendLine("GraphicsDeviceID: " + SystemInfo.graphicsDeviceID.ToString());
            sb.AppendLine("GraphicsDeviceName: " + SystemInfo.graphicsDeviceName);
            sb.AppendLine("GraphicsDeviceType: " + SystemInfo.graphicsDeviceType.ToString());
            sb.AppendLine("GraphicsDeviceVendor: " + SystemInfo.graphicsDeviceVendor);
            sb.AppendLine("GraphicsDeviceVendorID: " + SystemInfo.graphicsDeviceVendorID.ToString());
            sb.AppendLine("GraphicsDeviceVersion: " + SystemInfo.graphicsDeviceVersion);
            sb.AppendLine("GraphicsMemorySize: " + SystemInfo.graphicsMemorySize.ToString());
            sb.AppendLine("GraphicsMultiThreaded: " + SystemInfo.graphicsMultiThreaded.ToString());
            sb.AppendLine("GraphicsShaderLevel: " + SystemInfo.graphicsShaderLevel.ToString());
            sb.AppendLine("MaxTextureSize: " + SystemInfo.maxTextureSize.ToString());
            sb.AppendLine("NpotSupport: " + SystemInfo.npotSupport.ToString());
            sb.AppendLine("OperatingSystem: " + SystemInfo.operatingSystem);
            sb.AppendLine("ProcessorCount: " + SystemInfo.processorCount.ToString());
            sb.AppendLine("ProcessorType: " + SystemInfo.processorType);
            sb.AppendLine("SupportedRenderTargetCount: " + SystemInfo.supportedRenderTargetCount.ToString());
            sb.AppendLine("Supports3DTextures: " + SystemInfo.supports3DTextures.ToString());
            sb.AppendLine("SupportsAccelerometer: " + SystemInfo.supportsAccelerometer.ToString());
            sb.AppendLine("SupportsComputeShaders: " + SystemInfo.supportsComputeShaders.ToString());
            sb.AppendLine("SupportsGyroscope: " + SystemInfo.supportsGyroscope.ToString());
            sb.AppendLine("SupportsImageEffects: " + SystemInfo.supportsImageEffects.ToString());
            sb.AppendLine("SupportsInstancing: " + SystemInfo.supportsInstancing.ToString());
            sb.AppendLine("SupportsLocationService: " + SystemInfo.supportsLocationService.ToString());
            sb.AppendLine("SupportsRenderTextures: " + SystemInfo.supportsRenderTextures.ToString());
            sb.AppendLine("SupportsRenderToCubemap: " + SystemInfo.supportsRenderToCubemap.ToString());
            sb.AppendLine("SupportsShadows: " + SystemInfo.supportsShadows.ToString());
            sb.AppendLine("SupportsSparseTextures: " + SystemInfo.supportsSparseTextures.ToString());
            sb.AppendLine("SupportsStencil: " + SystemInfo.supportsStencil.ToString());
            sb.AppendLine("SupportsVibration: " + SystemInfo.supportsVibration.ToString());
            sb.AppendLine("SystemMemorySize: " + SystemInfo.systemMemorySize.ToString());
            sb.AppendLine("-------------------------SystemInfo-------------------------");

            Log.Debug(sb.ToString());
        }
        #endregion
    }
}