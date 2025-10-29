using UnityEngine;
using FMODUnity;
using EchoesOfEtherion.ScriptableObjects.Channels;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private AudioChannel audioChannel;
        [SerializeField] private EventReference leavesFootstepSound;

        //todo: Verify the ground material and then play the corresponding step sound.
        public void PlayFootStepSound()
        {
            audioChannel.PlayOneShot(leavesFootstepSound, transform.position);
        }
    }
}
