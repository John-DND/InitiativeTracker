using System;
using System.Globalization;
using System.Windows.Controls;

namespace InitiativeTracker.Data.Util
{
    public enum PropertyType
    {
        Name,
        Health, 
        ArmorClass,
        Dexterity,
        DieSides,
        DieCount,
        EntityCount
    }

    /// <summary>
    /// Validate is called when the user attempts to create or edit an
    /// entity. It ensures the values inputted are valid. This is a standard
    /// WPF design paradigm.
    /// </summary>
    public class EntityValidator : ValidationRule
    {
        public PropertyType Type { get; set; }

        public EntityValidator() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (Type)
            {
                case PropertyType.Name:
                    string name = (string) value;
                    if (name.Length > 24 || name.Length == 0) return new ValidationResult(false, "Name must be shorter than 24 characters but longer than 0.");
                    else return new ValidationResult(true, "Valid.");
                case PropertyType.Health:
                case PropertyType.ArmorClass:
                case PropertyType.Dexterity:
                    string dataString = (string) value;

                    if (Int32.TryParse(dataString, out int result))
                    {
                        if (result < 0) return new ValidationResult(false, "Input must be positive.");
                        return new ValidationResult(true, "Valid.");
                    }
                    else return new ValidationResult(false, "Input must be a valid integer.");
                case PropertyType.DieCount:
                case PropertyType.DieSides:
                    string dieString = (string) value;
                    if (Int32.TryParse(dieString, out int dieStringResult))
                    {
                        if (dieStringResult <= 0) return new ValidationResult(false, "Input must be a valid, positive, non-zero integer.");
                        return new ValidationResult(true, "Valid.");
                    }
                    else return new ValidationResult(false, "Input must be a valid integer.");
                case PropertyType.EntityCount:
                    string countString = (string)value;
                    if (Int32.TryParse(countString, out int countStringResult))
                    {
                        if (countStringResult >= 1 && countStringResult <= 10000)
                        {
                            return new ValidationResult(true, "Valid.");
                        }
                        else return new ValidationResult(false, "Input must be between 1 and 10000, inclusive.");
                    }
                    else return new ValidationResult(false, "Input must be a valid integer.");
                default:
                    /*
                     * this can never be called, but it's necessary for the compiler as it does not know that the
                     * PropertyType enum is ALWAYS going to be limited to these values
                     */
                    return new ValidationResult(false, "Unknown validation error.");
            }
        }
    }
}
