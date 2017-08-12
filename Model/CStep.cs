using System;
using System.ComponentModel;

namespace cangun
{
    [Serializable]
    public class CStep : INotifyPropertyChanged
    {
        private string _command;
        private CMessageViewModel _message;
        private CSignalViewModel _signal;

        private int _time;
        private int _value;
        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }
        public CMessageViewModel Message
        {
            get { return _message; }
            set
            {
                _message = value;
            }
        }
        public CSignalViewModel Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }
        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public int Value
        {
            get { return _value; }
            set { _value = value; }
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
