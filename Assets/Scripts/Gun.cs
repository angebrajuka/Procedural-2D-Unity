using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Hierarchy
    public string gunName;
    public float damage;
    public byte h_ammoType;
    public float rpm;
    public float spread;
    public float range;
    public int clipSize;
    public int ammoPerShot;
    public float reloadTime;
    public float recoil;
    public int bullets;
    public AudioClip audio_shoot;
    public float volume_shoot;
    public AudioClip audio_reload;
    public float volume_reload;
    public Transform muzzleFlashPrefab;


    // stats
    [HideInInspector] public float secondsBetweenShots;
    [HideInInspector] public Item ammoType;
    [HideInInspector] public int ammo;

    // components
    public Transform barrelTip;

    protected void Start()
    {
        ammoType = (Item)(h_ammoType+(int)Item.BULLETS_SMALL);
        secondsBetweenShots = 60.0f/rpm;
        damage /= bullets;
        ammo = clipSize;
        barrelTip = transform.GetChild(0);
    }

    public bool Shoot(Vector3 position, Vector2 direction, float angle, Rigidbody2D rigidbody)
    {
        AudioManager.PlayClip(audio_shoot, volume_shoot, Mixer.SFX, 0.5f, position);
        if(muzzleFlashPrefab != null)
        {
            Transform flash = Instantiate(muzzleFlashPrefab, barrelTip.position, transform.rotation, transform);
            Vector3 vec = flash.localScale;
            vec.x /= transform.localScale.x;
            vec.y /= transform.localScale.y;
            flash.localScale = vec;
        }

        rigidbody.AddForce(-direction*recoil);
        
        
        ammo -= ammoPerShot;
        PlayerStats.gunRpmTimer = secondsBetweenShots;

        bool hit = false;
        for(int i=0; i<bullets; i++)
        {
            if(ShootBullet(position, angle)) hit = true;
        }
        return hit;
    }

    public static Vector3 AngleToVector3(float degrees)
    {
        degrees *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(degrees), Mathf.Sin(degrees), 0);
    }

    protected bool ShootBullet(Vector3 position, float angle)
    {
        angle += (Random.value-0.5f)*spread;

        Vector3 direction = AngleToVector3(angle);

        const int layerMask = ~(1<<8 | 1<<2 | 1<<10 | 1<<12);
        RaycastHit2D raycast = Physics2D.Raycast(position, direction, range, layerMask);
        if(raycast.collider != null)
        {    
            Target target = raycast.transform.GetComponent<Target>();

            if(target != null)
            {
                raycast.transform.GetComponent<Rigidbody2D>().AddForceAtPosition(direction*100, raycast.point);
                return target.Damage(damage);
            }
        }

        // if(bulletTrailPrefab != null)
        // {
        //     Transform trail = Instantiate(bulletTrailPrefab, barrelTip.position, bulletTrailPrefab.rotation);
        //     trail.localEulerAngles *= angle;
        //     Transform child = trail.GetChild(0);
        //     child.localPosition += Vector3.right*range/2;
            
        //     ParticleSystem ps = child.GetComponent<ParticleSystem>();
        //     ParticleSystem.ShapeModule shape = ps.shape;
        //     shape.scale += Vector3.right*range;
        // }

        return false;
    }
}