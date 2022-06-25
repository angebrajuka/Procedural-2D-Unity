using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStart : MonoBehaviour
{
    public InputField inputSeed;

    public void OnClick()
    {
        ProceduralGeneration.SetSeed(ushort.TryParse(inputSeed.text, out var seed) ? seed : ProceduralGeneration.RandomSeed());
        // MenuHandler.Start();
    }
}
