using System;
using System.ComponentModel;
using System.Data;

namespace cangun.Model
{
    [Serializable]
    public class CProject : INotifyPropertyChanged
    {
        public string _name;
        public string _pathDbc;
        public string _description;
        public string _pathMRDB;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public string PathDbc
        {
            get { return _pathDbc; }
            set
            {
                _pathDbc = value;
                RaisePropertyChanged("PathDbc");
            }
        }
        public string PathMRDB
        {
            get { return _pathMRDB; }
            set
            {
                _pathMRDB = value;
                RaisePropertyChanged("PathMRDB");
            }
        }
        public DataTable dtMRs;
        public DataTable dtMRsFav;

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
