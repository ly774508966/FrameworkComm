using Framework;
using Framework.UI;

/// <summary>
/// Managers初始化
/// @zhenhaiwang
/// </summary>
namespace Project
{
    public sealed class ProjectManager : MonoSingleton<ProjectManager>
    {
        void Start()
        {
            E2JManager.instance.enabled = true;
            SoundManager.instance.enabled = true;
            UIPopManager.instance.enabled = true;
        }
    }
}
