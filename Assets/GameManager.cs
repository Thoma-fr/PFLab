using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Platforms Tracker"), Range(0, 10)]
    private List<List<GameObject>> _placedPlatforms = new List<List<GameObject>>();
    public NamedInt[] _maxNumbers = new NamedInt[6];
    public GameObject hoveredPlatform;
    public GameObject deleteParticle;
    public AudioClip destroySound;

    private AudioSource _audioSource;
    private void Start()
    {
        for(int i = 0; i < 6; i++)
            _placedPlatforms.Add(new List<GameObject>());

        _audioSource = GetComponent<AudioSource>();

    }

    public bool DeleteHoveredPlatform()
    {
        if (hoveredPlatform == null)
            return false;

        int id = (int)hoveredPlatform.GetComponent<Platform>().pfType;

        _placedPlatforms[id - 1].Remove(hoveredPlatform);
        Instantiate(deleteParticle, hoveredPlatform.transform.position, Quaternion.identity);
        Destroy(hoveredPlatform);
        return true;
    }

    public void AddPlatformToTracker(GameObject platform, int id)
    {
        if (_placedPlatforms[id - 1].Count >= _maxNumbers[id - 1].value)
        {
            GameObject platformToDelete = _placedPlatforms[id - 1][0];
            _placedPlatforms[id - 1].Remove(platformToDelete);
            Instantiate(deleteParticle, platformToDelete.transform.position,Quaternion.identity);
            Destroy(platformToDelete);
            PlayDestroySound();
        }

        _placedPlatforms[id - 1].Add(platform);
    }

    private void PlayDestroySound()
    {
        _audioSource.pitch = UnityEngine.Random.Range(.75f, 1.25f);
        _audioSource.PlayOneShot(destroySound);
    }
}

[System.Serializable]
public struct NamedInt
{
    public string name;
    public int value;
}
