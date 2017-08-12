using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace cangun
{
    [Serializable]
    public class CStepViewModel : INotifyPropertyChanged
    {
        CStep _step = new CStep();
        ObservableCollection<CSignalViewModel> _signals = new ObservableCollection<CSignalViewModel>();
        public string StepCommand
        {
            get { return _step.Command; }
            set { _step.Command = value; }
        }
        public CMessageViewModel StepMessage
        {
            get { return _step.Message; }
            set
            {
                _step.Message = value;
                _signals = _step.Message.Signals;
                RaisePropertyChanged("StepSignals");
            }
        }
        public ObservableCollection<CSignalViewModel> StepSignals
        {
            get { return _signals; }
            set { _signals = value; }
        }
        public CSignalViewModel StepSignal
        {
            get { return _step.Signal; }
            set
            {
                _step.Signal = value;
            }
        }
        public int StepTime
        {
            get { return _step.Time; }
            set { _step.Time = value; }
        }
        public int StepValue
        {
            get { return _step.Value; }
            set { _step.Value = value; }
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
