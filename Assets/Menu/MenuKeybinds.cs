using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuKeybinds : MonoBehaviour
{
    public GameObject keybindButton;
    public Transform overlay;

    void Start()
    {
        int i = 0;
        foreach(KeyValuePair<Keybind, string> pair in PlayerInput.keybindStrings)
        {
            GameObject obj = Instantiate(keybindButton, Vector3.zero, Quaternion.identity, this.transform);
            Transform transform = obj.transform;
            transform.localPosition = new Vector3(340, -50 - (80*i));
            ButtonKeybind button = transform.GetChild(0).GetComponent<ButtonKeybind>();
            button.keybind = pair.Key;
            button.overlay = overlay;

            Text keybindText = transform.GetChild(1).GetComponent<Text>();
            keybindText.text = pair.Value;

            i++;
        }
    }
}
