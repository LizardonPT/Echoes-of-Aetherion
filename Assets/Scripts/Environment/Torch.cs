using EchoesOfEtherion.Player.Components;
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

    //todo: Make it ignite when it touches a Fire Spell instead of the player.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent<PlayerController>(out _))
        {
            if (isLitten)
                Out();
            else
                Ignite();
        }
    }
}
