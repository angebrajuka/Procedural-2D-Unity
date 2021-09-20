using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public float speed;

    private SpriteRenderer m_renderer;

    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        m_renderer.color = new Color(m_renderer.color.r, m_renderer.color.g, m_renderer.color.b, m_renderer.color.a-(Time.deltaTime*speed));
    }
}