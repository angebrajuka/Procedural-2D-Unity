using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private WorldLoading worldLoading;
    [SerializeField] private Follow cameraFollow;
    [SerializeField] private PlayerAnimator pAnimator;
    [SerializeField] private float walkSpeed, runSpeed, waterSpeed;

    public static Rigidbody2D rb;
    private Collider2D c2d;
    private float halfColliderHeight;

    private int inX, inY;
    private bool sneak;

    public int X { get { return (int)Mathf.Floor(rb.position.x + c2d.offset.x); } }
    public int Y { get { return (int)Mathf.Floor(rb.position.y + c2d.offset.y - halfColliderHeight); } }
    public bool InWater { get { return worldLoading.IsWater(X, Y); } }
    public bool InOcean { get { return worldLoading.IsDeepWater(X, Y); } }
    public bool Moving { get { return !PauseHandler.paused && (rb.velocity != Vector2.zero); } }

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        c2d = GetComponent<Collider2D>();
        halfColliderHeight = (c2d.bounds.max.y - c2d.bounds.min.y) / 2f;
    }

    public void tp(float x, float y) {
        var pos = transform.position;
        pos.x = x;
        pos.y = y;
        transform.position = pos;
    }

    void Update() {
        inX = (PlayerInput.GetKey("move east" ) ? 1 : 0) + (PlayerInput.GetKey("move west" ) ? -1 : 0);
        inY = (PlayerInput.GetKey("move north") ? 1 : 0) + (PlayerInput.GetKey("move south") ? -1 : 0);
        sneak = (PlayerInput.GetKey("sneak"));

        pAnimator.UpdateMovement(
            (rb.velocity.x == 0) ? (Input.mousePosition.x > (Screen.width / 2) ? false : true) : (rb.velocity.x > 0 ? false : true),
            Moving,
            rb.velocity.magnitude
        );
    }

    void FixedUpdate() {
        rb.velocity = new Vector2(inX, inY).normalized * (InWater ? waterSpeed : sneak ? walkSpeed : runSpeed);
    }
}