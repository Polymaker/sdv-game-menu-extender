using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class ConfigBase : INotifyPropertyChanged
    {
        private bool _HasChanged;

        public bool IsNew { get; set; }
        public bool HasChanged { get => _HasChanged || IsNew; private set => _HasChanged = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            HasChanged = true;
        }

        public virtual void MarkAsSaved()
        {
            IsNew = false;
            HasChanged = false;
        }
    }
}
