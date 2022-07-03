using UnityEngine;
using TMPro;
using System.Threading.Tasks;

using static GameState;
using static Singles;

public class MenuEvents : MonoBehaviour {
    const int FADE_DELAY = 500;
    const int FRAME_DELAY = 100;

    public static async void MainMenu() {
        FadeTransition.black = true;
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        singles.menuCampfire.Lit = false;
        singles.worldGen.enabled = false;

        await Task.Delay(FADE_DELAY);

        singles.worldGen.enabled = true;
        singles.worldGen.UnloadFar(true);
        var msp = singles.worldGen.menuSeeds[Random.Range(0, singles.worldGen.menuSeeds.Length)];
        WorldGen.SetSeed(msp.seed);
        singles.menuCampfire.transform.position = new Vector3(msp.x, msp.y, singles.menuCampfire.transform.position.z);
        singles.cameraFollow.toFollow = singles.menuCampfire.transform;
        singles.cameraFollow.offset = new Vector3(0, 2.4f, 0);
        await Task.Delay(FRAME_DELAY); // makes sure we wait till next frame for transform update
        singles.cameraFollow.Snap();

        singles.worldGen.GenerateMap();
        singles.worldGen.ForceLoadAllLagSpike();
        await Task.Delay(FRAME_DELAY); // makes sure we wait till next frame after lag spike for smooth fade, otherwise deltaTime is too long
        singles.menuCampfire.gameObject.SetActive(true);

        MenuHandler.MainMenu();
        PauseHandler.Pause();
        PauseHandler.blurred = false;
        FadeTransition.black = false;

        await Task.Delay(FADE_DELAY);
        singles.menuCampfire.Lit = true;
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
        await Task.Delay(300);
        FadeTransition.black = true;
        await Task.Delay(FADE_DELAY);

        gameState.saveName = buttons.GetChild(0).GetComponent<TMP_InputField>().text;
        WorldGen.SetSeed((ushort)Mathf.Abs(MathUtils.TryParse(buttons.GetChild(1).GetComponent<TMP_InputField>().text, WorldGen.RandomSeed()))); // InputFieldClamp handles bounds
        gameState.difficulty = (byte)buttons.GetChild(2).GetComponent<TMP_Dropdown>().value;

        PauseHandler.Pause();
        singles.menuCampfire.gameObject.SetActive(false);
        singles.worldGen.GenerateMap();
        singles.pMovement.StartGame();
        PlayerMovement.rb.position = WorldGen.playerSpawnPoint;
        await Task.Delay(FRAME_DELAY); // makes sure we wait till next frame for transform update
        singles.worldGen.ForceLoadAllLagSpike();
        await Task.Delay(FRAME_DELAY); // makes sure we wait till next frame after lag spike for smooth fade, otherwise deltaTime is too long

        DaylightCycle.time = DaylightCycle.k_DAY/2;
        FadeTransition.black = false;
        PauseHandler.blurred = false;
        PauseHandler.UnPause();
    }
}