using FMODUnity;
using UnityEngine.EventSystems;

namespace EchoesOfEtherion.Game.Locations
{
    [System.Serializable]
    public struct LocationData
    {
        public string Name;
        public LocationType Type;
        public EventReference MusicTrack;
        public EventReference AmbienceTrack;
        public float GlobalLightIntensity;
        public LocationData(string name, LocationType type, float globalLightIntensity, EventReference musicTrack, EventReference ambienceTrack)
        {
            Name = name;
            Type = type;
            GlobalLightIntensity = globalLightIntensity;
            MusicTrack = musicTrack;
            AmbienceTrack = ambienceTrack;
        }
    }
}
