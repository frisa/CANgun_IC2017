using System;
using System.ComponentModel;

namespace cangun
{
    [Serializable]
    public class CMessage : INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private int _dlc;
        private string _ecu;
        private byte[] _data;
        private string _comment;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public int Dlc
        {
            get { return _dlc; }
            set { _dlc = value; }
        }
        public string Ecu
        {
            get { return _ecu; }
            set { _ecu = value; }
        }
        public byte[] Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged("Data");
            }
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
