using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSaveAndQuit : MonoBehaviour
{
    public void OnClick()
    {
        Save_Load.Save(PlayerStats.save);
        PauseHandler.UnPause();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
