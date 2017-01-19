using UnityEngine;

namespace Framework
{
    public class PopupManager : FSingleton<PopupManager>
    {
        private const float MASK_ALPHA = 0.3f;
        private const string LAYER_NAME = "PopupLayer";
        private const string POPUPPANEL_NAME = "PopupPanel";
        private const string MASK_NAME = "PanelMask";

        private int BASEPOPUPPANEL_DEPTH = 100;
        private int BASEPOPUPPANEL_SORTINGORDER = 100;

        public GameObject AddPopUp(string prefabName, bool mode = true, string id = null, bool isBringForward = true, float maskAlpha = MASK_ALPHA, bool canRemove = true)
        {
            GameObject prefab = (GameObject)Resources.Load(prefabName as string);
            return AddPopUp(prefab, mode, id, isBringForward, maskAlpha, canRemove);
        }

        public T AddPopUp<T>(string prefabName, bool mode = true, string id = null, bool isBringForward = true, float maskAlpha = MASK_ALPHA, bool canRemove = true)
        {
            GameObject popup = AddPopUp((GameObject)Resources.Load(prefabName as string), mode, id, isBringForward, maskAlpha, canRemove);
            if (popup != null)
            {
                T ret = popup.GetComponent<T>();
                if (ret == null)
                {
                    FLog.Debug("Can't Find Type: " + typeof(T).ToString());
                    RemovePopUp(popup);
                }
                else
                {
                    return ret;
                }
            }
            return default(T);
        }

        public GameObject AddPopUp(GameObject prefab, bool mode = true, string id = null, bool isBringForward = true, float maskAlpha = MASK_ALPHA, bool canRemove = true)
        {
            GameObject layer = GetPopUpLayer(true);
            if (layer == null || prefab == null)
                return null;

            GameObject popUp = null;
            GameObject maskClip = null;
            GameObject panel = null;
            DestroyEventTrigger trigger = null;

            if (!string.IsNullOrEmpty(id))
            {
                panel = GetPopUpPanel(id, layer);
                popUp = GetPopUp(panel);
            }
            else
            {
                if (prefab.transform.parent != null)
                {
                    if (prefab.transform.parent.parent == layer)
                    {
                        panel = prefab.transform.parent.gameObject;
                        popUp = prefab;
                    }
                }
            }

            if (popUp != null)
            {
                maskClip = GetMask(panel);
                if (mode == false && maskClip)
                    NGUITools.Destroy(maskClip);
                else if (mode == true && maskClip == null)
                    maskClip = CreateMask(panel, maskAlpha);
            }
            else
            {
                if (panel == null)
                    panel = NGUITools.AddChild(layer);

                panel.name = POPUPPANEL_NAME + (!string.IsNullOrEmpty(id) ? ("_" + id) : "");
                layer.SetActive(true);

                if (prefab.transform.parent == null)
                {
                    popUp = NGUITools.AddChild(panel, prefab);
                }
                else
                {
                    popUp = prefab;
                    Transform t = popUp.transform;
                    t.parent = panel.transform;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    popUp.layer = panel.layer;
                }

                panel.AddComponent<UIPanel>();
                trigger = popUp.GetComponent<DestroyEventTrigger>();
                if (trigger == null)
                    trigger = popUp.AddComponent<DestroyEventTrigger>();

                trigger.canBeBatchRemoved = canRemove;
                NGUITools.SetLayer(panel, panel.layer);
                trigger.onDestroy += OnDestroy;

                if (mode) maskClip = CreateMask(panel, maskAlpha);
            }

            AdjustPanelSortingOrderAndDepth(layer, panel);
            if (isBringForward) FUtil.BringForwardInParent(layer, panel);

            return popUp;
        }

        void AdjustPanelSortingOrderAndDepth(GameObject layer, GameObject obj)
        {
            if (obj == null)
                return;

            int sortingOrder = layer.transform.childCount * BASEPOPUPPANEL_SORTINGORDER;
            UIPanel[] objs = obj.GetComponentsInChildren<UIPanel>(true);
            foreach (UIPanel panel in objs)
            {
                panel.sortingOrder = sortingOrder;
                panel.depth += BASEPOPUPPANEL_DEPTH;//点击事件响应
            }
        }

        public GameObject GetPopUp(string id)
        {
            return GetPopUp(id, GetPopUpLayer());
        }

        public GameObject RemovePopUp(GameObject popUp)
        {
            if (popUp != null)
            {
                DestroyEventTrigger trigger = popUp.GetComponent<DestroyEventTrigger>();
                if (trigger != null)
                    trigger.onDestroy -= OnDestroy;

                Transform panel = popUp.transform.parent;
                if (panel != null && panel.name.IndexOf(POPUPPANEL_NAME) != -1)
                    NGUITools.Destroy(panel.gameObject);
                else
                    NGUITools.Destroy(popUp);
            }
            return popUp;
        }

