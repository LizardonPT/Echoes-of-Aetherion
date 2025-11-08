using EchoesOfEtherion.Player.Components;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Game.Locations
{
    public class Location : MonoBehaviour
    {
        [field: SerializeField] public string LocationName { get; private set; }
        [field: SerializeField] public LocationType Type { get; private set; }
        [Header("Activation Method")]
        [SerializeField] private LocationActivationMethod activationMethod = LocationActivationMethod.Trigger;

        [Header("Location Properties")]
        [SerializeField] private EventReference musicTrack;
        [SerializeField] private EventReference ambienceTrack;

        private LocationController locationController;

        private void Start()
        {
            locationController = LocationController.Instance;
            if (activationMethod == LocationActivationMethod.Start)
            {
                Enter();
            }
        }

        public void Enter()
        {
            locationController.EnterLocation(this);
        }

        public void Leave()
        {
            locationController.LeaveLocation(this);
        }

        public LocationData GetLocationData()
        {
            return new LocationData(LocationName, Type, musicTrack, ambienceTrack);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (activationMethod != LocationActivationMethod.Trigger) return;

            if (collision.GetComponent<PlayerController>() != null)
            {
                Enter();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (activationMethod != LocationActivationMethod.Trigger) return;

            if (collision.GetComponent<PlayerController>() != null)
            {
                Leave();
            }
        }

        [ContextMenu("Enter Location Manually")]
        public void EnterManually()
        {
            Enter();
        }

        [ContextMenu("Leave Location Manually")]
        public void LeaveManually()
        {
            Leave();
        }
    }
}
