using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStart : MonoBehaviour
{
    public InputField inputSeed;

    public void OnClick()
    {
        WorldGen.SetSeed(ushort.TryParse(inputSeed.text, out var seed) ? seed : WorldGen.RandomSeed());
        // MenuHandler.Start();
    }
}
