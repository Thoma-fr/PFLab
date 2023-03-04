using UnityEngine;

public class PlatformLaser : Platform
{
    [Header("Laser settings")]
    [SerializeField, Tooltip("How much damage the laser deals per second.")]
    private float _damagePerSecond;
    [SerializeField, Tooltip("Max distance the laser can travel."), Range(10, 100)]
    private float _laserRange;
    [SerializeField, Range(0, 1)]
    private float _laserWidth;

    protected LineRenderer _lineRenderer;
    protected RaycastHit2D _hit;

    //=========================================================

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;
    }

    protected void ShootRaycast(Vector2 origin, Vector2 direction)
    {
        _hit = Physics2D.Raycast(origin, direction, _laserRange);
    }

    protected void DrawLaser(Vector2 origin, Vector2 direction)
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, origin);
        if (_hit)
            _lineRenderer.SetPosition(1, _hit.point);
        else
            _lineRenderer.SetPosition(1, origin + direction * _laserRange);
    }

    private void LateUpdate()
    {
        if(this is not PlatformMirror)
        {
            ShootRaycast(transform.position, transform.up);
            DrawLaser(transform.position, transform.up);
        }

        if (_hit && _hit.collider.CompareTag("Mirror"))
        {

        }
    }
}
