using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.Game.Locations
{
    public class LocationController : Singleton<LocationController>
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        public event Action<LocationData> OnLocationEntered;
        public event Action<LocationData> OnLocationLeft;
        public event Action<string> OnRegionChanged; // When entering a new region
        public event Action<string> OnAreaEntered;   // When entering an area within region

        private LocationData currentLocation;
        private string currentRegion;
        private readonly Stack<LocationData> locationHistory = new();
        
        protected override void Initialize()
        {
            Log("Initialized successfully");
        }

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
                OnRegionChanged?.Invoke(currentRegion);
            }

            // Handle area within region
            if (locationData.Type == LocationType.Area)
            {
                OnAreaEntered?.Invoke(locationData.Name);
            }

            // Notify listeners
            OnLocationEntered?.Invoke(locationData);
        }

        public void LeaveLocation(Location location)
        {
            Log($"Leaving location: {location.LocationName} of type {location.Type}");
            var locationData = location.GetLocationData();
            OnLocationLeft?.Invoke(locationData);
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
                OnLocationEntered?.Invoke(currentLocation);
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
