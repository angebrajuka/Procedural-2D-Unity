using UnityEngine;
using TMPro;

public class MenuEvents : MonoBehaviour {
    public static void MainMenu() {
        ProceduralGeneration.instance.SetMainMenu();
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        PauseHandler.Pause();
        MenuHandler.MainMenu();
    }

    public static void LoadGame(int slot) {
        if(!Save_Load.TryLoad(slot)) {
            MenuHandler.SetMenu("NewGame");
            return;
        }
        FadeTransition.black = true;
        ProceduralGeneration.instance.GenerateMap();
        MenuHandler.CloseAll();
    }

    public static void NewGame(Transform buttons) {
        var name = buttons.GetChild(0).GetComponent<TMP_InputField>().text;
        var seed = buttons.GetChild(1).GetComponent<TMP_InputField>().text;
        var dfct = buttons.GetChild(2).GetComponent<TMP_Dropdown>().value;

        Debug.Log(name+","+seed+","+dfct);
    }
}