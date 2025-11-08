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

        public LocationData(string name, LocationType type, EventReference musicTrack, EventReference ambienceTrack)
        {
            Name = name;
            Type = type;
            MusicTrack = musicTrack;
            AmbienceTrack = ambienceTrack;
        }
    }
}
