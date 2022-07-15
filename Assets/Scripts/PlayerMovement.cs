using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private WorldLoading worldLoading;
    [SerializeField] private Follow cameraFollow;
    [SerializeField] private PlayerAnimator pAnimator;
    [SerializeField] private float walkSpeed, accel, decel, runSpeed, waterSpeed;

    public static Rigidbody2D rb;
    private Collider2D c2d;
    private float halfColliderHeight;

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
        var input = new Vector2(
            (PlayerInput.GetKey("move east" ) ? 1 : 0) + (PlayerInput.GetKey("move west" ) ? -1 : 0),
            (PlayerInput.GetKey("move north") ? 1 : 0) + (PlayerInput.GetKey("move south") ? -1 : 0)
        );
        bool sneak = PlayerInput.GetKey("sneak");
        var target = input.normalized * (InWater ? waterSpeed : (sneak ? walkSpeed : runSpeed));
        var targetWalk = input.normalized * walkSpeed;
        float cornerWalkSpeed = walkSpeed * Math.ROOT_ONE_HALF;

        var vel = rb.velocity;

        float rbxSign = Mathf.Sign(vel.x);
        float rbySign = Mathf.Sign(vel.y);
        vel -= new Vector2(
            (input.x != rbxSign || Mathf.Abs(vel.x) > Mathf.Abs(target.x)) ? rbxSign : 0,
            (input.y != rbySign || Mathf.Abs(vel.y) > Mathf.Abs(target.y)) ? rbySign : 0
        ).normalized * decel * Time.deltaTime;
        vel = new Vector2(
            Mathf.Abs(vel.x) <= (input.y == 0 ? walkSpeed : cornerWalkSpeed) ? targetWalk.x : vel.x,
            Mathf.Abs(vel.y) <= (input.x == 0 ? walkSpeed : cornerWalkSpeed) ? targetWalk.y : vel.y
        );

        if(!sneak) {
            vel += input.normalized * accel * Time.deltaTime;
        }
        vel.Cap(InWater ? waterSpeed : runSpeed);
        rb.velocity = vel;

        pAnimator.UpdateMovement(
            (rb.velocity.x == 0) ? (Input.mousePosition.x > (Screen.width / 2) ? false : true) : (rb.velocity.x > 0 ? false : true),
            Moving,
            rb.velocity.magnitude
        );
    }
}