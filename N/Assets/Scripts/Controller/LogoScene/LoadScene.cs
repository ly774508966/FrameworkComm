﻿using UnityEngine;
using System.Collections;
using System.Text;
using Framework;

namespace Project
{
    public class LoadScene : BaseSceneFadeInOut
    {
        private bool _ready = false;

        protected override void InitUI()
        {
            base.InitUI();
            InstantiateManagers();
        }

        protected override void OnSceneEaseInFinish()
        {
            base.OnSceneEaseInFinish();
            DebugSystemInfo();
            StartCoroutine(Check());
        }

        private void InstantiateManagers()
        {
            GameManager.instance.enabled = true;
        }

        private IEnumerator Check()
        {
            FLog.Debug("Check() start.");

            while (true)
            {
                if (_ready)
                {
                    FLog.Debug("Check() end, game is ready.");
                    break;
                }

                _ready = true;

                yield return new WaitForSeconds(1.0f);
            }

            yield return new WaitForEndOfFrame();

            //GotoScene(SceneName.MainScene);
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

            FLog.Debug(sb.ToString());
        }
        #endregion
    }
}