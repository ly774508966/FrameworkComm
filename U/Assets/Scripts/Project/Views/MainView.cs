﻿using UnityEngine.UI;
using Framework;
using Framework.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Project
{
    public class MainView : BaseViewMVVM<MainViewModel>
    {
        public Text mainText;

        protected override void OnListenViewModel()
        {
            viewModel.message.OnValueChanged = (string value) =>
            {
                mainText.text = value;
            };
        }

        public void OnClickMainButton(int index)
        {
            Log.Debug("Click MainButton " + index.ToString());

            switch (index)
            {
                case 1:
                    {
                        UIPopManager.instance.PopUp("UI/UITest", true);
                    }
                    break;
                case 2:
                    {
                        UIPopManager.instance.PopUp("UI/UITest", true, false, 0.5f);
                    }
                    break;
                case 3:
                    {
                        UIPopManager.instance.PopUp("UI/UITest", false);
                    }
                    break;
                case 4:
                    {
                        UIPopManager.instance.PopUp("UI/UITest", false, false);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}