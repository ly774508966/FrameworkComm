using UnityEngine;

namespace Framework
{
    public class FScreenshot
    {
        /// <summary>
        /// 对相机截图
        /// </summary>
        public static Texture2D Screenshot(Camera camera)
        {
            Camera camShot = camera;
            if (camShot == null)
                camShot = Camera.main;

            int w = Screen.width;
            int h = Screen.height;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            return Screenshot(new Camera[1] { camShot }, w, h, rect);
        }

        /// <summary>  
        /// 对相机指定区域截图
        /// </summary>  
        public static Texture2D Screenshot(Camera camera, int w, int h, Rect rect)
        {
            Camera camShot = camera;
            if (camShot == null)
                camShot = Camera.main;

            return Screenshot(new Camera[1] { camShot }, w, h, rect);
        }

        public static Texture2D Screenshot(Camera[] cameras, int w, int h, Rect rect)
        {
            // 创建一个RenderTexture对象
            RenderTexture rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                camera.targetTexture = rt;
                camera.Render();
            }
            // 激活这个rt, 并从中中读取像素
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(rect, 0, 0);  // 注：从RenderTexture.active中读取像素
            screenShot.Apply();
            // 重置相关参数，以使用camera继续在屏幕上显示
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                camera.targetTexture = null;
            }
            RenderTexture.active = null;
            GameObject.DestroyImmediate(rt);
            return screenShot;
        }
    }
}
