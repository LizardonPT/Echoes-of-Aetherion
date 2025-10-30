using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Collider2D))]
public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private EventReference audioEvent;
    [SerializeField] private bool playOnEnter = true;
    [SerializeField] private bool playOnExit = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playOnEnter)
            RuntimeManager.PlayOneShot(audioEvent, transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playOnExit)
            RuntimeManager.PlayOneShot(audioEvent, transform.position);
    }
}
