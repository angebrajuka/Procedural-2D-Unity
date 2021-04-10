using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ammo {
    BULLETS,
    SHELLS,
    ENERGY
}

public class Gun : MonoBehaviour {

    // Hierarchy
    public string gunName;
    public Ammo ammoType;
    public float damage;
    public float rpm;
    public float spread;
    public float range;
    public int clipSize;
    public int ammoPerShot;
    public float reloadTime;
    public float recoil;
    public int bullets;
    public Sprite sprite;
    public AudioClip audio_shoot;
    public float volume_shoot;
    public AudioClip audio_reload;
    public float volume_reload;
    

    // components
    ParticleSystem muzzleFlash;


    // stats
    [HideInInspector] public float secondsBetweenShots;
    [HideInInspector] public int ammo;

    protected void Start() {
        secondsBetweenShots = 60.0f/rpm;
        damage /= bullets;
        ammo = clipSize;
        muzzleFlash = GetComponent<ParticleSystem>();
    }

    public bool Shoot(Vector3 position, Vector2 direction, float angle, Rigidbody2D rigidbody) {
        AudioManager.PlayClip(position, audio_shoot, volume_shoot, Mixer.SFX);
        muzzleFlash.Play();
        muzzleFlash.Emit(70);

        rigidbody.AddForce(-direction*recoil);
        
        
        ammo -= ammoPerShot;
        PlayerStats.state = PlayerStats.PlayerState.CYCLING;
        PlayerStats.gunTimer = secondsBetweenShots;

        bool hit = false;
        for(int i=0; i<bullets; i++) {
            if(ShootBullet(position, angle)) hit = true;
        }
        return hit;
    }

    public static Vector3 AngleToVector3(float degrees) {
        degrees *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(degrees), Mathf.Sin(degrees), 0);
    }

    protected bool ShootBullet(Vector3 position, float angle) {

        angle += (Random.value-0.5f)*spread;

        Vector3 direction = AngleToVector3(angle);

        const int layerMask = ~(1<<8 | 1<<2 | 1<<10 | 1<<12);
        RaycastHit2D raycast = Physics2D.Raycast(position, direction, range, layerMask);
        if(raycast.collider != null) {
            
            Target target = raycast.transform.GetComponent<Target>();

            if(target != null) {
                raycast.transform.GetComponent<Rigidbody2D>().AddForceAtPosition(direction*100, raycast.point);
                return target.Damage(damage);
            }
        }

        return false;
    }
}