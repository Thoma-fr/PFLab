using UnityEngine;
using UnityEngine.VFX;

public class PlatformMirror : Platform
{
    private Laser _laser;
    private int _previousLaserNb = 0;
    private Coroutine _laserDestroyCooldown;
    private bool _isHitByLaser;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Ghost Mirror Platform");
    }

    private void Start()
    {
        gameObject.layer = _isGhost ? LayerMask.NameToLayer("Ghost Mirror Platform") : LayerMask.NameToLayer("Platform");
    }

    private void FixedUpdate()
    {
        gameObject.layer = _isGhost ? LayerMask.NameToLayer("Ghost Mirror Platform") : LayerMask.NameToLayer("Platform");

        if (Time.timeScale >= 1f)
        {
            if (!_isHitByLaser && _laser)
            {
                Destroy(_laser.gameObject);
            }

            _isHitByLaser = false;
        }
    }

    private new void Update()
    {
        base.Update();

        if (Time.timeScale < 1f)
        {
            if (!_isHitByLaser && _laser)
            {
                Destroy(_laser.gameObject);
            }

            _isHitByLaser = false;
        }
    }

    public void GhostReflectLaser(Vector2 origin, Vector2 inDirection, Vector2 normal, int maxLasers, int laserNb, float laserRange, float laserWidth, Material laserMaterial, int laserOrderInLayer, Color laserGhostColor)
    {
        _isHitByLaser = true;

        // Create the laser
        if ((maxLasers - laserNb) > (maxLasers - _previousLaserNb) || !_laser)
        {
            if (_laser)
            {
                //Destroy(transform.GetChild(transform.childCount - 1).gameObject);
                Destroy(_laser.gameObject);
                //_laser = null;
            }

            GameObject laserGO = new("Laser");
            laserGO.transform.position = transform.position;
            laserGO.transform.rotation = transform.rotation;
            laserGO.transform.parent = transform;
            _laser = laserGO.AddComponent<Laser>();

            _laser.InitiateLaser(maxLasers - laserNb, laserNb, 0, laserRange, laserWidth, laserMaterial, laserOrderInLayer, Color.black, laserGhostColor, true, null);
            _previousLaserNb = laserNb;
        }

        _laser.UpdateLaser(origin, Vector2.Reflect(inDirection, normal).normalized, true);
    }
}
