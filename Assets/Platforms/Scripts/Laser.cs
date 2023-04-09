using UnityEngine;

public class Laser : MonoBehaviour, IGhostable
{

    private int _laserNb;
    private int _maxNbOfLasers;
    private float _damagePerSecond;
    private float _laserRange;
    private LineRenderer _lineRenderer;
    private bool _isGhost;
    private Color _laserColor;
    private Color _laserGhostColor;

    private Laser _reflectedLaser; // The reflected laser

    //=========================================================

    private void CreateLaser()
    {
        if (!_reflectedLaser)
        {
            GameObject laserGO = new("Laser");
            laserGO.transform.position = transform.position;
            laserGO.transform.rotation = transform.rotation;
            laserGO.transform.parent = transform;
            _reflectedLaser = laserGO.AddComponent<Laser>();

            _reflectedLaser.InitiateLaser(
                maxNbOfLasers: _maxNbOfLasers,
                laserNb: _laserNb + 1,
                damagePerSecond: _damagePerSecond,
                laserRange: _laserRange,
                laserWidth: _lineRenderer.startWidth,
                laserMaterial: _lineRenderer.material,
                laserOrderInLayer: _lineRenderer.sortingOrder,
                laserColor: _laserColor,
                laserGhostColor: _laserGhostColor,
                isGhost: _isGhost
                );
        }
    }

    public void InitiateLaser(int maxNbOfLasers, int laserNb, float damagePerSecond, float laserRange, float laserWidth, Material laserMaterial, int laserOrderInLayer, Color laserColor, Color laserGhostColor, bool isGhost)
    {
        _damagePerSecond = damagePerSecond;
        _laserRange = laserRange;
        _laserNb = laserNb;
        _maxNbOfLasers = maxNbOfLasers;

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = laserMaterial;
        _lineRenderer.startColor = laserColor;
        _lineRenderer.endColor = laserColor;
        _lineRenderer.startWidth = laserWidth;
        _lineRenderer.endWidth = laserWidth;
        _lineRenderer.sortingOrder = laserOrderInLayer;

        _laserColor = laserColor;
        _laserGhostColor = laserGhostColor;
        if (isGhost)
            Ghostify();
        else
            RenderPhysical();

        if(_laserNb < _maxNbOfLasers)
        {
            CreateLaser();
        }
    }

    public void DeactivateLaser()
    {
        if(_reflectedLaser && _reflectedLaser.gameObject.activeSelf)
            _reflectedLaser.DeactivateLaser();

        gameObject.SetActive(false);
    }

    private void Collision(Vector2 origin, Vector2 dir, RaycastHit2D hit)
    {
        // Draw the laser
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, hit.point);

        // Reflection stuff
        if (hit.collider.CompareTag("Mirror"))
        {
            if (!_reflectedLaser)
                return;

            if (!_reflectedLaser.gameObject.activeSelf)
                _reflectedLaser.gameObject.SetActive(true);

            _reflectedLaser.UpdateLaser(hit.point, Vector2.Reflect(dir, hit.normal), _isGhost);
        }
        else
        {
            if (_reflectedLaser && _reflectedLaser.gameObject.activeSelf)
                    _reflectedLaser.DeactivateLaser();
        }
    }

    private void NoCollision(Vector2 origin, Vector2 dir)
    {
        // Draw the laser
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, origin + _laserRange * dir);

        // Deactive all the child reflections
        if(_reflectedLaser && _reflectedLaser.gameObject.activeSelf)
            _reflectedLaser.DeactivateLaser();
    }

    public void UpdateLaser(Vector2 origin, Vector2 dir, bool isGhost)
    {
        if (isGhost)
            Ghostify();
        else
            RenderPhysical();

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, _laserRange, ~LayerMask.GetMask("Ghost Mirror Platform"));
        if (hit)
            Collision(origin, dir, hit);
        else
            NoCollision(origin, dir);

        if (!isGhost)
        {
            hit = Physics2D.Raycast(origin, dir, _laserRange);
            if (hit && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ghost Mirror Platform"))
            {
                if (hit.transform.TryGetComponent(out PlatformMirror mirror))
                    mirror.GhostReflectLaser(hit.point, dir.normalized, hit.normal, _maxNbOfLasers, _laserNb, _laserRange, _lineRenderer.startWidth, _lineRenderer.material, _lineRenderer.sortingOrder, _laserGhostColor);
            }
        }
    }

    public void Ghostify()
    {
        _isGhost = true;
        _lineRenderer.startColor = _laserGhostColor;
        _lineRenderer.endColor = _laserGhostColor;
    }

    public void RenderPhysical()
    {
        _isGhost = false;
        _lineRenderer.startColor = _laserColor;
        _lineRenderer.endColor = _laserColor;
    }
}
