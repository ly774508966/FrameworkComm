using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [ExecuteInEditMode]
    public class UISpriteText : BaseView
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
        public TextAlign textAlign;
        public string mText = "";
        public Color fontColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        private Dictionary<char, Sprite> _charSpriteDic;

        public string text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
                InvalidView();
            }
        }

        public void SetText(string _text)
        {
            mText = _text;
            InvalidView();
        }

        public void SetFontColor(Color color)
        {
            fontColor = color;
            Image[] images = GetComponentsInChildren<Image>();
            for (int j = 0; j < images.Length; j++)
            {
                images[j].color = fontColor;
            }
        }

        protected override void Start()
        {
            InitCharSpritesDic();
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

        Image AddSprite(Transform spritesContainer, Sprite letterSprite, int horDock, int spriteIndex)
        {
            GameObject letterObject = null;
            Image image = null;

            if (spriteIndex < spritesContainer.childCount)
            {
                letterObject = spritesContainer.GetChild(spriteIndex).gameObject;
                image = letterObject.GetComponent<Image>();
            }
            else
            {
                letterObject = UIUtils.AddChild(spritesContainer.gameObject, null);
                letterObject.name = "Letter";
                image = letterObject.AddComponent<Image>();
            }

            image.sprite = letterSprite;
            image.color = fontColor;
            image.SetNativeSize();
            letterObject.transform.localPosition = new Vector3(horDock + (int)image.preferredWidth / 2, spriteIndex * veticalPadding, 0);

            return image;
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
                UIUtils.AddChild(gameObject, null);
            }
            spritesContainer = transform.GetChild(0);
            spritesContainer.name = "SpritesContainer";

            // default left align
            int currentHorDock = 0;
            int currentTotalWidth = 0;
            int spriteHeight = 0;
            int spriteAddIdx = 0;
            for (int i = 0; i < mText.Length; i++)
            {
                if (_charSpriteDic.ContainsKey(mText[i]))
                {
                    Image image = AddSprite(spritesContainer, _charSpriteDic[mText[i]], currentHorDock, spriteAddIdx++);
                    spriteHeight = Math.Max(spriteHeight, (int)image.preferredHeight);
                    currentHorDock += ((int)image.preferredWidth + padding);
                }
            }
            if (Application.isPlaying)
                UIUtils.RemoveAllChildrenFrom(spritesContainer.gameObject, false, spriteAddIdx);
            else
                UIUtils.RemoveAllChildrenFrom(spritesContainer.gameObject, true, spriteAddIdx);

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
                scaleFact = (float)maxWidth / currentTotalWidth;
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
