using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;

namespace cangun
{
    [Serializable]
    public class CMessageViewModel : INotifyPropertyChanged
    {
        private CMessage _message;
        ObservableCollection<CSignalViewModel> _signals = new ObservableCollection<CSignalViewModel>();
        public ObservableCollection<CSignalViewModel> Signals
        {
            get { return _signals; }
            set { _signals = value; }
        }
        public CMessage Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public CMessageViewModel()
        {
            _message = new CMessage() { Id = 1, Name = "Message", Dlc = 8, Ecu = "Ecu", Data = new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7 } };
        }
        public int MessageId
        {
            get { return _message.Id; }
            set { _message.Id = value; }
        }
        public string MessageName
        {
            get { return _message.Name; }
            set { _message.Name = value; }
        }
        public int MessageDlc
        {
            get { return _message.Dlc; }
            set { _message.Dlc = value; }
        }
        public string MessageEcu
        {
            get { return _message.Ecu; }
            set { _message.Ecu = value; }
        }
        public byte[] MessageData
        {
            get { return _message.Data; }
            set
            {
                _message.Data = value;
                foreach (CSignalViewModel sig in Signals)
                {
                    sig.SignalValue = (int)getSignalValue(_message.Data, sig.SignalStartBit, sig.SignalLenght);
                }
                RaisePropertyChanged("MessageData");
            }
        }
        public string MessageComment
        {
            get { return _message.Comment; }
            set { _message.Comment = value; }
        }
        private UInt64 getSignalValue(byte[] byteMessage, int iStartBit, int iLength)
        {
            BitArray bitsMessage = new BitArray(byteMessage);
            UInt64 ui64SignalValue = 0;
            int iSignalPow = 0;

            bool[] bitsMsg = new bool[64];
            bool[] bitsSgl = new bool[iLength];
            uint[] uiValue = new uint[1];

            bitsMessage.CopyTo(bitsMsg, 0);

            for (int idx = iStartBit; idx < (iStartBit + iLength); idx++)
            {
                bitsSgl[iSignalPow] = bitsMsg[idx];
                if (bitsMsg[idx])
                {
                    ui64SignalValue = ui64SignalValue + Convert.ToUInt64(Math.Pow(2, iSignalPow));
                }
                iSignalPow++;
            }

            return ui64SignalValue;
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
