using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonDifficulty : MonoBehaviour
{
    public byte difficulty;

    public void OnClick()
    {
        PlayerStats.difficulty = difficulty;
        PauseHandler.UnBlur();
        PauseHandler.UnPause();
        MenuHandler.currentMenuPrefab = null;
        SceneManager.LoadScene("Player", LoadSceneMode.Single);
    }
}
