using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.Game.Locations
{
    public class LocationController : Singleton<LocationController>
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        public event Action<LocationData> LocationEntered;
        public event Action<LocationData> LocationLeft;
        public event Action<string> RegionChanged; // When entering a new region
        public event Action<string> AreaEntered;   // When entering an area within region

        private LocationData currentLocation;
        private string currentRegion;
        private readonly Stack<LocationData> locationHistory = new();
        
        public void EnterLocation(Location location)
        {
            var locationData = location.GetLocationData();
            Log($"Entering location: {locationData.Name} of type {locationData.Type}");
            // Store previous location
            if (!string.IsNullOrEmpty(currentLocation.Name))
            {
                locationHistory.Push(currentLocation);
            }

            // Update current location
            currentLocation = locationData;

            // Handle region changes
            if (locationData.Type == LocationType.Region && currentRegion != locationData.Name)
            {
                string previousRegion = currentRegion;
                currentRegion = locationData.Name;
                RegionChanged?.Invoke(currentRegion);
            }

            // Handle area within region
            if (locationData.Type == LocationType.Area)
            {
                AreaEntered?.Invoke(locationData.Name);
            }

            // Notify listeners
            LocationEntered?.Invoke(locationData);
        }

        public void LeaveLocation(Location location)
        {
            Log($"Leaving location: {location.LocationName} of type {location.Type}");
            var locationData = location.GetLocationData();
            LocationLeft?.Invoke(locationData);
        }

        public LocationData GetCurrentLocation()
        {
            return currentLocation;
        }

        public string GetCurrentRegion()
        {
            return currentRegion;
        }

        public bool TryRevertToPreviousLocation()
        {
            if (locationHistory.Count > 0)
            {
                currentLocation = locationHistory.Pop();
                LocationEntered?.Invoke(currentLocation);
                Log($"Reverted to previous location: {currentLocation.Name} of type {currentLocation.Type}");
                return true;
            }
            Log("No previous location to revert to.");
            return false;
        }

        private void Log(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[LocationController] {message}");
            }
        }
    }
}
