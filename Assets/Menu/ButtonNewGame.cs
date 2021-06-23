using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonNewGame : MonoBehaviour {
    

    public void OnClick() {
        PauseHandler.UnBlur();
        PauseHandler.UnPause();
        Inventory.items.Clear();
        SceneManager.LoadScene("Player", LoadSceneMode.Single);
    }
}
