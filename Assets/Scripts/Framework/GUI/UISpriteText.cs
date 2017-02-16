using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    [ExecuteInEditMode]
    public class UISpriteText : FBaseController
    {
        [Serializable]
        public struct CharSpritePair
        {
            public string letterChar;
            public Sprite letterSprite;
        }

        public CharSpritePair[] charSpritePairs;

        public enum TextAlign { LEFT, CENTER, RIGHT };

        public int fontHeight;
        public int maxWidth;
        public int padding;
        public int veticalPadding = 0;
        public int spaceWidth;
        public TextAlign textAlign;
        public int depth = 0;
        public string content = "";
        public Color fontColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color[] frontColorGradient;

        private Dictionary<char, Sprite> _charSpriteDic;

        public string text
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                InvalidView();
            }
        }

        protected override void InitUI()
        {
            base.InitUI();
            InitCharSpritesDic();
        }

        void OnGradientSetColor(UIWidget widget, int bufferOffset, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
        {
            if (frontColorGradient != null && frontColorGradient.Length > 1)
            {
                cols[1] = cols[2] = frontColorGradient[0];
                cols[0] = cols[3] = frontColorGradient[1];
            }
        }

        void InitCharSpritesDic()
        {
            if (_charSpriteDic == null)
            {
                _charSpriteDic = new Dictionary<char, Sprite>();
            }

            if (charSpritePairs != null)
            {
                foreach (CharSpritePair pair in charSpritePairs)
                {
                    if (pair.letterChar.Length > 0 && pair.letterSprite != null)
                    {
                        _charSpriteDic[pair.letterChar[0]] = pair.letterSprite;
                    }
                }
            }
        }

        void InitCharSpritesDicInEditMode()
        {
            if (charSpritePairs == null || charSpritePairs.Length == 0)
            {
                SpriteRenderer[] spriteRenders = transform.GetComponentsInChildren<SpriteRenderer>(true);
                if (spriteRenders.Length > 0)
                {
                    charSpritePairs = new CharSpritePair[spriteRenders.Length];
                    int i = 0;
                    foreach (SpriteRenderer spriteRender in spriteRenders)
                    {
                        CharSpritePair pair = new CharSpritePair();
                        pair.letterChar = spriteRender.name;
                        pair.letterSprite = spriteRender.sprite;
                        charSpritePairs[i++] = pair;
                        DestroyImmediate(spriteRender.gameObject);
                    }
                }
            }
            InitCharSpritesDic();
        }

        public void SetFontColor(Color color)
        {
            fontColor = color;
            UI2DSprite[] sprites = GetComponentsInChildren<UI2DSprite>();
            for (int j = 0; j < sprites.Length; j++)
            {
                sprites[j].color = fontColor;
            }
        }

        UI2DSprite AddSprite(Transform spritesContainer, Sprite letterSprite, int horDock, int spriteIndex)
        {
            GameObject letterObject = null;
            UI2DSprite uiSprite = null;
            if (spriteIndex < spritesContainer.childCount)
            {
                letterObject = spritesContainer.GetChild(spriteIndex).gameObject;
                uiSprite = letterObject.GetComponent<UI2DSprite>();
            }
            else
            {
                letterObject = NGUITools.AddChild(spritesContainer.gameObject);
                letterObject.name = "Letter";
                uiSprite = letterObject.AddComponent<UI2DSprite>();
            }

            uiSprite.sprite2D = letterSprite;
            uiSprite.color = fontColor;
            uiSprite.onPostFill = OnGradientSetColor;
            uiSprite.MakePixelPerfect();
            uiSprite.depth = depth;
            uiSprite.shader = uiSprite.shader;
            letterObject.transform.localPosition = new Vector3(horDock + uiSprite.width / 2, spriteIndex * veticalPadding, 0);
            return uiSprite;
        }

        protected override void Update()
        {
#if UNITY_EDITOR
            base.Update();
            if (!Application.isPlaying)
            {
                InitCharSpritesDicInEditMode();
                UpdateView();
            }
#endif
        }

        protected override void UpdateView()
        {
            Transform spritesContainer = null;

            if (transform.childCount == 0)
            {
                NGUITools.AddChild(gameObject);
            }
            spritesContainer = transform.GetChild(0);
            spritesContainer.name = "SpritesContainer";

            // default left align
            int currentHorDock = 0;
            int currentTotalWidth = 0;
            int spriteHeight = 0;
            int spriteAddIdx = 0;
            for (int i = 0; i < content.Length; i++)
            {
                if (_charSpriteDic.ContainsKey(content[i]))
                {
                    UI2DSprite uiSprite = AddSprite(spritesContainer, _charSpriteDic[content[i]], currentHorDock, spriteAddIdx++);
                    spriteHeight = Math.Max(spriteHeight, uiSprite.height);
                    currentHorDock += (uiSprite.width + padding);
                }
            }
            if (Application.isPlaying)
                FUtil.RemoveAllChildren(spritesContainer.gameObject, false, spriteAddIdx);
            else
                FUtil.RemoveAllChildren(spritesContainer.gameObject, true, spriteAddIdx);

            currentTotalWidth = currentHorDock - padding;

            float scaleFact = 1;
            int calWidth = 0;
            if (fontHeight > 0 && spriteHeight > 0)
            {
                scaleFact = fontHeight / (float)spriteHeight;
                calWidth = (int)(scaleFact * currentTotalWidth);
            }
            else
            {
                scaleFact = 1.0f;
                calWidth = currentTotalWidth;
            }

            if (maxWidth > 0 && calWidth > maxWidth)
            {
                scaleFact = (float)maxWidth / (float)currentTotalWidth;
            }

            spritesContainer.localScale = new Vector3(scaleFact, scaleFact, scaleFact);

            // text alignment
            if (textAlign == TextAlign.RIGHT)
            {
                spritesContainer.localPosition = new Vector3(-currentTotalWidth * scaleFact, 0, 0);
            }
            else if (textAlign == TextAlign.CENTER)
            {
                spritesContainer.localPosition = new Vector3(-currentTotalWidth / 2 * scaleFact, 0, 0);
            }
            else if (textAlign == TextAlign.LEFT)
            {
                spritesContainer.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}
