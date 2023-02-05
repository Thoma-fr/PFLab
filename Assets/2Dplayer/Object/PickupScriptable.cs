using UnityEngine;
[CreateAssetMenu(fileName ="ObjectScriptable",menuName ="ScriptableObject/Object")]
public class PickupScriptable : ScriptableObject
{
    [field: SerializeField] public int maxHealthToAdd { get; private set; }
    [field: SerializeField] public int maxSpeedToAdd { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
}
