using UnityEngine;

public class Laser : MonoBehaviour, IGhostable
{
    private static int _nbOfLasers;

    private float _damagePerSecond;
    private float _laserRange;
    private LineRenderer _lineRenderer;

    private Laser _reflectedLaser; // The reflected laser

    //=========================================================

    public void Ghostify() { Debug.Log("Ghostifying " + name); }
    public void RenderPhysical() { Debug.Log("Un-ghostifying " + name); }

    private void CreateLaser(int maxNbOfLasers)
    {
        if (!_reflectedLaser)
        {
            GameObject laserGO = new("Laser");
            laserGO.transform.position = transform.position;
            laserGO.transform.rotation = transform.rotation;
            laserGO.transform.parent = transform;
            _reflectedLaser = laserGO.AddComponent<Laser>();

            _reflectedLaser.InitiateLaser(
                maxNbOfLasers: maxNbOfLasers,
                damagePerSecond: _damagePerSecond,
                laserRange: _laserRange,
                laserWidth: _lineRenderer.startWidth,
                laserMaterial: _lineRenderer.material,
                laserOrderInLayer: _lineRenderer.sortingOrder,
                laserColor: _lineRenderer.startColor
                );
        }
    }

    public void InitiateLaser(int maxNbOfLasers, float damagePerSecond, float laserRange, float laserWidth, Material laserMaterial, int laserOrderInLayer, Color laserColor)
    {
        _damagePerSecond = damagePerSecond;
        _laserRange = laserRange;

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = laserMaterial;
        _lineRenderer.startColor = laserColor;
        _lineRenderer.endColor = laserColor;
        _lineRenderer.startWidth = laserWidth;
        _lineRenderer.endWidth = laserWidth;
        _lineRenderer.sortingOrder = laserOrderInLayer;

        _nbOfLasers++;

        if(_nbOfLasers < maxNbOfLasers)
        {
            CreateLaser(maxNbOfLasers);
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

            _reflectedLaser.UpdateLaser(hit.point, Vector2.Reflect(dir, hit.normal));
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

    public void UpdateLaser(Vector2 origin, Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, _laserRange);
        if (hit)
            Collision(origin, dir, hit);
        else
            NoCollision(origin, dir);
    }
}
