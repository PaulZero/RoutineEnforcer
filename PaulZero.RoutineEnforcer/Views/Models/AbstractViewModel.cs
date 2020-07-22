using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PaulZero.RoutineEnforcer.Views.Models
{
    public class AbstractViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string propertyName] => ValidateProperty(propertyName);

        public string Error => ValidateModel();

        private string[] _validatingPropertyNames;

        protected string ValidateModel()
        {
            foreach (var propertyName in GetValidatingPropertyNames())
            {
                if (ValidateProperty(propertyName) != string.Empty)
                {
                    return "The form is not valid.";
                }
            }

            return string.Empty;
        }

        protected string ValidateProperty(string propertyName)
        {
            var results = new List<ValidationResult>();
            var value = GetPropertyValue(propertyName);
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            if (!Validator.TryValidateProperty(value, context, results))
            {
                return results.First().ErrorMessage;
            }

            return string.Empty;
        }

        protected string[] GetValidatingPropertyNames()
        {
            if (_validatingPropertyNames is null)
            {
                _validatingPropertyNames = GetType()
                    .GetProperties()
                    .Where(p => p.GetCustomAttributes().Any(a => typeof(ValidationAttribute).IsAssignableFrom(a.GetType())))
                    .Select(p => p.Name)
                    .ToArray();
            }

            return _validatingPropertyNames;
        }

        protected object GetPropertyValue(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);

            return property.GetValue(this);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
