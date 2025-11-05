using UnityEngine;

[CreateAssetMenu(fileName = "SpellPage", menuName = "Scriptable Objects/Spell")]
public class SpellPage : ScriptableObject
{
    [Header("Prefab")]
    [field: SerializeField] public GameObject SpellPrefab { get; private set; }
    [Header("Spell Information")]
    [field: SerializeField] public string SpellName { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: TextArea][field: SerializeField] public string Description { get; private set; }

    [Header("Spell Attributes")]
    [field: SerializeField] public int ManaCost { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
}
