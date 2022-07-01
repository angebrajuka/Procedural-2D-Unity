using UnityEngine;

public class Singles : MonoBehaviour {
    public static Singles /*hot*/ singles /*in your area*/;

    public void Start() {
        singles = this;
    }

    // hierarchy
    public Follow cameraFollow;
    public WorldGen worldGen;
    public Campfire menuCampfire;
    public PlayerAnimator pAnimator;
    public PlayerMovement pMovement;
}