using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InitiativeTracker.Data
{
    /// <summary>
    /// Any class that needs to have some sort of meaningful visual representation in the application
    /// should inherit from this one.
    /// </summary>
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method uses black magic (System.Runtime.CompilerServices) to raise the PropertyChanged
        /// event that WPF listens to in order to update any bound components.
        /// </summary>
        /// <typeparam name="T">The type of the object backing the property.</typeparam>
        /// <param name="field">A reference to the backing object.</param>
        /// <param name="newValue">The new value being assigned to the background object.</param>
        /// <param name="propertyName">You generally don't need to specify a third argument! If you call this function
        /// from a property, propertyName magically becomes the name of the property you called it from.
        /// Such power is highly dangerous — use with caution.</param>
        /// <returns>true if the field was actually changed (and thus an event raised), false otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            /*
             * if the new value is equal to the old value, there's no point in updating anything.
             * note that this will default to comparing object reference (Object.ReferenceEquals())
             * if the class doesn't define an Equals() override. but, using Equals makes the code
             * work as expected for strings and most of C#'s inbuilt classes that have overridden
             * the method to compare object data, not reference. 
             */
            if (Object.Equals(field, newValue)) return false;
            else
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
        }

        /// <summary>
        /// This mundane method just raises the PropertyChanged event on the given property name, thus
        /// forcing WPF to update bindings regardless of if the property actually changed or not.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}