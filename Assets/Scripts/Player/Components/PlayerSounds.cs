using UnityEngine;
using UnityEngine.Tilemaps;
using FMODUnity;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private Tilemap grassTilemap;
        [SerializeField] private Tilemap earthTilemap;
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
                RuntimeManager.PlayOneShot(grassFootstepSound, transform.position);
            }
            else if (onEarth)
            {
                RuntimeManager.PlayOneShot(earthFootstepSound, transform.position);
            }
            else
            {
                RuntimeManager.PlayOneShot(earthFootstepSound, transform.position);
            }

        }

    }
}