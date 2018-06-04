using UnityEngine.UI;
using Framework;

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
    }
}