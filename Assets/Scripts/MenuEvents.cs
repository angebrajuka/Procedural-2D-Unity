using UnityEngine;
using TMPro;

using static GameState;
using static Singles;

public class MenuEvents : MonoBehaviour {
    public static void MainMenu() {
        FadeTransition.black = true;
        var msp = singles.worldGen.menuSeeds[Random.Range(0, singles.worldGen.menuSeeds.Length)];
        WorldGen.SetSeed(msp.seed);
        singles.menuCampfire.SetActive(true);
        singles.menuCampfire.transform.position = new Vector3(msp.x, msp.y, singles.menuCampfire.transform.position.z);

        singles.cameraFollow.toFollow = singles.menuCampfire.transform;
        singles.cameraFollow.Snap();

        singles.worldGen.GenerateMap();
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        PauseHandler.Pause();
        MenuHandler.MainMenu();
    }

    public static void LoadGame(int slot) {
        Save_Load.currentSave = slot;
        if(!Save_Load.TryLoad<SaveData>(slot)) {
            MenuHandler.SetMenu("NewGame");
            return;
        }
    }

    public static void NewGame(Transform buttons) {
        FadeTransition.black = true;
        MenuHandler.CloseAll();
        PauseHandler.Pause();

        var name = buttons.GetChild(0).GetComponent<TMP_InputField>().text;
        var seed = buttons.GetChild(1).GetComponent<TMP_InputField>().text;
        var dfct = buttons.GetChild(2).GetComponent<TMP_Dropdown>().value;

        gameState.saveName = name;
        WorldGen.SetSeed((ushort)Mathf.Abs(MathUtils.TryParse(seed, WorldGen.RandomSeed()))); // InputFieldClamp handles bounds
        gameState.difficulty = (byte)dfct;

        singles.menuCampfire.SetActive(false);
        DaylightCycle.time = DaylightCycle.k_DAY/2;
        singles.worldGen.GenerateMap();

        PlayerMovement.instance.StartGame();
    }
}