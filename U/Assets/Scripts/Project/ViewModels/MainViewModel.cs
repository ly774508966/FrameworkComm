using Framework;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Project
{
    public class MainViewModel : BaseViewModel
    {
        public BindableProperty<string> message = new BindableProperty<string>();

        public override void OnInit()
        {
            base.OnInit();

            message.Value = "zhenhaiwang";
        }
    }
}