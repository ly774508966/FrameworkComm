using UnityEngine;
using System.Collections;
using System.Text;
using Framework;

namespace TikiAL
{
    public class LogoScene : BaseSceneFadeInOut
    {
        private bool _resInitialized = false;

        protected override void InitUI()
        {
            base.InitUI();
        }

        protected override void OnSceneEaseInFinish()
        {
            base.OnSceneEaseInFinish();
            DebugSystemInfo();
            StartLoadRes();
            StartCoroutine(GoToMainScene());
        }

        private void StartLoadRes()
        {
            ResourceManager.instance.LoadResourcesAsyn(PathConfig.Gift, delegate ()
            {
                ResourceManager.instance.LoadResourcesAsyn(PathConfig.Guest, delegate ()
                {
                    _resInitialized = true;
                });
            });
        }

        private IEnumerator GoToMainScene()
        {
            Log.Debug("Check() start.");

            while (true)
            {
                if (_resInitialized
                    && GiftModel.instance.IsInitialized
                    && GuestModel.instance.IsInitialized
                    && LotteryModel.instance.IsInitialized)
                {
                    Log.Debug("Check() end, all Models has already initialized.");
                    break;
                }

                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(1.0f);

            GotoScene(SceneName.MainScene);
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