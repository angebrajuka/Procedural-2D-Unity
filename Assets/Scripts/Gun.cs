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
    public GameObject bulletTrailPrefab;
    public float secondsBetweenShots;
    public Vector3 barrelTip;
    public Transform transform;

    public static Dictionary<string, GameObject> muzzleFlashes = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> bulletTrails = new Dictionary<string, GameObject>();

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
        if(!muzzleFlashes.ContainsKey(json.muzzleFlashPrefab))
        {
            var go = Resources.Load<GameObject>("ItemData/MuzzleFlash_"+json.muzzleFlashPrefab);
            go.SetActive(false);
            muzzleFlashes.Add(json.muzzleFlashPrefab, go);
        }
        muzzleFlashPrefab = muzzleFlashes[json.muzzleFlashPrefab];
        if(!bulletTrails.ContainsKey(json.bulletTrailPrefab))
        {
            var go = Resources.Load<GameObject>("ItemData/BulletTrail_"+json.bulletTrailPrefab);
            go.SetActive(false);
            bulletTrails.Add(json.bulletTrailPrefab, go);
        }
        bulletTrailPrefab = bulletTrails[json.bulletTrailPrefab];
        secondsBetweenShots = 60.0f/json.rpm;
        damage /= pellets;
        barrelTip = new Vector3(json.barrelTip[0], json.barrelTip[1], 0);
        this.transform = transform;
    }

    public bool Shoot(Vector3 position, Vector2 direction, float angle, Rigidbody2D rigidbody)
    {
        AudioManager.PlayClip(audio_shoot, volume_shoot, Mixer.SFX, 0.5f, position);
        Transform flash = MonoBehaviour.Instantiate(muzzleFlashPrefab, position, transform.rotation, transform).transform;
        flash.gameObject.SetActive(true);
        if(direction.x < 0) barrelTip.y *= -1;
        flash.localPosition += barrelTip;
        Vector3 vec = flash.localScale;
        vec.x /= transform.localScale.x;
        vec.y /= transform.localScale.y;
        flash.localScale = vec;

        rigidbody.AddForce(-direction*recoil);

        PlayerStats.SetAmmo(PlayerStats.GetAmmo()-ammoPerShot);
        PlayerState.gunRpmTimer = secondsBetweenShots;

        bool hit = false;
        for(int i=0; i<pellets; i++)
        {
            if(ShootBullet(position, angle)) hit = true;
        }
        return hit;
    }

    public const int layerMask = ~(1<<8 | 1<<2 | 1<<10 | 1<<12 | 1<<11 | 1<<14);
    // 8 to ignore player, 2 to ignore ignore raycast, 10 to ignore ground, 12 to ignore knife, 11 & 14 to avoid items

    protected bool ShootBullet(Vector3 position, float angle)
    {
        angle += (Random.value-0.5f)*spread;

        Vector2 direction = Math.AngleToVector2(angle);
        position += Math.Vec3(direction)*barrelTip[0];

        RaycastHit2D raycast = Physics2D.Raycast(position, direction, range, layerMask);

        Transform trail = MonoBehaviour.Instantiate(bulletTrailPrefab, position, Quaternion.Euler(0, 0, angle)).transform;
        trail.gameObject.SetActive(true);
        trail.GetComponent<BulletTrail>().Init(new Vector3(raycast.collider == null ? range*Random.Range(0.95f, 1.05f) : raycast.distance, 0, 0));

        if(raycast.collider != null)
        {    
            var target = raycast.transform.GetComponent<Target>();

            if(target != null)
            {
                return target.Damage(damage, angle);
            }
        }

        return false;
    }
}