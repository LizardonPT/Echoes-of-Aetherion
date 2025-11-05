using UnityEngine;
public abstract class SpellPage : MonoBehaviour
{
    [Header("Spell Information")]
    [SerializeField] private string spellName;
    [SerializeField] private Sprite icon;
    [TextArea][SerializeField] private string description;

    [Header("Spell Attributes")]
    [SerializeField] private int manaCost;
    [SerializeField] private float cooldown;

    private float lastCastTime;

    public bool CanCast()
    {
        return Time.time >= lastCastTime + cooldown;
    }

    public void Cast(GameObject caster)
    {
        if (CanCast())
        {
            lastCastTime = Time.time;
            ExecuteSpell(caster);
        }
        else
        {
            Debug.Log($"{spellName} is on cooldown.");
        }
    }
    
    protected abstract void ExecuteSpell(GameObject caster);
}
