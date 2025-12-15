using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.Spells;
using EchoesOfEtherion.Spells.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Torch : MonoBehaviour
{
    [SerializeField] private UnityEvent OnIgnite;
    [SerializeField] private UnityEvent OnOut;
    private bool isLitten = false;

    public void Ignite()
    {
        if (isLitten) return;
        isLitten = true;
        OnIgnite?.Invoke();
    }

    public void Out()
    {
        if (!isLitten) return;
        isLitten = false;
        OnOut?.Invoke();
    }

    //todo: Make it more generic.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent(out ProjectileSpellRuntime spell))
        {
            if (spell.SpellInfo.SpellElement == SpellElement.Fire)
            {
                if (isLitten)
                    Out();
                else
                    Ignite();
            }
        }
    }
}
