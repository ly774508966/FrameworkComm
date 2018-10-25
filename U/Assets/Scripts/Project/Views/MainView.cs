using UnityEngine.UI;
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
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;
            }
        }
    }
}