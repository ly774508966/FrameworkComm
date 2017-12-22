using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    public class FPSDisplay : MonoBehaviour
    {
        float deltaTime = 0.0f;

        GUIStyle style;
        Rect rect;

        void Start()
        {
            rect = new Rect(0, 0, Screen.width, Screen.height * 2 / 100);

            style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = Screen.height * 2 / 75;
            style.normal.textColor = new Color(0f, 1f, 0f, 1f);
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            GUI.Label(rect, string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps), style);
        }
    }
}