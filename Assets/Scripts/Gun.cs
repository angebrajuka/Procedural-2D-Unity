using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun
{
    public string name;
    public float damage;
    public string ammoType;
    public float spread;
    public float range;
    public int clipSize;
    public int ammoPerShot;
    public float reloadTime;
    public float recoil;
    public int pellets;
    public AudioClip audio_shoot;
    public float volume_shoot;
    public AudioClip audio_reload;
    public float volume_reload;
    public GameObject muzzleFlashPrefab;
    public float secondsBetweenShots;
    public Vector3 barrelTip;
    public Sprite sprite;
    public Transform transform;

    public static GameObject[] muzzleFlashes = new GameObject[]{null, null, null};

    public Gun(JsonGun json, Transform transform)
    {
        name = json.name;
        damage = json.damage;
        ammoType = json.ammoType;
        spread = json.spread;
        range = json.range;
        clipSize = json.clipSize;
        ammoPerShot = json.ammoPerShot;
        reloadTime = json.reloadTime;
        recoil = json.recoil;
        pellets = json.pellets;
        audio_shoot = Resources.Load<AudioClip>("Audio/Guns/"+name+"_shoot");
        volume_shoot = json.volume_shoot;
        audio_reload = Resources.Load<AudioClip>("Audio/Guns/"+name+"_reload");
        volume_reload = json.volume_reload;
        if(muzzleFlashes[json.muzzleFlashPrefab] == null)
        {
            muzzleFlashes[json.muzzleFlashPrefab] = Resources.Load<GameObject>("ItemData/MuzzleFlash"+json.muzzleFlashPrefab);
        }
        muzzleFlashPrefab = muzzleFlashes[json.muzzleFlashPrefab];
        secondsBetweenShots = 60.0f/json.rpm;
        damage /= pellets;
        barrelTip = new Vector3(json.barrelTip[0], json.barrelTip[1], 0);
        sprite = Resources.Load<Sprite>("Sprites/Guns/"+name);
        this.transform = transform;
    }

    public bool Shoot(Vector3 position, Vector2 direction, float angle, Rigidbody2D rigidbody)
    {
        AudioManager.PlayClip(audio_shoot, volume_shoot, Mixer.SFX, 0.5f, position);
        Transform flash = MonoBehaviour.Instantiate(muzzleFlashPrefab, transform.position, transform.rotation, transform).transform;
        var tip = barrelTip;
        if(direction.x < 0) tip.y *= -1;
        flash.localPosition += tip;
        Vector3 vec = flash.localScale;
        vec.x /= transform.localScale.x;
        vec.y /= transform.localScale.y;
        flash.localScale = vec;

        rigidbody.AddForce(-direction*recoil);

        PlayerStats.SetAmmo(PlayerStats.GetAmmo()-ammoPerShot);
        PlayerStats.gunRpmTimer = secondsBetweenShots;

        bool hit = false;
        for(int i=0; i<pellets; i++)
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