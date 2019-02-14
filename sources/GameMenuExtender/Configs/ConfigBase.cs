using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class ConfigBase : INotifyPropertyChanged
    {
        internal ConfigManager Manager { get; set; }
        private bool _HasChanged;
        private List<string> _ChangedProperties = new List<string>();
        public bool IsNew { get; set; }
        public bool HasChanged { get => _HasChanged || IsNew; private set => _HasChanged = value; }
        public IList<string> ChangedProperties => _ChangedProperties.AsReadOnly();
        public event PropertyChangedEventHandler PropertyChanged;

        public ConfigBase() { }

        internal ConfigBase(ConfigManager manager)
        {
            Manager = manager;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _ChangedProperties.Add(propertyName);
            HasChanged = true;
        }

        protected void SetPropertyValue<T>(ref T prop, T value, [CallerMemberName] string propertyName = null)
        {
            var comparer = EqualityComparer<T>.Default;
            if (!comparer.Equals(prop, value))
            {
                prop = value;
                OnPropertyChanged(propertyName);
            }
        }

        public virtual void MarkAsSaved()
        {
            IsNew = false;
            HasChanged = false;
            _ChangedProperties.Clear();
        }
    }
}
