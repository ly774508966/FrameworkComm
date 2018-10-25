using UnityEngine;
using Framework;
using Framework.UI;

/// <summary>
/// @zhenhaiwang
/// </summary>
public class __TestFramework : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UIPopManager.instance.PopUp("UI/UITest_1", true, true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            UIPopManager.instance.PopUp("UI/UITest_2", false, true);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UIPopManager.instance.PopUp("UI/UITest_3", true, false);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UIPopManager.instance.PopUp("UI/UITest_4", false, false);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            UIPopManager.instance.PopBack();
        }
    }
}
