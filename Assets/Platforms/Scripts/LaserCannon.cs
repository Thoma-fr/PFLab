using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannon : MonoBehaviour
{
    [Header("Laser settings")]
    [SerializeField, Tooltip("How much damage the laser deals per second.")]
    private float _damagePerSecond;
    [SerializeField, Tooltip("Max distance the laser can travel."), Range(10, 100)]
    private float _laserRange;
    [SerializeField, Tooltip("Max distance the laser can travel."), Range(1, 10)]
    private int _maxNbOfLasers;

    [Header("LineRenderer settings")]
    [SerializeField, Range(0, 1)]
    private float _laserWidth;
    [SerializeField]
    private Material _laserMaterial;
    [SerializeField]
    private int _laserOrderInLayer = -2;
    [SerializeField]
    private Color _laserColor;
    [SerializeField]
    private Color _laserGhostColor;

    private Laser _laser;
    private Platform _parentPlatform;

    //=========================================================

    private void CreateLaser()
    {
        if (!_laser)
        {
            GameObject laserGO = new("Laser");
            laserGO.transform.position = transform.position;
            laserGO.transform.rotation = transform.rotation;
            laserGO.transform.parent = transform;
            _laser = laserGO.AddComponent<Laser>();

            _laser.InitiateLaser(
                maxNbOfLasers: _maxNbOfLasers,
                laserNb: 1,
                damagePerSecond: _damagePerSecond,
                laserRange: _laserRange,
                laserWidth: _laserWidth,
                laserMaterial: _laserMaterial,
                laserOrderInLayer: _laserOrderInLayer,
                laserColor: _laserColor,
                laserGhostColor: _laserGhostColor,
                isGhost: true
                );
        }
    }

    private void Start()
    {
        transform.parent.TryGetComponent(out _parentPlatform);

        CreateLaser();
        _laser.DeactivateLaser();
        _laser.gameObject.SetActive(true);
    }

    private void Update()
    {
        _laser.UpdateLaser(transform.position, transform.up, _parentPlatform == null ? false : _parentPlatform.IsGhost);
    }
}
