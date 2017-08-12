using System;
using System.ComponentModel;

namespace cangun
{
[Serializable]public class CMRComment : INotifyPropertyChanged
    {
        public string _modul;
        public string _comment;
        public string _author;
        public string Modul
        {
            get { return _modul; }
            set
            {
                _modul = value;
                RaisePropertyChanged("Modul");
            }
        }
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                RaisePropertyChanged("Author");
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
