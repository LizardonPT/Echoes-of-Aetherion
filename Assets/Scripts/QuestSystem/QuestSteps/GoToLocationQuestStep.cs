using System;
using EchoesOfEtherion.Game.Locations;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem.QuestSteps
{
    public class GoToLocationQuestStep : QuestStep
    {
        [SerializeField] private string targetLocationName;
        [SerializeField] private LocationType targetLocationType;
        [field: SerializeField] public override string StepDescription { get; protected set; } = "Go to the designated location.";
        public override event Action<int, int> ProgressChanged;
        private void Start()
        {
            LocationController.Instance.LocationEntered += LocationEntered;
            ProgressChanged?.Invoke(0, 1);
        }

        private void LocationEntered(LocationData location)
        {
            bool rightName = location.Name == targetLocationName;
            bool rightType = location.Type == targetLocationType;

            if (rightName && rightType)
            {
                ProgressChanged?.Invoke(1, 1);
                FinishQuestStep();
            }
        }

        public override (int, int) GetProgress()
        {
            return isFinished ? (1, 1) : (0, 1);
        }
    }
}