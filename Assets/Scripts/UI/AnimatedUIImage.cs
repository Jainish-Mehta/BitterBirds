using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))] // This forces Unity to add it automatically!
public class AnimatedUIImage : MonoBehaviour
{
    private Image uiImage;
    private SpriteRenderer fakeRenderer;

    void Awake()
    {
        uiImage = GetComponent<Image>();
        fakeRenderer = GetComponent<SpriteRenderer>();

        // 1. Force the Animator to play, even if invisible
        GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;

        // 2. Make the SpriteRenderer completely invisible so it doesn't draw behind the UI
        fakeRenderer.color = Color.clear;
    }

    void LateUpdate()
    {
        // 3. Copy the animated frame to the UI Image every single frame!
        if (fakeRenderer.sprite != null && uiImage.sprite != fakeRenderer.sprite)
        {
            uiImage.sprite = fakeRenderer.sprite;
        }
    }
}