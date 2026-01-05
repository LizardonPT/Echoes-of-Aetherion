using UnityEngine;
using UnityEngine.Tilemaps;
using FMODUnity;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap grassTilemap;
        [SerializeField] private Tilemap earthTilemap;
        [SerializeField] private EventReference grassFootstepSound;
        [SerializeField] private EventReference earthFootstepSound;

        public void PlayFootStepSound()
        {
            Vector3 worldPos = transform.position;

            Vector3Int cellPos = grid.WorldToCell(worldPos);

            bool onGrass = false;
            
            if (grassTilemap != null)
                onGrass = grassTilemap.GetTile(cellPos) != null;

            bool onEarth = false;

            if (earthTilemap != null)
                onEarth = earthTilemap.GetTile(cellPos) != null;

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