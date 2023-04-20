using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using static Unity.Burst.Intrinsics.X86;

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
    public AudioClip creationAnimationSound;

    [Header("GFX")]
    [SerializeField, Tooltip("GFX Component of this platform.")]
    private PlatformGFX _platformGFX;
    [SerializeField, Tooltip("Ordered width steps for beyond Middles.")]
    private float[] _stepsMinWidth;
    [SerializeField, Range(0, 1)]
    private float _colliderWidthOffset;
    [SerializeField]
    private Transform _middle;
    [SerializeField]
    private PlatformSide[] _platformSidesLimits = new PlatformSide[2];
    [SerializeField]
    private PlatformSide[] _platformSidesExtenders = new PlatformSide[2];

    private float _leftWidth, _rightWidth;
    protected BoxCollider2D _coll;
    protected float _width; // Width of the platform, varies in relation to the available space. Capped at _minSize and _maxSize.
    protected bool _isGhost; // Is the current platform in ghost mode or not ?
    public bool IsGhost => _isGhost;
    protected bool _canBePlaced = false;
    public bool CanBePlaced => _canBePlaced;
    public PLATFORM pfType { get; set; }
    public GameManager gameManager { get; set; }
    private Coroutine _creationAnimation;
    protected AudioSource _audioSource;

    //=========================================================================================================

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>(); 

        if (!CompareTag("LaserPlatform"))
        {
            if (_platformSidesLimits[0].regions.Length != _stepsMinWidth.Length || _platformSidesLimits[1].regions.Length != _stepsMinWidth.Length)
                throw new System.Exception("_platformSidesLimits does not have the same amount of steps as _stepsMinWidth.");

            if (_platformSidesLimits[0].regions.Length != _platformSidesExtenders[0].regions.Length + 1 || _platformSidesLimits[1].regions.Length != _platformSidesExtenders[1].regions.Length + 1)
                throw new System.Exception("_platformSidesLimits[X].regions.Length != _platformSidesExtenders[X].regions.Length + 1");
        }

        TryGetComponent(out _coll);

        _leftWidth = _maxWidth * .5f;
        _rightWidth = _maxWidth * .5f;
    }

    protected void Update()
    {
        if (!_isGhost)
            return;

        if (_creationAnimation != null)
            return;

        _width = _leftWidth + _rightWidth;

        if (!IsThereEnoughSpace())
        {
            _platformGFX.MakeRed();
            DeactivateAllRegions();
            _canBePlaced = false;
            return;
        }
        else
        {
            _platformGFX.MakeGhost();
            _canBePlaced = true;
        }

        if (CompareTag("LaserPlatform"))
            return;
        {
            DeactivateAllRegions();
            ActivateRelevantRegions();
            ActivateExtenders();
            UpdateCollider();
        }
    }

    public void Ghostify()
    {
        _isGhost = true;

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

        _platformGFX.DefaultColor();
    }

    /// <summary> Checks the surrounding area to see if there is enough space to spawn at least a _minSize sized platform. </summary>
    private bool IsThereEnoughSpace()
    {
        LayerMask mask = ~LayerMask.GetMask("Interface Interactable", "Ghost Mirror Platform", "Platform Mouse Detection");
        if (Physics2D.OverlapCircle(transform.position, 0.3f, mask))
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, _maxWidth * .5f, mask);
        _rightWidth = hit ? hit.distance : _maxWidth * .5f;

        hit = Physics2D.Raycast(transform.position, -transform.right, _maxWidth * .5f, mask);
        _leftWidth = hit ? hit.distance : _maxWidth * .5f;

        return (_leftWidth >= _minWidth * .5f && _rightWidth >= _minWidth * .5f);
    }

    /// <summary> Starts the animation of the platform's creation. </summary>
    public void CreatePlatform()
    {
        if (!CompareTag("LaserPlatform"))
            _creationAnimation = StartCoroutine(CreateAnimation());
    }

    /// <summary> The animation of the platform's creation. </summary>
    private IEnumerator CreateAnimation()
    {
        PlayCreationSound();
        Vector3 iniScaleMiddle = _middle.localScale;
        Vector2[] iniPositionsMiddles = new Vector2[2] { _platformSidesLimits[0].regions[0].localPosition, _platformSidesLimits[1].regions[0].localPosition };
        Vector2[] iniPositionsExtreme = new Vector2[2] { _platformSidesLimits[0].regions[1].localPosition, _platformSidesLimits[1].regions[1].localPosition };

        // les milieux
        _middle.gameObject.SetActive(false);
        bool limit00 = _platformSidesLimits[0].regions[0].gameObject.activeSelf;
        _platformSidesLimits[0].regions[0].gameObject.SetActive(false);
        bool limit10 = _platformSidesLimits[1].regions[0].gameObject.activeSelf;
        _platformSidesLimits[1].regions[0].gameObject.SetActive(false);

        // les extenders
        _platformSidesExtenders[0].regions[0].gameObject.SetActive(false);
        _platformSidesExtenders[1].regions[0].gameObject.SetActive(false);

        // les extremes
        bool limit01 = _platformSidesLimits[0].regions[1].gameObject.activeSelf;
        _platformSidesLimits[0].regions[1].gameObject.SetActive(false);
        bool limit11 = _platformSidesLimits[1].regions[1].gameObject.activeSelf;
        _platformSidesLimits[1].regions[1].gameObject.SetActive(false);

        SpriteShapeController ssc0 = _platformSidesExtenders[0].regions[0].GetComponent<SpriteShapeController>();
        SpriteShapeController ssc1 = _platformSidesExtenders[1].regions[0].GetComponent<SpriteShapeController>();


        int i = -1;
        float t = 1;
        while (i < 3)
        {
            if (_coll.enabled)
                _coll.enabled = false;

            switch (i)
            {
                case 0:
                    _middle.localScale = Vector3.LerpUnclamped(Vector3.zero, iniScaleMiddle, _animationCurve.Evaluate(t));
                    t += Time.deltaTime / _animationDuration;
                    yield return new WaitForSeconds(Time.deltaTime);
                break;

                case 1:
                    if(limit00)
                        _platformSidesLimits[0].regions[0].localPosition = Vector2.LerpUnclamped(Vector2.zero, iniPositionsMiddles[0], _animationCurve.Evaluate(t));
                    
                    if (limit10)
                        _platformSidesLimits[1].regions[0].localPosition = Vector2.LerpUnclamped(Vector2.zero, iniPositionsMiddles[1], _animationCurve.Evaluate(t));
                    
                    t += Time.deltaTime / _animationDuration;
                    yield return new WaitForSeconds(Time.deltaTime);
                    break;

                case 2:
                    if (limit01)
                    {
                        _platformSidesLimits[0].regions[1].localPosition = Vector2.LerpUnclamped(_platformSidesLimits[0].regions[0].localPosition, iniPositionsExtreme[0], _animationCurve.Evaluate(t));
                        ssc0.spline.Clear();
                        ssc0.spline.InsertPointAt(0, new Vector2(_platformSidesLimits[0].regions[1].localPosition.x - ssc0.transform.localPosition.x, 0));
                        ssc0.spline.InsertPointAt(1, Vector2.zero);
                    }

                    if (limit11)
                    {
                        _platformSidesLimits[1].regions[1].localPosition = Vector2.LerpUnclamped(_platformSidesLimits[1].regions[0].localPosition, iniPositionsExtreme[1], _animationCurve.Evaluate(t));
                        ssc1.spline.Clear();
                        ssc1.spline.InsertPointAt(0, Vector2.zero);
                        ssc1.spline.InsertPointAt(1, new Vector2(_platformSidesLimits[1].regions[1].localPosition.x - ssc1.transform.localPosition.x, 0));
                    }

                    t += Time.deltaTime / _animationDuration;
                    yield return new WaitForSeconds(Time.deltaTime);
                    break;

                default:
                    break;
            }

            if(t >= 1)
            {
                t = 0;
                i++;

                switch (i)
                {
                    case 0:
                        _middle.gameObject.SetActive(true);
                        break;

                    case 1:
                        _platformSidesLimits[0].regions[0].gameObject.SetActive(limit00);
                        _platformSidesLimits[1].regions[0].gameObject.SetActive(limit10);
                        break;

                    case 2:
                        _platformSidesExtenders[0].regions[0].gameObject.SetActive(limit01);
                        _platformSidesExtenders[1].regions[0].gameObject.SetActive(limit11);
                        _platformSidesLimits[0].regions[1].gameObject.SetActive(limit01);
                        _platformSidesLimits[1].regions[1].gameObject.SetActive(limit11);
                        break;

                    default:
                        break;
                }
            }
        }

        _coll.enabled = true;
        _creationAnimation = null;

        if(this is PlatformGravity)
            (this as PlatformGravity).PlayGravitySound();

        yield break;
    }

    private void DeactivateAllRegions()
    {
        for(int i = 0; i < 2; i++)
        {
            foreach (PlatformSide side in (i == 0 ? _platformSidesLimits : _platformSidesExtenders))
            {
                foreach (Transform t in side.regions)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary> Get regions that should be visible based on left and right width; </summary>
    private Transform[,] GetRegionsToSpawn()
    {
        if (!IsThereEnoughSpace())
        {
            _platformGFX.MakeRed();
            return null;
        }
        else
        {
            if(!_isGhost)
                _platformGFX.DefaultColor();
            else
                _platformGFX.MakeGhost();
        }

        // Cas ou left et right sont tres petits
        if (_leftWidth < _stepsMinWidth[0] * .5f && _rightWidth < _stepsMinWidth[0] * .5f)
            return null;

        Transform[,] ret = new Transform[2, _stepsMinWidth.Length]; // Un liste pour la gauche et l'autre pour la droite

        for(int i = 0; i < _stepsMinWidth.Length; i++)
        {
            if (_leftWidth >= _stepsMinWidth[i] * .5f)
            {
                ret[0, i] = _platformSidesLimits[0].regions[i];
            }

            if (_rightWidth >= _stepsMinWidth[i] * .5f)
            {
                ret[1, i] = _platformSidesLimits[1].regions[i];
            }
        }

        return ret;
    }

    private void ActivateRelevantRegions()
    {
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
    }

    private void ActivateExtenders()
    {
        int side = 0;
        foreach (PlatformSide platformSide in _platformSidesLimits)
        {
            for (int i = platformSide.regions.Length - 1; i >= 1; i--)
            {
                // Regle la position des extremites.
                if (i == platformSide.regions.Length - 1 && platformSide.regions[i])
                    platformSide.regions[i].localPosition = (side == 0 ? -_leftWidth : _rightWidth) * Vector2.right;

                SpriteShapeController ssc = _platformSidesExtenders[side].regions[i - 1]?.GetComponent<SpriteShapeController>();
                ssc.spline.Clear();
                ssc.spline.InsertPointAt(0, side == 0 ? new Vector2(-_leftWidth - ssc.transform.localPosition.x, 0) : Vector2.zero);
                ssc.spline.InsertPointAt(1, side == 0 ? Vector2.zero : new Vector2(_rightWidth - ssc.transform.localPosition.x, 0));
                ssc.gameObject.SetActive(platformSide.regions[i].gameObject.activeSelf);
            }

            side++;
        }
    }

    private void UpdateCollider()
    {
        if (!_coll)
            _coll = GetComponent<BoxCollider2D>();

        if (_leftWidth < _stepsMinWidth[0] * .5f && _rightWidth < _stepsMinWidth[0] * .5f)
        {
            _coll.size = new(_minWidth, _coll.size.y);
            _coll.offset = Vector2.zero;
        }
        else
        {
            _width = _leftWidth + _rightWidth + _colliderWidthOffset * (0.5f * (_platformSidesLimits[0].regions[_platformSidesLimits[0].regions.Length - 1].gameObject.activeSelf ? 1 : 0) + 0.5f * (_platformSidesLimits[1].regions[_platformSidesLimits[1].regions.Length - 1].gameObject.activeSelf ? 1 : 0));
            _coll.size = new(_width, _coll.size.y);
            _coll.offset = new(((_rightWidth + _colliderWidthOffset * (_platformSidesLimits[1].regions[_platformSidesLimits[1].regions.Length - 1].gameObject.activeSelf ? 1 : 0)) - (_leftWidth + _colliderWidthOffset * (_platformSidesLimits[0].regions[_platformSidesLimits[0].regions.Length - 1].gameObject.activeSelf ? 1 : 0))) * .5f, 0);
        }
    }

    public void PlayCreationSound()
    {
        _audioSource.pitch = UnityEngine.Random.Range(.90f, 1.1f);
        _audioSource.PlayOneShot(creationAnimationSound);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (EditorApplication.isPlaying)
            return;

        if(_minWidth < 0) _minWidth = 0;
        if(_maxWidth <= _minWidth) _maxWidth = _minWidth;
        if(_colliderWidthOffset < 0) _colliderWidthOffset = 0;

        Vector3 upOffset = drawWidthVerticalOffset * transform.up;

        // Collider width offset
        Gizmos.color = new Color32(255, 127, 0, 255); // Orange
        Gizmos.DrawLine(transform.position - .5f * (_maxWidth + _colliderWidthOffset) * transform.right + upOffset, transform.position + .5f * (_maxWidth + _colliderWidthOffset) * transform.right + upOffset);
        Gizmos.DrawCube(transform.position - .5f * (_maxWidth + _colliderWidthOffset) * transform.right + upOffset, new(0.01f, .1f));
        Gizmos.DrawCube(transform.position + .5f * (_maxWidth + _colliderWidthOffset) * transform.right + upOffset, new(0.01f, .1f));

        // Max width
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - .5f * _maxWidth * transform.right + upOffset, transform.position + .5f * _maxWidth * transform.right + upOffset);
        Gizmos.DrawCube(transform.position - .5f * _maxWidth * transform.right + upOffset, new(0.01f, .1f));
        Gizmos.DrawCube(transform.position + .5f * _maxWidth * transform.right + upOffset, new(0.01f, .1f));

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
            Gizmos.DrawLine(transform.position - .5f * _stepsMinWidth[i] * transform.right + upOffset, transform.position + .5f * _stepsMinWidth[i] * transform.right + upOffset);
            Gizmos.DrawCube(transform.position - .5f * _stepsMinWidth[i] * transform.right + upOffset, new(0.01f, .1f));
            Gizmos.DrawCube(transform.position + .5f * _stepsMinWidth[i] * transform.right + upOffset, new(0.01f, .1f));
        }

        // Min width
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - .5f * _minWidth * transform.right + upOffset, transform.position + .5f * _minWidth * transform.right + upOffset);
        Gizmos.DrawCube(transform.position - .5f * _minWidth * transform.right + upOffset, new(0.01f, .1f));
        Gizmos.DrawCube(transform.position + .5f * _minWidth * transform.right + upOffset, new(0.01f, .1f));

        if (_leftWidth > _maxWidth * .5f || _leftWidth < 0) _leftWidth = Mathf.Clamp(_leftWidth, 0, _maxWidth * .5f);
        if (_rightWidth > _maxWidth * .5f || _rightWidth < 0) _rightWidth = Mathf.Clamp(_rightWidth, 0, _maxWidth * .5f);
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