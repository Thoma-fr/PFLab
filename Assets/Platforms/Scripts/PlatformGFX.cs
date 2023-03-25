using UnityEngine;

public class PlatformGFX : MonoBehaviour
{
    public Color ghostColor;
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

    public void MakeCorporeal()
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.color = Color.white;
        }
    }
}
