using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStart : MonoBehaviour
{
    public InputField inputSeed;

    public void OnClick()
    {
        float seed;
        ProceduralGeneration.SetSeed(float.TryParse(inputSeed.text, out seed) ? seed : ProceduralGeneration.RandomSeed());
        MenuHandler.Start();
    }
}
