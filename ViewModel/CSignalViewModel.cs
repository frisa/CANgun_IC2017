using cangun.Model;
using System;
using System.ComponentModel;

namespace cangun.ViewModel
{
    [Serializable]
    public class CSignalViewModel : INotifyPropertyChanged
    {
        private CSignal _signal;
        public CSignal Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }
        public CSignalViewModel()
        {
            _signal = new CSignal();
        }
        public string SignalName
        {
            get { return _signal.Name; }
            set { _signal.Name = value; }
        }
        public int SignalLenght
        {
            get { return _signal.Lenght; }
            set { _signal.Lenght = value; }
        }
        public int SignalStartBit
        {
            get { return _signal.StartBit; }
            set { _signal.StartBit = value; }
        }
        public string SignalIntel
        {
            get { return _signal.Intel; }
            set { _signal.Intel = value; }
        }
        public string SignalSigned
        {
            get { return _signal.Signed; }
            set { _signal.Signed = value; }
        }
        public int SignalValue
        {
            get { return _signal.Value; }
            set
            {
                _signal.Value = value;
                RaisePropertyChanged("SignalValue");
            }
        }
        public string SignalComment
        {
            get { return _signal.Comment; }
            set { _signal.Comment = value; }
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
