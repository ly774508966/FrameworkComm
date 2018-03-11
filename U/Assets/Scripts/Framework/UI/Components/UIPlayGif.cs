using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;
using Image = UnityEngine.UI.Image;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class UIPlayGif : BaseViewMVP
    {
        public string gifPath;

        public float speed = 1f;
        public bool autoPlay = true;

        private Image _image;

        private bool _isPlay = false;
        private int _frameIndex = 0;

        private List<Sprite> _gifFrameList = new List<Sprite>();

        protected override void Awake()
        {
            if (string.IsNullOrEmpty(gifPath))
                return;

            _image = GetComponent<Image>();

            System.Drawing.Image gifImage = System.Drawing.Image.FromFile(gifPath);
            FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);
            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);

                Bitmap frame = new Bitmap(gifImage.Width, gifImage.Height);
                System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);

                Texture2D frameTexture = new Texture2D(frame.Width, frame.Height);
                for (int x = 0; x < frame.Width; x++)
                {
                    for (int y = 0; y < frame.Height; y++)
                    {
                        System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                        frameTexture.SetPixel(x, frame.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                    }
                }
                frameTexture.Apply();

                _gifFrameList.Add(Sprite.Create(frameTexture, new Rect(0, 0, frameTexture.width, frameTexture.height), Vector2.zero));
            }

            _isPlay = autoPlay;
        }

        protected override void Update()
        {
            if (_isPlay == true)
            {
                _image.sprite = _gifFrameList[(int)(++_frameIndex * speed / 2f) % _gifFrameList.Count];
            }
            else
            {
                if (_frameIndex > 0)
                {
                    _frameIndex = 0;
                    _image.sprite = _gifFrameList[0];
                }
            }
        }

        public void Play()
        {
            _isPlay = true;
        }

        public void Stop()
        {
            _isPlay = false;
        }
    }
}