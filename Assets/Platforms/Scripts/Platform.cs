using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Platform : MonoBehaviour, IGhostable
{
#if UNITY_EDITOR
    [Header("Debug settings")]
    public float drawWidthVerticalOffset;
#endif

    [Header("General settings")]
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
    [SerializeField, Tooltip("Ordered width steps for beyond Middles.")]
    private float[] _stepsMinWidth;
    [SerializeField]
    private PlatformSide[] _platformSidesLimits = new PlatformSide[2];
    [SerializeField]
    private PlatformSide[] _platformSidesExtenders = new PlatformSide[2];

    private BoxCollider2D _coll;
    private float _height;
    public float _leftWidth, _rightWidth;
    protected float _width; // Width of the platform, varies in relation to the available space. Capped at _minSize and _maxSize.
    protected bool _isGhost; // Is the current platform in ghost mode or not ?
    public bool IsGhost => _isGhost;

    //=========================================================================================================

    private void Awake()
    {
        if (_platformSidesLimits[0].regions.Length != _stepsMinWidth.Length || _platformSidesLimits[1].regions.Length != _stepsMinWidth.Length)
            throw new System.Exception("_platformSidesLimits does not have the same amount of steps as _stepsMinWidth.");

        if (_platformSidesLimits[0].regions.Length != _platformSidesExtenders[0].regions.Length + 1 || _platformSidesLimits[1].regions.Length != _platformSidesExtenders[1].regions.Length + 1)
            throw new System.Exception("_platformSidesLimits[X].regions.Length != _platformSidesExtenders[X].regions.Length + 1");

        TryGetComponent(out _coll);
        if (_coll)
            _height = _coll.size.y;
    }
    public void Ghostify()
    {
        _isGhost = true;
        DrawGhost();

        if(_coll == null)
        {
            if(TryGetComponent<BoxCollider2D>(out _coll) && gameObject.layer != LayerMask.NameToLayer("Ghost Mirror Platform"))
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
            if (TryGetComponent<BoxCollider2D>(out _coll))
                _coll.enabled = true;
        }
        else
            _coll.enabled = true;

        _platformGFX.MakeCorporeal();
    }

    /// <summary> Checks the surrounding area to see if there is enough space to spawn at least a _minSize sized platform. </summary>
    private bool IsThereEnoughSpace()
    {
        return (_leftWidth >= _minWidth * .5f && _rightWidth >= _minWidth * .5f);
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

    /// <summary> Get regions that should be visible based on left and right width; </summary>
    private Transform[,] GetRegionsToSpawn()
    {
        if(!IsThereEnoughSpace())
            return null;

        // Cas ou left et right sont tres petits
        if (_leftWidth < _stepsMinWidth[0] * .5f && _rightWidth < _stepsMinWidth[0] * .5f)
            return null;

        Transform[,] ret = new Transform[2, _stepsMinWidth.Length]; // Un liste pour la gauche et l'autre pour la droite

        for(int i = 0; i < _stepsMinWidth.Length; i++)
        {
            if (_leftWidth >= _stepsMinWidth[i])
            {
                ret[0, i] = _platformSidesLimits[0].regions[i];
            }

            if (_rightWidth >= _stepsMinWidth[i])
            {
                ret[1, i] = _platformSidesLimits[1].regions[i];
            }
        }

        return ret;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(_minWidth < 0) _minWidth = 0;
        if(_maxWidth <= _minWidth) _maxWidth = _minWidth;

        Vector3 upOffset = drawWidthVerticalOffset * transform.up;

        // Max width
        Gizmos.color = Color.green;
        Gizmos.DrawLine(-.5f * _maxWidth * transform.right + upOffset, .5f * _maxWidth * transform.right + upOffset);
        Gizmos.DrawCube(-.5f * _maxWidth * transform.right + upOffset, new(0.01f, .1f));
        Gizmos.DrawCube(.5f * _maxWidth * transform.right + upOffset, new(0.01f, .1f));

        // Steps widths
        for(int i = _stepsMinWidth.Length - 1; i >= 0; i--)
        {
            if (i == 0 && _stepsMinWidth[i] < _minWidth)
            {
                _stepsMinWidth[i] = _minWidth;
            }
            else if (i != 0 && (_stepsMinWidth[i] < _stepsMinWidth[i - 1]))
            {
                _stepsMinWidth[i] = _stepsMinWidth[i - 1];
            }
            else if (_stepsMinWidth[i] > _maxWidth)
            {
                _stepsMinWidth[i] = _maxWidth;
            }

            Gizmos.color = Color.Lerp(Color.red, Color.green, (i + 1) / (float)(_stepsMinWidth.Length + 1));
            Gizmos.DrawLine(-.5f * _stepsMinWidth[i] * transform.right + upOffset, .5f * _stepsMinWidth[i] * transform.right + upOffset);
            Gizmos.DrawCube(-.5f * _stepsMinWidth[i] * transform.right + upOffset, new(0.01f, .1f));
            Gizmos.DrawCube(.5f * _stepsMinWidth[i] * transform.right + upOffset, new(0.01f, .1f));
        }

        // Min width
        Gizmos.color = Color.red;
        Gizmos.DrawLine(-.5f * _minWidth * transform.right + upOffset, .5f * _minWidth * transform.right + upOffset);
        Gizmos.DrawCube(-.5f * _minWidth * transform.right + upOffset, new(0.01f, .1f));
        Gizmos.DrawCube(.5f * _minWidth * transform.right + upOffset, new(0.01f, .1f));

        // A SUPPRIMER SINON OUCH OUILLE LES PERFS HOLALA
        {
            if (_leftWidth > _maxWidth * .5f || _leftWidth < 0) _leftWidth = Mathf.Clamp(_leftWidth, 0, _maxWidth * .5f);
            if (_rightWidth > _maxWidth * .5f || _rightWidth < 0) _rightWidth = Mathf.Clamp(_rightWidth, 0, _maxWidth * .5f);

            // Desactive tout
            for(int i = 0; i < 2; i++)
            {
                foreach (Transform t in _platformSidesLimits[i].regions)
                {
                    t.gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                foreach (Transform t in _platformSidesExtenders[i].regions)
                {
                    t.gameObject.SetActive(false);
                }
            }

            // Reactive les bons
            Transform[,] regionsToSpawn = GetRegionsToSpawn();
            if (regionsToSpawn != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < _stepsMinWidth.Length; j++)
                    {
                        regionsToSpawn[i, j]?.gameObject.SetActive(true);
                    }
                }
            }

            int side = 0;
            foreach(PlatformSide platformSide in _platformSidesLimits)
            {
                for(int i = platformSide.regions.Length - 1; i >= 1; i--)
                {
                    SpriteShapeController ssc = _platformSidesExtenders[side].regions[i - 1]?.GetComponent<SpriteShapeController>();
                    ssc.spline.Clear();
                    ssc.spline.InsertPointAt(0, side == 0 ? platformSide.regions[i].position : platformSide.regions[i - 1].position);
                    ssc.spline.InsertPointAt(1, side == 0 ? platformSide.regions[i - 1].position : platformSide.regions[i].position);
                    ssc.gameObject.SetActive(platformSide.regions[i].gameObject.activeSelf);
                }

                side++;
            }
        }
    }
#endif
}

[System.Serializable]
struct PlatformSide
{
    public string name;
    [Tooltip("Ordered set of spawnable regions of the platform.")]
    public Transform[] regions;
}