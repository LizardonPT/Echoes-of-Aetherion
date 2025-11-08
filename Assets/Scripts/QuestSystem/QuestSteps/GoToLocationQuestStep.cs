using EchoesOfEtherion.Game.Locations;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem.QuestSteps
{
    public class GoToLocationQuestStep : QuestStep
    {
        [SerializeField] private string targetLocationName;
        [SerializeField] private LocationType targetLocationType;

        private void Start()
        {
            LocationController.Instance.OnLocationEntered += LocationEntered;
        }

        private void LocationEntered(LocationData location)
        {
            bool rightName = location.Name == targetLocationName;
            bool rightType = location.Type == targetLocationType;
            
            if (rightName && rightType)
            {
                FinishQuestStep();
            }
        }
    }
}