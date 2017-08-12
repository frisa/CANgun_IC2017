using System;
using System.ComponentModel;

namespace cangun
{
    [Serializable]
    public class CSentMessageViewModel : INotifyPropertyChanged
    {
        private int _startTime = 0;
        private int _stopTime = 0;
        private CMessageViewModel _message;
        private CSignalViewModel _signal;
        public int StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        public int StopTime
        {
            get { return _stopTime; }
            set { _stopTime = value; }
        }
        public CMessageViewModel Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public CSignalViewModel Signal
        {
            get { return _signal; }
            set { _signal = value; }
        }

        /// <summary>
        /// The message which has been sent ot the target, so the time stamp needs to be logged
        /// </summary>
        /// <param name="message"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        public CSentMessageViewModel(CMessageViewModel message, int start, int stop)
        {
            _message = new CMessageViewModel() { MessageName = message.MessageName, MessageId = message.MessageId, MessageData = message.MessageData, MessageDlc = message.MessageDlc };
            _startTime = start;
            _stopTime = stop;
        }

        public CSentMessageViewModel(CStepViewModel step)
        {
            _message = step.StepMessage;
            _signal = step.StepSignal;
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
