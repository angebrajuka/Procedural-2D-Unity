public class MenuEvents {
    public static void MainMenu() {
        ProceduralGeneration.instance.SetMainMenu();
        DaylightCycle.time = DaylightCycle.k_NIGHT;
        PauseHandler.Pause();
        MenuHandler.MainMenu();
    }
}