using PaulZero.RoutineEnforcer.Models;
using System.ComponentModel;
using System.Reflection;

namespace PaulZero.RoutineEnforcer.Views.Models
{
    public class EventActionTypeViewModel
    {
        public EventActionType ActionType { get; }

        public string Description { get; }

        public EventActionTypeViewModel(EventActionType actionType)
        {
            ActionType = actionType;
            Description = ExtractDescription(actionType);
        }

        private string ExtractDescription(EventActionType actionType)
        {
            return actionType.GetType().GetField(actionType.ToString()).GetCustomAttribute<DescriptionAttribute>().Description;
        }
    }
}
