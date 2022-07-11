using UnityEngine;
using TMPro;
using System.Threading.Tasks;

using static GameState;

public class MenuEvents : MonoBehaviour {
    public WorldGen worldGen;
    public Campfire menuCampfire;
    public Follow cameraFollow;
    public PlayerMovement pMovement;

    const int FRAMES = 10;
    // number of frames to wait is arbitrary, just needs to be more than 1 to compensate for delta time, 10 seemed to solve all issues

    private async Task AwaitFade() {
        while(!FadeTransition.Done) await Task.Yield();
    }

    // no guaruntee it runs on the very next frame, only guaruntee is waiting AT LEAST one frame (or c frames if c isnt 1)
    private async Task NextFrame(int c=1) {
        var current = Time.frameCount;
        while(Time.frameCount-c < current) await Task.Yield();
    }

    public async void MainMenu() {
        FadeTransition.black = true;
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        menuCampfire.Lit = false;
        worldGen.enabled = false;

        await AwaitFade();

        var msp = worldGen.menuSeeds[Random.Range(0, worldGen.menuSeeds.Length)];
        WorldGen.SetSeed(msp.seed);
        menuCampfire.transform.position = new Vector3(msp.x, msp.y, menuCampfire.transform.position.z);
        cameraFollow.toFollow = menuCampfire.transform;
        cameraFollow.offset = new Vector3(0, 2.4f, 0);
        cameraFollow.Snap();

        await worldGen.GenerateMapAsync();
        worldGen.ForceLoadAllLagSpike();
        await NextFrame(FRAMES); // makes sure we wait several frames after lag spike for smooth fade, otherwise deltaTime is too long
        menuCampfire.gameObject.SetActive(true);
        worldGen.enabled = true;

        MenuHandler.MainMenu();
        PauseHandler.Pause();
        PauseHandler.blurred = false;
        FadeTransition.black = false;

        await AwaitFade();
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
        await AwaitFade();

        gameState.saveName = buttons.GetChild(0).GetComponent<TMP_InputField>().text;
        WorldGen.SetSeed((ushort)Mathf.Abs(MathUtils.TryParse(buttons.GetChild(1).GetComponent<TMP_InputField>().text, WorldGen.RandomSeed()))); // InputFieldClamp handles bounds
        gameState.difficulty = (byte)buttons.GetChild(2).GetComponent<TMP_Dropdown>().value;

        PauseHandler.Pause();
        menuCampfire.gameObject.SetActive(false);
        await worldGen.GenerateMapAsync();
        pMovement.StartGame();
        worldGen.ForceLoadAllLagSpike();
        await NextFrame(FRAMES); // makes sure we wait till next frame after lag spike for smooth fade, otherwise deltaTime is too long

        DaylightCycle.time = DaylightCycle.k_DAY/2;
        FadeTransition.black = false;
        PauseHandler.blurred = false;
        PauseHandler.UnPause();
    }
}