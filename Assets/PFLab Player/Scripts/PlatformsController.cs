using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PLATFORM
{
    NONE = 0,
    BASIC = 1,
    BOUNCE = 2,
    BOOST = 3,
    LASER = 4,
    MIRROR = 5,
    GRAVITY = 6
}

public class PlatformsController : MonoBehaviour
{
    [Header("Platforms")]
    public GameManager gameManager;
    public PLATFORM choosenPlatform;
    public GameObject choosenPlatformGameobject;
    public GameObject[] platformPrefabs;
    public GameObject pfContainer;
    private GameObject _newPlatform;
    public bool RotatingPlatform { get; set; }

    [Header("Other")]
    public float timeScaleValue;
    public AudioClip platformShootSound;
    public AudioClip destroySound;

    private Vector2 _mousePosition;
    private AudioSource _audioSource;

    //==========================================================================

    private void Start()
    {
        choosenPlatform = PLATFORM.NONE;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        FollowMouse();

        if (RotatingPlatform && _newPlatform)
        {
            Vector3 dir = _newPlatform.transform.InverseTransformPoint(_mousePosition);
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            _newPlatform.transform.Rotate(0, 0, angle);
        }
    }

    public void SelectPF(int id)
    {
        choosenPlatform = (PLATFORM)id;

        if (choosenPlatformGameobject != null)
            Destroy(choosenPlatformGameobject);

        choosenPlatformGameobject = Instantiate(platformPrefabs[(int)choosenPlatform], _mousePosition, Quaternion.identity);
        choosenPlatformGameobject.GetComponent<Platform>().Ghostify();
    }

    public void SpawnPlatform()
    {
        _newPlatform = Instantiate(platformPrefabs[(int)choosenPlatform], _mousePosition, Quaternion.identity);
        _newPlatform.GetComponent<Platform>().Ghostify();
        choosenPlatformGameobject.SetActive(false);
        DOTween.KillAll();
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScaleValue, 0.2f);
    }

    public void PlacePlatform()
    {
        if (_newPlatform == null) return;

        choosenPlatformGameobject.SetActive(true);

        Platform platformScript = _newPlatform.GetComponent<Platform>();

        if (!platformScript.CanBePlaced)
        {
            Destroy(_newPlatform);
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.1f);
            return;
        }

        platformScript.pfType = choosenPlatform;
        platformScript.RenderPhysical();
        platformScript.gameManager = gameManager;
        gameManager.AddPlatformToTracker(_newPlatform, (int)choosenPlatform);
        DOTween.KillAll();
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.1f);
        PlayPlatformShootSound();
    }

    public void DeletePlatform()
    {
        if(gameManager.DeleteHoveredPlatform())
            PlayDestroySound();
    }

    public void FollowMouse()
    {
        if (choosenPlatform == PLATFORM.NONE) return;

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (choosenPlatformGameobject)
            choosenPlatformGameobject.transform.position = _mousePosition;
    }

    public void ToggleSelectionWheel(bool openClose)
    {
        if (openClose)
        {
            pfContainer.SetActive(true);
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, timeScaleValue, 0.2f);
            pfContainer.GetComponent<PFUIContainer>().AnimeUI();
        }
        else
        {
            pfContainer.SetActive(false);
            DOTween.KillAll();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.2f);
        }
    }

    private void PlayPlatformShootSound()
    {
        _audioSource.pitch = UnityEngine.Random.Range(.75f, 1.25f);
        _audioSource.PlayOneShot(platformShootSound);
    }

    private void PlayDestroySound()
    {
        _audioSource.pitch = UnityEngine.Random.Range(.75f, 1.25f);
        _audioSource.PlayOneShot(destroySound, .5f);
    }

    private void FixedUpdate()
    {
        Collider2D coll = Physics2D.OverlapCircle(_mousePosition, 0, LayerMask.GetMask("Platform", "Bouncing Platform", "Platform Mouse Detection"));
        if (coll && coll.gameObject.layer != LayerMask.NameToLayer("Platform Mouse Detection"))
            gameManager.hoveredPlatform = coll.gameObject;
        else if (coll && coll.gameObject.layer == LayerMask.NameToLayer("Platform Mouse Detection"))
            gameManager.hoveredPlatform = coll.transform.parent.gameObject;
        else
            gameManager.hoveredPlatform = null;
    }
}
