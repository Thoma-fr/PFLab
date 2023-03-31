using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour, IGhostable
{
    [Header("General settings")]
    [SerializeField, Tooltip("Max life time of the platform.")]
    private float _lifeTime;
    [SerializeField, Tooltip("Minimum width of the platform.")]
    private float _minWidth;
    [SerializeField, Tooltip("Maximum width of the platform.")]
    private float _maxWidth;
    [SerializeField, Tooltip("Duration of the creation and destruction animations of the platform.")]
    private float _animationDuration;
    [SerializeField, Tooltip("Tweening of the creation and destruction animations of the platform.")]
    private AnimationCurve _animationCurve;

    [Header("GFX")]
    [SerializeField, Tooltip("GFX Component of this platform.")]
    private PlatformGFX _platformGFX;

    private Collider2D _coll;
    private float _age; // Current age of the platform.
    private const float _height = 1f; // Height of the platform, should always be a fixed number.
    protected float _width; // Width of the platform, varies in relation to the available space. Capped at _minSize and _maxSize.
    protected bool _isGhost; // Is the current platform in ghost mode or not ?

    //=========================================================================================================

    public void Ghostify()
    {
        _isGhost = true;
        DrawGhost();

        if(_coll == null)
        {
            if(TryGetComponent<Collider2D>(out _coll))
                _coll.enabled = false;
        }
        else
            _coll.enabled = false;

        _platformGFX.MakeGhost();
    }
    public void RenderPhysical()
    {
        _isGhost = false;
        CreatePlatform();

        if (_coll == null)
        {
            if (TryGetComponent<Collider2D>(out _coll))
                _coll.enabled = true;
        }
        else
            _coll.enabled = true;

        _platformGFX.MakeCorporeal();
    }

    /// <summary> Checks the surrounding area to see if there is enough space to spawn at least a _minSize sized platform. </summary>
    private bool IsThereEnoughSpace()
    {
        // TODO
        return true;
    }

    /// <summary> Draws a ghostly image of the platform as feedback to see how the platform will be place in the world. </summary>
    protected void DrawGhost()
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

    /// <summary> Modifies the current age of the platform. </summary>
    private void UpdateAge(float timeToAdd)
    {
        _age = Mathf.Clamp(_age + timeToAdd, 0, _lifeTime);
    }
}
