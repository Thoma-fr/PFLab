using DG.Tweening;
using UnityEngine;

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

    private Vector2 _mousePosition;

    //==========================================================================

    private void Start()
    {
        choosenPlatform = PLATFORM.NONE;
    }

    private void Update()
    {
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
            return;
        }

        platformScript.pfType = choosenPlatform;
        platformScript.RenderPhysical();
        platformScript.gameManager = gameManager;
        gameManager.AddPlatformToTracker(_newPlatform, (int)choosenPlatform);
        DOTween.KillAll();
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.1f);
    }

    public void DeletePlatform()
    {
        gameManager.DeleteHoveredPlatform();
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
}
