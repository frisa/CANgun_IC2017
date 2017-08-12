using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace cangun
{
    [Serializable]
    public class CSequenceViewModel : INotifyPropertyChanged
    {
        private CSequence _sequence;
        ObservableCollection<CStepViewModel> _steps = new ObservableCollection<CStepViewModel>();
        public ObservableCollection<CStepViewModel> Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }
        public CSequence Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
        public string SequenceName
        {
            get { return _sequence.Name; }
            set { _sequence.Name = value; }
        }
        public string SequenceComment
        {
            get { return _sequence.Comment; }
            set { _sequence.Comment = value; }
        }

        public CSequenceViewModel()
        {
            _sequence = new CSequence();
        }
        public ICommand AddStep { get { return new CRelatedCommandNoParam(AddStepExecute, CanAddStepExecute); } }
        void AddStepExecute()
        {
            _steps.Add(new CStepViewModel());
        }
        bool CanAddStepExecute()
        {
            return true;
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
