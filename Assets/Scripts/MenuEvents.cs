using UnityEngine;
using TMPro;
using System.Threading.Tasks;

using static GameState;
using static Singles;

public class MenuEvents : MonoBehaviour {
    public static async void MainMenu() {
        FadeTransition.black = true;
        singles.menuCampfire.Lit = false;
        singles.worldGen.enabled = false;

        await Task.Delay(500);

        singles.worldGen.enabled = true;
        var msp = singles.worldGen.menuSeeds[Random.Range(0, singles.worldGen.menuSeeds.Length)];
        WorldGen.SetSeed(msp.seed);
        singles.menuCampfire.transform.position = new Vector3(msp.x, msp.y, singles.menuCampfire.transform.position.z);
        singles.cameraFollow.toFollow = singles.menuCampfire.transform;
        singles.cameraFollow.offset = new Vector3(0, 2.4f, 0);
        singles.cameraFollow.Snap();

        singles.worldGen.GenerateMap();
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        MenuHandler.MainMenu();
        singles.menuCampfire.gameObject.SetActive(true);
        PauseHandler.Pause();
        PauseHandler.blurred = false;
    }

    public static void LoadGame(int slot) {
        Save_Load.currentSave = slot;
        if(!Save_Load.TryLoad<SaveData>(slot)) {
            MenuHandler.SetMenu("NewGame");
            return;
        }
    }

    public static async void NewGame(Transform buttons) {
        singles.menuCampfire.Lit = false;
        MenuHandler.CloseAll();
        await Task.Delay(500);
        FadeTransition.black = true;
        await Task.Delay(500);

        var name = buttons.GetChild(0).GetComponent<TMP_InputField>().text;
        var seed = buttons.GetChild(1).GetComponent<TMP_InputField>().text;
        var dfct = buttons.GetChild(2).GetComponent<TMP_Dropdown>().value;

        gameState.saveName = name;
        WorldGen.SetSeed((ushort)Mathf.Abs(MathUtils.TryParse(seed, WorldGen.RandomSeed()))); // InputFieldClamp handles bounds
        gameState.difficulty = (byte)dfct;

        PauseHandler.Pause();
        singles.menuCampfire.gameObject.SetActive(false);
        DaylightCycle.time = DaylightCycle.k_DAY/2;
        singles.worldGen.GenerateMap();

        singles.pMovement.StartGame();
        PlayerMovement.rb.position = WorldGen.playerSpawnPoint;
    }
}