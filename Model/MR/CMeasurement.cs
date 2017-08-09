using System;
using System.ComponentModel;

namespace cangun.Model.MR
{
    [Serializable]
    public class CMRMeasurement : INotifyPropertyChanged
    {
        public string _state;
        public string _module;
        public string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                RaisePropertyChanged("Author");
            }
        }
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged("State");
            }
        }
        public string Module
        {
            get { return _module; }
            set
            {
                _module = value;
                RaisePropertyChanged("Module");
            }
        }
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
