using UnityEngine;
using System.Collections;
using Framework;

namespace TikiAL
{
    public class BaseSceneFadeInOut : BaseSceneEaseInOut
    {
        private GameObject _maskGo;
        private float _alphaLight = 0.1f;
        private float _alphaDark = 1.0f;

        protected override void InitUI()
        {
            base.InitUI();
            _maskGo = CreateMask();
        }

        protected override void SceneEaseIn(Callback.FunVoid fCallback)
        {
            TweenMask(_alphaDark, _alphaLight, fCallback);
        }

        protected override void SceneEaseOut(Callback.FunVoid fCallback)
        {
            TweenMask(_alphaLight, _alphaDark, fCallback);
        }

        private GameObject CreateMask()
        {
            GameObject root = Util.GetRoot();
            if (root == null) return null;

            //Create Panel
            GameObject maskPanel = NGUITools.AddChild(root, true);
            maskPanel.name = "MaskPanel";
            UIPanel panel = maskPanel.AddComponent<UIPanel>();
            panel.depth = GameConfig.MaxDepth;
            panel.sortingOrder = GameConfig.MaxSortingOrder;

            //Create Mask GameObject
            GameObject maskGo = NGUITools.AddChild(maskPanel, true);
            maskGo.name = "Mask";

            //Create UI2DSprite
            UI2DSprite tx = maskGo.AddComponent<UI2DSprite>();
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();
            tx.sprite2D = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            tx.type = UIBasicSprite.Type.Sliced;
            tx.alpha = 0.0f;
            UIWidget widget = tx;

            //Create BoxCollider and Widget Size
            BoxCollider box = maskGo.AddComponent<BoxCollider>();
            box.isTrigger = true;
            widget.autoResizeBoxCollider = true;
            Vector2 size = Util.GetCurrentScreenSize();
            widget.width = (int)size.x;
            widget.height = (int)size.y;

            return maskGo;
        }

        private void TweenMask(float from, float to, Callback.FunVoid fCallback, float time = 1.0f)
        {
            if (_maskGo == null)
            {
                fCallback();
                return;
            }

            _maskGo.SetActive(true);

            BoxCollider box = _maskGo.GetComponent<BoxCollider>();
            if (box != null) box.enabled = true;

            UIWidget widget = _maskGo.GetComponent<UIWidget>();
            if (widget == null)
            {
                fCallback();
                return;
            }

            widget.alpha = from;
            iTween.ValueTo(_maskGo, iTween.Hash(
                "from", from,
                "to", to,
                "delay", .1f,
                "time", time,
                "easetype", iTween.EaseType.linear,
                "onupdate", Callback.CreateAction(delegate (object x)
                {
                    float value = Mathf.Clamp01((float)x);
                    widget.alpha = value;
                }),
                "onupdatetarget", gameObject,
                "oncomplete", Callback.CreateAction(delegate ()
                {
                    if (box != null) box.enabled = false;
                    fCallback();
                }),
                "oncompletetarget", gameObject));
        }
    }
}