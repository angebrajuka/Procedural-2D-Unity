using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : MonoBehaviour {
    public float activateSpeed;
    public AudioSource audioSource;
    public Animator animator;
    public ParticleSystem ps;
    public FlickeringLight flicker;

    bool lit = false;
    public bool Lit {
        get {
            return lit;
        }
        set {
            lit = value;
            if(lit) {
                ps.Play();
            } else {
                ps.Pause();
            }
            flicker.on = lit;
            bool active = gameObject.activeSelf;
            gameObject.SetActive(true);
            animator.SetBool("lit", lit);
            gameObject.SetActive(active);
        }
    }

    void Update() {
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, lit ? 1 : 0, Time.deltaTime * activateSpeed);
    }
}