using UnityEngine;
using UnityEngine.U2D;

public class PlatformGFX : MonoBehaviour
{
    public Color ghostColor;
    public Color redColor;
    [SerializeField, Tooltip("All the sprite renderers elements for this object.")]
    private SpriteRenderer[] _spriteRenderers;
    [SerializeField, Tooltip("All the sprite shape renderers elements for this object.")]
    private SpriteShapeRenderer[] _spriteShapeRenderers;

    //=========================================================================================================

    public void MakeGhost()
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.color = ghostColor;
        }

        foreach (var renderer in _spriteShapeRenderers)
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

        foreach (var renderer in _spriteShapeRenderers)
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

        foreach (var renderer in _spriteShapeRenderers)
        {
            renderer.color = redColor;
        }
    }
}
