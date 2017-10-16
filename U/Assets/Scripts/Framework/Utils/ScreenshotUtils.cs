using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public static class ScreenshotUtils
    {
        public static Texture2D Screenshot(Camera camera, TextureFormat textureFormat)
        {
            Camera camShot = camera;
            if (camShot == null)
                camShot = Camera.main;

            int w = Screen.width;
            int h = Screen.height;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            return Screenshot(new Camera[1] { camShot }, w, h, rect, textureFormat);
        }

        public static Texture2D Screenshot(Camera[] cameras, TextureFormat textureFormat)
        {
            int w = Screen.width;
            int h = Screen.height;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            return Screenshot(cameras, w, h, rect, textureFormat);
        }

        public static Texture2D Screenshot(Camera camera, int w, int h, Rect rect, TextureFormat textureFormat)
        {
            Camera camShot = camera;
            if (camShot == null)
                camShot = Camera.main;

            return Screenshot(new Camera[1] { camShot }, w, h, rect, textureFormat);
        }

        public static Texture2D Screenshot(Camera[] cameras, int w, int h, Rect rect, TextureFormat textureFormat)
        {
            RenderTexture rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);

            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera != null)
                {
                    camera.targetTexture = rt;
                    camera.Render();
                }
            }

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, textureFormat, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera != null)
                {
                    camera.targetTexture = null;
                }
            }

            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            return screenShot;
        }
    }
}
