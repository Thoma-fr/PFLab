using UnityEngine;

public class PlatformGFX : MonoBehaviour
{
    public Color ghostColor;
    public Color redColor;
    [SerializeField, Tooltip("All the sprite renderers elements for this object.")]
    private SpriteRenderer[] _spriteRenderers;

    //=========================================================================================================

    public void MakeGhost()
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.color = ghostColor;
        }
    }

    public void DefaultColor()
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.color = Color.white;
        }
    }

    public void MakeRed()
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.color = redColor;
        }
    }
}
