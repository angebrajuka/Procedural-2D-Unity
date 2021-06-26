using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonNewGame : MonoBehaviour {
    

    public int save;

    public void OnClick() {
        PauseHandler.UnBlur();
        PauseHandler.UnPause();
        MenuHandler.currentMenuPrefab = null;
        SceneManager.LoadScene("Player", LoadSceneMode.Single);
    }
}
