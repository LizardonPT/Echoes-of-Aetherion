using UnityEngine;
using UnityEngine.Tilemaps;
using FMODUnity;
using EchoesOfEtherion.ScriptableObjects.Channels;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private Tilemap grassTilemap;
        [SerializeField] private Tilemap earthTilemap;
        [SerializeField] private AudioChannel audioChannel;
        [SerializeField] private EventReference grassFootstepSound;
        [SerializeField] private EventReference earthFootstepSound;

        public void PlayFootStepSound()
        {
            Vector3 worldPos = transform.position;

            Vector3Int cellPos = grassTilemap.WorldToCell(worldPos);

            bool onGrass = grassTilemap.GetTile(cellPos) != null;
            bool onEarth = earthTilemap.GetTile(cellPos) != null;

            if (onGrass)
            {
                audioChannel.PlayOneShot(grassFootstepSound, transform.position);
            }
            else if (onEarth)
            {
                audioChannel.PlayOneShot(earthFootstepSound, transform.position);
            }
            else
            {
                audioChannel.PlayOneShot(earthFootstepSound, transform.position);
            }

        }

    }
}