using UnityEngine;

public class OnExit : MonoBehaviour
{
    void OnApplicationQuit()
    {
        Save_Load.Save(PlayerStats.save);
    }
}