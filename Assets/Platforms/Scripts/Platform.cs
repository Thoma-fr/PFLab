using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("General settings")]
    [SerializeField, Tooltip("Max life time of the platform.")]
    private float _lifeTime;
    [SerializeField, Tooltip("Minimum width of the platform.")]
    private float _minSize;
    [SerializeField, Tooltip("Maximum width of the platform.")]
    private float _maxSize;
    [SerializeField, Tooltip("Duration of the creation and destruction animations of the platform.")]
    private float _animationDuration;
    [SerializeField, Tooltip("Tweening of the creation and destruction animations of the platform.")]
    private AnimationCurve _animationCurve;

    private float _age; // Current age of the platform.
    private const float _height = 1f; // Height of the platform, should always be a fixed number.
    private float _width; // Width of the platform, varies in relation to the available space. Capped at _minSize and _maxSize.
    private bool _isGhost = true; // Is the current platform in ghost mode or not ?

    //=========================================================================================================

    /// <summary> Checks the surrounding area to see if there is enough space to spawn at least a _minSize sized platform. </summary>
    private bool IsThereEnoughSpace()
    {
        // TODO
        return true;
    }

    /// <summary> Draws a ghostly image of the platform as feedback to see how the platform will be place in the world. </summary>
    public void DrawGhost()
    {
        // TODO
    }

    /// <summary> Starts the animation of the platform's creation. </summary>
    public void CreatePlatform()
    {
        // TODO
    }

    /// <summary> The animation of the platform's creation. </summary>
    private IEnumerator CreateAnimation()
    {
        // TODO
        yield break;
    }

    /// <summary> Starts the animation of the platform's destruction. </summary>
    public void DestroyPlatform()
    {
        // TODO
    }

    /// <summary> The animation of the platform's destruction. </summary>
    private IEnumerator DestroyAnimation()
    {
        // TODO
        yield break;
    }

    /// <summary> Rotates the platform towards the given direction. </summary>
    public void RotatePlatformTowards(Vector2 platformToCursor)
    {
        // TODO
    }

    /// <summary> Modifies the current age of the platform. </summary>
    private void UpdateAge(float timeToAdd)
    {
        _age = Mathf.Clamp(_age + timeToAdd, 0, _lifeTime);
    }
}
