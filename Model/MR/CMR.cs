using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace cangun
{ 
    [Serializable]
    public class CMR : INotifyPropertyChanged
    {
        public string _number;
        public string _title;
        public string _content;
        public string _state;
        public string _file;
        public string _project;
        public string _author;
        public ObservableCollection<CMRComment> _comments;
        public ObservableCollection<CMRCcb> _ccb;
        public ObservableCollection<CMRMeasurement> _meas;

        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                RaisePropertyChanged("Number");
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }
        public string Project
        {
            get { return _project; }
            set
            {
                _project = value;
                RaisePropertyChanged("Project");
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
        public string File
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged("File");
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
        public ObservableCollection<CMRComment> Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                RaisePropertyChanged("Comments");
            }
        }
        public ObservableCollection<CMRCcb> CCBs
        {
            get { return _ccb; }
            set
            {
                _ccb = value;
                RaisePropertyChanged("CCBs");
            }
        }
        public ObservableCollection<CMRMeasurement> Measurements
        {
            get { return _meas; }
            set
            {
                _meas = value;
                RaisePropertyChanged("Measurements");
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
