using UnityEngine;

[RequireComponent(typeof(Grimoire))]
public class SpellInputHandler : MonoBehaviour
{
    private Grimoire grimoire;

    void Start()
    {
        grimoire = GetComponent<Grimoire>();
    }

    void Update()
    {
        // teclas 1–4
        if (Input.GetKeyDown(KeyCode.Alpha1)) grimoire.CastSpell(0, GameObject.Find("Caster"));
        if (Input.GetKeyDown(KeyCode.Alpha2)) grimoire.CastSpell(1, GameObject.Find("Caster"));
        if (Input.GetKeyDown(KeyCode.Alpha3)) grimoire.CastSpell(2, GameObject.Find("Caster"));
        if (Input.GetKeyDown(KeyCode.Alpha4)) grimoire.CastSpell(3, GameObject.Find("Caster"));
    }
}
