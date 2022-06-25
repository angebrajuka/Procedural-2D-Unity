using UnityEngine;

public class Tool_KeyCode : MonoBehaviour
{
    public KeyCode key;
    public int code;

    public void Get()
    {
        code =  (int)key;
    }
}