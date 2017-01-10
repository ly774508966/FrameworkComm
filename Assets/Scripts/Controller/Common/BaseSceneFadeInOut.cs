using UnityEngine;
using Framework;

namespace Project
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

        protected override void SceneEaseIn(FCallback.FunVoid fCallback)
        {
            TweenMask(_alphaDark, _alphaLight, fCallback);
        }

        protected override void SceneEaseOut(FCallback.FunVoid fCallback)
        {
            TweenMask(_alphaLight, _alphaDark, fCallback);
        }

        private GameObject CreateMask()
        {
            GameObject root = FUtil.GetRoot();
            if (root == null) return null;

            //Create Panel
            GameObject maskPanel = NGUITools.AddChild(root, true);
            maskPanel.name = "MaskPanel";
            UIPanel panel = maskPanel.AddComponent<UIPanel>();
            panel.depth = GameConfig.MaxDepth;
            panel.sortingOrder = GameConfig.MaxSortingOrder;

            return PopupManager.instance.CreateMask(maskPanel, 0.0f, "Mask");
        }

        private void TweenMask(float from, float to, FCallback.FunVoid fCallback, float time = 1.0f)
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
                "onupdate", FCallback.CreateAction(delegate (object x)
                {
                    float value = Mathf.Clamp01((float)x);
                    widget.alpha = value;
                }),
                "onupdatetarget", gameObject,
                "oncomplete", FCallback.CreateAction(delegate ()
                {
                    if (box != null) box.enabled = false;
                    fCallback();
                }),
                "oncompletetarget", gameObject));
        }
    }
}