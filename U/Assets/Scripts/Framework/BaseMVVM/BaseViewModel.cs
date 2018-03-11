/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public abstract class BaseViewModel
    {
        public BaseViewModel() { }

        public virtual void OnInit() { }

        public virtual void OnDestroy() { }
    }
}