using Framework;

/// <summary>
/// Managers初始化
/// @zhenhaiwang
/// </summary>
namespace Project
{
    public class ProjectManager : MonoSingleton<ProjectManager>
    {
        void Start()
        {
            E2JManager.instance.enabled = true;
        }
    }
}