        public GameObject RemovePopUp(string id)
        {
            GameObject panel = GetPopUp(id);
            if (panel != null)
            {
                GameObject popUp = GetPopUp(panel);
                return RemovePopUp(popUp);
            }
            return null;
        }

        public void RemoveAll()
        {
            GameObject layer = GetPopUpLayer();
            RemoveAll(layer);
        }

        private void RemoveAll(GameObject layer)
        {
            if (layer == null)
                return;

            Transform t = layer.transform;
            DestroyEventTrigger trigger;
            int iCanNotRemove = 0;
            while (t.childCount > iCanNotRemove)
            {
                Transform child = t.GetChild(iCanNotRemove);

                trigger = child.GetComponentInChildren<DestroyEventTrigger>();
                if (trigger != null)
                {
                    if (trigger.canBeBatchRemoved == false)
                    {
                        iCanNotRemove++;
                        continue;
                    }
                    trigger.onDestroy -= OnDestroy;
                }

                NGUITools.Destroy(child);
            }
        }

        public void Remove<T>() where T : Component
        {
            GameObject layer = GetPopUpLayer();
            Remove<T>(layer);
        }

        public void Remove<T>(GameObject layer) where T : Component
        {
            if (layer != null)
            {
                T[] childs = layer.GetComponentsInChildren<T>();
                DestroyEventTrigger trigger;
                for (int i = 0; i < childs.Length; i++)
                {
                    T child = childs[i];

                    trigger = child.GetComponent<DestroyEventTrigger>();
                    trigger.onDestroy -= OnDestroy;
                    NGUITools.Destroy(child.transform.parent);
                }
            }
        }

        public GameObject CreateMask(GameObject parent, float maskAlpha, string name = MASK_NAME)
        {
            //Create Mask GameObject
            GameObject maskClip = NGUITools.AddChild(parent, true);
            maskClip.name = MASK_NAME;

            //Create UI2DSprite
            UI2DSprite tx = maskClip.AddComponent<UI2DSprite>();
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();
            tx.sprite2D = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            tx.type = UIBasicSprite.Type.Sliced;
            tx.alpha = maskAlpha;
            UIWidget widget = tx;

            //Create BoxCollider and Widget Size
            BoxCollider box = maskClip.AddComponent<BoxCollider>();
            box.isTrigger = true;
            widget.depth = -1;
            widget.autoResizeBoxCollider = true;
            Vector2 size = FUtil.GetCurrentScreenSize();
            widget.width = (int)size.x;
            widget.height = (int)size.y;

            return maskClip;
        }

        private GameObject GetPopUpLayer(bool isNullCreate = false)
        {
            GameObject root = FUtil.GetRoot();
            if (root != null)
            {
                Transform layer = root.transform.Find(LAYER_NAME);
                if (layer != null)
                {
                    return layer.gameObject;
                }

                if (isNullCreate)
                {
                    GameObject layerGo = NGUITools.AddChild(root);
                    layerGo.name = LAYER_NAME;
                    layerGo.layer = root.layer;
                    return layerGo;
                }
            }
            return null;
        }

        private GameObject GetPopUp(string id, GameObject layer)
        {
            GameObject panel = GetPopUpPanel(id, layer);
            if (panel == null) return null;
            return GetPopUp(panel);
        }

        private GameObject GetPopUp(GameObject panel)
        {
            if (panel != null)
            {
                DestroyEventTrigger popUp = panel.GetComponentInChildren<DestroyEventTrigger>();
                if (popUp != null) return popUp.gameObject;
            }
            return null;
        }

        private GameObject GetMask(GameObject panel)
        {
            if (panel != null)
            {
                Transform mask = panel.transform.Find(MASK_NAME);
                if (mask != null) return mask.gameObject;
            }
            return null;
        }

        private GameObject GetPopUpPanel(string id, GameObject layer)
        {
            if (layer != null && !string.IsNullOrEmpty(id))
            {
                Transform panel = layer.transform.Find(POPUPPANEL_NAME + "_" + id);
                if (panel != null) return panel.gameObject;
            }
            return null;
        }

        private void OnDestroy(GameObject gameObject)
        {
            Transform parent;
            if (gameObject.transform.parent != null)
            {
                parent = gameObject.transform.parent;
            }
            else
            {
                DestroyEventTrigger trigger = gameObject.GetComponent<DestroyEventTrigger>();
                parent = trigger.parent;
            }

            if (parent != null)
            {
                if (parent.parent != null) NGUITools.Destroy(parent);
            }
        }
    }
}
