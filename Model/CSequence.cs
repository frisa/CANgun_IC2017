using System;
using System.ComponentModel;

namespace cangun.Model
{
    [Serializable]
    public class CSequence : INotifyPropertyChanged
    {
        private string _name;
        private string _comment;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;
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
