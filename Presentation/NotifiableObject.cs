using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Presentation
{
    /// <summary>
    /// Class notifiable object which has been extended by all the ViewModels!
    /// </summary>
    public class NotifiableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// updated when property changed
        /// </summary>
        /// <param name="property"> the property name which has neen changed! </param>
        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
