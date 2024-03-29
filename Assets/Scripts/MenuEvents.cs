using UnityEngine;
using TMPro;
using System.Threading.Tasks;

using static GameState;

[System.Serializable]
public struct MenuSeedPosition {
    public ushort seed;
    public float x, y;
}

public class MenuEvents : MonoBehaviour {
    public WorldGen worldGen;
    public WorldLoading worldLoading;
    public Campfire menuCampfire;
    public Follow cameraFollow;
    public Transform player;
    public MenuSeedPosition[] menuSeeds;

    const int FRAMES = 10;
    // number of frames to wait is arbitrary, just needs to be more than 1 to compensate for delta time, 10 seemed to solve all issues

    public void Start() {
        FadeTransition.black = true;
        FadeTransition.Snap();
        MainMenu();
    }

    public async void MainMenu() {
        player.gameObject.SetActive(false);
        FadeTransition.black = true;
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        menuCampfire.Lit = false;

        await FadeTransition.AwaitFade();

        var msp = menuSeeds[Random.Range(0, menuSeeds.Length)];
        menuCampfire.transform.position = new Vector3(msp.x, msp.y, menuCampfire.transform.position.z);
        cameraFollow.toFollow = menuCampfire.transform;
        cameraFollow.offset = new Vector3(0, 2.4f, 0);
        cameraFollow.Snap();

        worldLoading.world = worldGen.GenerateWorld(msp.seed);
        worldLoading.ForceLoadAllLagSpike();
        await General.NextFrame(FRAMES); // makes sure we wait several frames after lag spike for smooth fade, otherwise deltaTime is too long
        menuCampfire.gameObject.SetActive(true);

        MenuHandler.MainMenu();
        PauseHandler.Pause();
        PauseHandler.blurred = false;
        FadeTransition.black = false;

        await FadeTransition.AwaitFade();
        menuCampfire.Lit = true;
    }

    public void LoadGame(int slot) {
        Save_Load.currentSave = slot;
        if(!Save_Load.TryLoad<SaveData>(slot)) {
            MenuHandler.SetMenu("NewGame");
            return;
        }
    }

    public async void NewGame(Transform buttons) {
        menuCampfire.Lit = false;
        MenuHandler.CloseAll();
        await Task.Delay(350);
        FadeTransition.black = true;
        await FadeTransition.AwaitFade();

        gameState.saveName = buttons.GetChild(0).GetComponent<TMP_InputField>().text;
        var seed = (ushort)Mathf.Abs(MathUtils.TryParse(buttons.GetChild(1).GetComponent<TMP_InputField>().text, worldGen.RandomSeed())); // InputFieldClamp handles bounds
        gameState.difficulty = (byte)buttons.GetChild(2).GetComponent<TMP_Dropdown>().value;

        PauseHandler.Pause();
        menuCampfire.gameObject.SetActive(false);
        var world = worldGen.GenerateWorld(seed);
        worldLoading.world = world;
        player.gameObject.SetActive(true);
        player.position = world.playerSpawnPoint; // transform.position is updated instantly, rb.position wont update till next frame, caused loading bug
        cameraFollow.toFollow = player;
        cameraFollow.offset = Vector3.zero;
        cameraFollow.Snap();
        worldLoading.ForceLoadAllLagSpike();
        await General.NextFrame(FRAMES); // makes sure we wait till next frame after lag spike for smooth fade, otherwise deltaTime is too long

        DaylightCycle.time = DaylightCycle.k_DAY/2;
        FadeTransition.black = false;
        PauseHandler.blurred = false;
        PauseHandler.UnPause();
    }
}