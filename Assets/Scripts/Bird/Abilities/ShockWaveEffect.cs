using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    public float maxScale = 5f;
    public float expandSpeed = 15f;
    public float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private Color currentColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            currentColor = sr.color;
        }

        // Start tiny
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // Destroy after 1 second
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        // 1. Expand the scale rapidly
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(maxScale, maxScale, 1f), Time.deltaTime * expandSpeed);

        // 2. Move slightly forward in the direction it was spawned
        transform.Translate(Vector3.right * Time.deltaTime * 5f, Space.Self);

        // 3. Smoothly fade the alpha to 0
        if (sr != null)
        {
            currentColor.a = Mathf.Lerp(currentColor.a, 0f, Time.deltaTime * fadeSpeed);
            sr.color = currentColor;
        }
    }
}