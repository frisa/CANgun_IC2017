using System;
using System.ComponentModel;

namespace cangun.Model
{
    [Serializable]
    public class CSignal : INotifyPropertyChanged
    {
        private string _name;
        private int _length;
        private int _startBit;
        private string _intel;
        private string _signed;
        private int _value;
        private string _comment;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Lenght
        {
            get { return _length; }
            set { _length = value; }
        }
        public int StartBit
        {
            get { return _startBit; }
            set { _startBit = value; }
        }
        public string Intel
        {
            get { return _intel; }
            set { _intel = value; }
        }
        public string Signed
        {
            get { return _signed; }
            set { _signed = value; }
        }
        public int Value
        {
            get { return _value; }
            set { _value = value; }
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
