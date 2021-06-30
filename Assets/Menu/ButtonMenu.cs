using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour {
    
    public void OnButtonPress() {
        PauseHandler.UnBlur();
        PauseHandler.UnPause();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
