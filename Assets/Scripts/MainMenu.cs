using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Destroy(this, 3);
    }

    void OnDestroy()
    {
        MenuHandler.MainMenu(true, true);
    }
}