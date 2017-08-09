﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using vxlapi_NET20;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Threading;
using cangun.Model;

namespace cangun.ViewModel
{
    [Serializable]
    public class CSolutionViewModel : INotifyPropertyChanged
    {
        private string _path;
        private string _description;
        private int _currentTime;
        private int _currentObservationTime;
        private int _clkTimeStart;
        private int _clkTime;
        private bool _clkActive;
        private string _log;
        private ObservableCollection<CSentMessageViewModel> _currentSendetMessages = new ObservableCollection<CSentMessageViewModel>();
        private CSequenceViewModel _currentSequenceViewModel;
        private CProjectViewModel _currentProjectViewModel;
        ObservableCollection<CProjectViewModel> _projects = new ObservableCollection<CProjectViewModel>();

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged("Path");
            }
        }
        public string SolutionDescription
        {
            get { return _description; }
            set { _description = value; }
        }
        public string SolutionLog
        {
            get { return _log; }
            set { _log = value; }
        }
        public int ClkTime
        {
            get { return _clkTime; }
            set
            {
                _clkTime = value;
                RaisePropertyChanged("ClkTime");
            }
        }
        public bool ClkActive
        {
            get { return _clkActive; }
            set
            {
                _clkActive = value;
                if (_clkActive)
                    _clkTimeStart = _currentTime;
                RaisePropertyChanged("ClkActive");
            }
        }

        public void log(string message)
        {
            SolutionLog = SolutionLog + message + "\n";
            RaisePropertyChanged("SolutionLog");
        }
        public ObservableCollection<CProjectViewModel> Projects
        {
            get
            {
                return _projects;
            }
            set
            {
                _projects = value;
                RaisePropertyChanged("Projects");
            }
        }
        ObservableCollection<String> _commands = new ObservableCollection<String>();
        ObservableCollection<int> _times = new ObservableCollection<int>();
        ObservableCollection<int> _values = new ObservableCollection<int>();
        public ObservableCollection<String> Commands
        {
            get { return _commands; }
            set { _commands = value; }
        }
        public ObservableCollection<int> Times
        {
            get { return _times; }
            set { _times = value; }
        }
        public ObservableCollection<int> Values
        {
            get { return _values; }
            set { _values = value; }
        }
        public int SolutionCurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                if (_clkActive) ClkTime = _currentTime - _clkTimeStart;
                RaisePropertyChanged("SolutionCurrentTime");
            }
        }
        public int SolutionCurrentObservationTime
        {
            get { return _currentObservationTime; }
            set
            {
                _currentObservationTime = value;
                RaisePropertyChanged("SolutionCurrentObservationTime");
            }
        }
        public CSequenceViewModel SolutionCurrentSequence
        {
            get { return _currentSequenceViewModel; }
            set { _currentSequenceViewModel = value; }
        }
        public CProjectViewModel SolutionCurrentProject
        {
            get { return _currentProjectViewModel; }
            set { _currentProjectViewModel = value; }
        }
        public ObservableCollection<CSentMessageViewModel> CurrentSendetMessages
        {
            get { return _currentSendetMessages; }
            set { _currentSendetMessages = value; }
        }
        private BackgroundWorker bwTx = new BackgroundWorker();
        private BackgroundWorker bwRx = new BackgroundWorker();

        public CSolutionViewModel()
        {
            _projects.Add(new CProjectViewModel { ProjectName = "Gen4" });
            _projects.Add(new CProjectViewModel { ProjectName = "Gen31" });
            //_projects.Add(new CProjectViewModel { ProjectName = "Roadmap PQ36" });
            //_projects.Add(new CProjectViewModel { ProjectName = "Roadmap AU210" });
            //_projects.Add(new CProjectViewModel { ProjectName = "Roadmap AU316" });

            _commands.Add("Set");
            _commands.Add("Set with ASR");
            _commands.Add("Step 500ms");
            _commands.Add("Timeout");
            _commands.Add("Break");
            _commands.Add("Restart");

            for (int idx = 0; idx < 100; idx++)
            {
                _times.Add(idx * 1000);
            }

            for (int idx = 0; idx < 21; idx++)
            {
                _values.Add(idx);
            }

            _values.Add(50);
            _values.Add(100);
            _values.Add(1000);
            _values.Add(2000);
            _values.Add(3000);
            _values.Add(4000);
            _values.Add(5000);
            _values.Add(10000);

            _currentTime = 0;
        }
        public ICommand Start { get { return new CRelatedCommandNoParam(StartExecute, CanStartExecute); } }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(int handle, int timeOut);

        private XLDriver _xlDriverTx = new XLDriver();
        private int _iEventHandleTx = -1;
        private int _iPortHandleTx = -1;
        UInt64 _ui64TransmitMaskTx = 0;

        private XLDriver _xlDriverRx = new XLDriver();
        private int _iEventHandleRx = -1;
        private int _iPortHandleRx = -1;
        UInt64 _ui64TransmitMaskRx = 0;

        public void xlStartCan(ref int _iEventHandle, ref int _iPortHandle, ref UInt64 _ui64TransmitMask)
        {
            UInt64 ui64AccessMask = 0;
            UInt64 ui64PermissionMask = 0;
            uint uiHWType = 0;
            uint uiHWIndex = 0;
            uint uiHWChannel = 0;
            uint uiBusType = (uint)XLClass.XLbusTypes.XL_BUS_TYPE_CAN;
            uint uiFlags = 0;
            XLClass.XLstatus xlRet;

            if ((_xlDriverTx.XL_GetApplConfig("xlCANSimulator", 0, ref uiHWType, ref uiHWIndex, ref uiHWChannel, uiBusType) != XLClass.XLstatus.XL_SUCCESS) ||
                (_xlDriverTx.XL_GetApplConfig("xlCANSimulator", 1, ref uiHWType, ref uiHWIndex, ref uiHWChannel, uiBusType) != XLClass.XLstatus.XL_SUCCESS))
            {
                _xlDriverTx.XL_SetApplConfig("xlCANSimulator", 0, 0, 0, 0, 0);
                _xlDriverTx.XL_SetApplConfig("xlCANSimulator", 1, 0, 0, 0, 0);
            }
            log("DRIVER-Open xlCANSimulator configuration");
            _xlDriverTx.XL_GetApplConfig("xlCANSimulator", 0, ref uiHWType, ref uiHWIndex, ref uiHWChannel, uiBusType);
            ui64AccessMask |= _xlDriverTx.XL_GetChannelMask((int)uiHWType, (int)uiHWIndex, (int)uiHWChannel);

            ui64PermissionMask = ui64AccessMask;
            _ui64TransmitMaskTx = ui64AccessMask;

            xlRet = _xlDriverTx.XL_OpenPort(ref _iPortHandle, "xlCANSimulator", ui64AccessMask, ref ui64PermissionMask, (uint)1024, uiBusType);
            if (XLClass.XLstatus.XL_SUCCESS != xlRet)
                log("DRIVER-Open port failed");
            else
                log("DRIVER-Open port success");

            xlRet = _xlDriverTx.XL_CanRequestChipState(_iPortHandle, ui64AccessMask);
            if (XLClass.XLstatus.XL_SUCCESS != xlRet)
                log("DRIVER-Request chip state failed");
            else
                log("DRIVER-Request chip state success");

            xlRet = _xlDriverTx.XL_ActivateChannel(_iPortHandle, ui64AccessMask, uiBusType, uiFlags);
            if (XLClass.XLstatus.XL_SUCCESS != xlRet)
                log("DRIVER-Activate channel failed");
            else
                log("DRIVER-Activate channel success");

            xlRet = _xlDriverTx.XL_SetNotification(_iPortHandle, ref _iEventHandle, 1);
            if (XLClass.XLstatus.XL_SUCCESS != xlRet)
                log("DRIVER-Set notificaiton failed");
            else
                log("DRIVER-Set notificaiton success");

            xlRet = _xlDriverTx.XL_ResetClock(_iPortHandle);
            if (XLClass.XLstatus.XL_SUCCESS != xlRet)
                log("DRIVER-Reset clock failed");
            else
                log("DRIVER-Reset clock success");
        }
        private byte[] getMessageValue(byte[] byteMessage, int value, int startBit, int length)
        {
            BitArray bitsMessage = new BitArray(byteMessage);
            BitArray bitsValue = new BitArray(BitConverter.GetBytes(value));
            byte[] byteMessageOut = new byte[byteMessage.Length];
            for (int idx = startBit; idx < (startBit + length); idx++)
            {
                bitsMessage[idx] = bitsValue[idx - startBit];
            }
            bitsMessage.CopyTo(byteMessageOut, 0);
            return byteMessageOut;
        }

        private void bwTx_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dictionary<int, CSentMessageViewModel> dicMainCurrentSendetMessages = (Dictionary<int, CSentMessageViewModel>)e.UserState;
            SolutionCurrentTime = (int)e.ProgressPercentage;
            foreach (KeyValuePair<int, CSentMessageViewModel> msg in dicMainCurrentSendetMessages)
            {
                if ((SolutionCurrentTime == msg.Value.StartTime) || (SolutionCurrentTime == msg.Value.StopTime))
                {
                    CurrentSendetMessages.Add(new CSentMessageViewModel(msg.Value.Message, msg.Value.StartTime, msg.Value.StopTime));
                }

            }
        }

        void StartExecute()
        {
            CurrentSendetMessages.Clear();
            bwTx.WorkerReportsProgress = true;
            bwTx.WorkerSupportsCancellation = true;
            bwTx.DoWork += delegate (object s, DoWorkEventArgs args)
            {
                CSequenceViewModel sequence = (CSequenceViewModel)args.Argument;
                Dictionary<int, CSentMessageViewModel> dicCurrentSendetMessages = new Dictionary<int, CSentMessageViewModel>();
                XLClass.xl_can_message xlMsg = new XLClass.xl_can_message();
                XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(0);
                XLClass.xl_event xlEvent;
                int time;
                time = 0;
                xlStartCan(ref _iEventHandleTx, ref _iPortHandleTx, ref _ui64TransmitMaskTx);

                while (true)
                {
                    if (bwTx.CancellationPending)
                    {
                        _xlDriverTx.XL_ClosePort(_iPortHandleTx);
                        _xlDriverTx.XL_CloseDriver();
                        args.Cancel = true;
                        break;
                    }
                    else
                    {
                        foreach (CStepViewModel step in sequence.Steps)
                        {
                            if (time == step.StepTime)
                            {
                                switch (step.StepCommand)
                                {
                                    case "Set":
                                        {
                                            if (!dicCurrentSendetMessages.ContainsKey(step.StepMessage.MessageId))
                                                dicCurrentSendetMessages.Add(step.StepMessage.MessageId, new CSentMessageViewModel(step));
                                            dicCurrentSendetMessages[step.StepMessage.MessageId].Message.MessageData = getMessageValue(step.StepMessage.MessageData, step.StepValue, step.StepSignal.SignalStartBit, step.StepSignal.SignalLenght);
                                            dicCurrentSendetMessages[step.StepMessage.MessageId].StartTime = Convert.ToInt32(time);
                                            dicCurrentSendetMessages[step.StepMessage.MessageId].StopTime = Int32.MaxValue;
                                            break;
                                        }
                                    case "Set with ASR":
                                        {

                                            break;
                                        }

                                    case "Step 500ms":
                                        {
                                            if (!dicCurrentSendetMessages.ContainsKey(step.StepMessage.MessageId))
                                                dicCurrentSendetMessages.Add(step.StepMessage.MessageId, new CSentMessageViewModel(step));
                                            dicCurrentSendetMessages[step.StepMessage.MessageId].Message.MessageData = getMessageValue(step.StepMessage.MessageData, step.StepValue, step.StepSignal.SignalStartBit, step.StepSignal.SignalLenght);
                                            dicCurrentSendetMessages[step.StepMessage.MessageId].StartTime = time;
                                            dicCurrentSendetMessages[step.StepMessage.MessageId].StopTime = time + 500;
                                            break;
                                        }
                                    case "Timeout":
                                        {
                                            if (dicCurrentSendetMessages.ContainsKey(step.StepMessage.MessageId))
                                                dicCurrentSendetMessages.Remove(step.StepMessage.MessageId);
                                            break;
                                        }
                                    case "Break":
                                        {
                                            if (MessageBox.Show(string.Format("Breaked sequence {0}, do you want to continue?", this.SolutionCurrentSequence.SequenceName), "Break", MessageBoxButton.YesNo, MessageBoxImage.Hand) != MessageBoxResult.Yes)
                                                return;
                                            break;
                                        }
                                    case "Restart":
                                        {
                                            //RestartExecute();
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                            }
                        }
                        foreach (KeyValuePair<int, CSentMessageViewModel> msg in dicCurrentSendetMessages)
                        {
                            if (msg.Value.StopTime == time)
                            {
                                msg.Value.Message.MessageData = getMessageValue(msg.Value.Message.MessageData, 0, msg.Value.Signal.SignalStartBit, msg.Value.Signal.SignalLenght);
                            }
                        }
                        xlEventCollection.xlEvent.Clear();
                        foreach (KeyValuePair<int, CSentMessageViewModel> msg in dicCurrentSendetMessages)
                        {
                            xlEvent = new XLClass.xl_event();
                            xlEvent.tagData.can_Msg.id = (uint)msg.Value.Message.MessageId;
                            xlEvent.tagData.can_Msg.dlc = (ushort)msg.Value.Message.MessageDlc;
                            xlEvent.tagData.can_Msg.data = msg.Value.Message.MessageData;
                            xlEvent.tag = (byte)XLClass.XLeventType.XL_TRANSMIT_MSG;
                            xlEventCollection.xlEvent.Add(xlEvent);
                        }
                        xlEventCollection.messageCount = (uint)xlEventCollection.xlEvent.Count;
                        if (xlEventCollection.messageCount > 0)
                            _xlDriverTx.XL_CanTransmit(_iPortHandleTx, _ui64TransmitMaskTx, xlEventCollection);
                    }
                    bwTx.ReportProgress(time++, dicCurrentSendetMessages);
                    Thread.Sleep(1);
                }
            };
            bwTx.ProgressChanged += bwTx_ProgressChanged;
            CurrentSendetMessages.Clear();
            bwTx.RunWorkerAsync(this._currentSequenceViewModel);
        }
        bool CanStartExecute()
        {
            return ((_currentSequenceViewModel != null) && !bwTx.IsBusy);
        }
        public ICommand Stop { get { return new CRelatedCommandNoParam(StopExecute, CanStopExecute); } }
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
        void StopExecute()
        {
            if (bwTx.IsBusy)
            {
                bwTx.CancelAsync();
                bwTx.ProgressChanged -= bwTx_ProgressChanged;
            }
            while (bwTx.IsBusy)
            {
                DoEvents();
            }
        }
        bool CanStopExecute()
        {
            return bwTx.WorkerSupportsCancellation && bwTx.IsBusy;
        }
        public ICommand Restart { get { return new CRelatedCommandNoParam(RestartExecute, CanRestartExecute); } }
        void RestartExecute()
        {
            StopExecute();
            StartExecute();
        }
        bool CanRestartExecute()
        {
            return true;
        }

        public ICommand ClkStart { get { return new CRelatedCommandNoParam(ClkStartExecute, CanClkStartExecute); } }
        void ClkStartExecute()
        {
            ClkActive = true;
        }
        bool CanClkStartExecute()
        {
            return !ClkActive;
        }

        public ICommand ClkStop { get { return new CRelatedCommandNoParam(ClkStopExecute, CanClkStopExecute); } }
        void ClkStopExecute()
        {
            ClkActive = false;
        }
        bool CanClkStopExecute()
        {
            return ClkActive;
        }

        public ICommand Insert { get { return new CRelatedCommandParam(InsertExecute, CanInsertExecute); } }
        void InsertExecute(object index)
        {
            SolutionCurrentSequence.Steps.Insert((int)index, new CStepViewModel());
        }
        bool CanInsertExecute()
        {
            if (SolutionCurrentSequence != null)
            {
                return SolutionCurrentSequence.Steps.Count > 0;
            }
            return false;
        }

        public ICommand Observe { get { return new CRelatedCommandNoParam(ObserveExecute, CanObserveExecute); } }
        private enum WaitResults : int
        {
            WAIT_OBJECT_0 = 0x0,
            WAIT_ABANDONED = 0x80,
            WAIT_TIMEOUT = 0x102,
            INFINITE = 0xFFFF,
            WAIT_FAILED = 0xFFFFFFF
        }
        void ObserveExecute()
        {

            bwRx.WorkerReportsProgress = true;
            bwRx.WorkerSupportsCancellation = true;
            bwRx.DoWork += delegate (object s, DoWorkEventArgs args)
            {
                int time;
                time = 0;
                xlStartCan(ref _iEventHandleRx, ref _iPortHandleRx, ref _ui64TransmitMaskRx);
                while (true)
                {
                    if (bwRx.CancellationPending)
                    {
                        args.Cancel = true;
                        log("Observation stopped");
                        break;
                    }
                    else
                    {
                        XLClass.xl_event xlReceivedEvent = new XLClass.xl_event();
                        XLClass.XLstatus xlStatus = XLClass.XLstatus.XL_SUCCESS;
                        WaitResults lbWaitResult = new WaitResults();
                        lbWaitResult = (WaitResults)WaitForSingleObject(_iEventHandleRx, 1000);
                        if (lbWaitResult != WaitResults.WAIT_TIMEOUT)
                        {
                            xlStatus = XLClass.XLstatus.XL_SUCCESS;
                            while (xlStatus != XLClass.XLstatus.XL_ERR_QUEUE_IS_EMPTY)
                            {
                                xlStatus = _xlDriverRx.XL_Receive(_iPortHandleRx, ref xlReceivedEvent);
                                if (xlStatus == XLClass.XLstatus.XL_SUCCESS)
                                {
                                    if ((xlReceivedEvent.flags & (byte)XLClass.XLeventFlags.XL_EVENT_FLAG_OVERRUN) != 0)
                                    {
                                        //throw new VectorCANcaseXLException("XL_EVENT_FLAG_OVERRUN");
                                    }
                                    if (xlReceivedEvent.tag == (byte)XLClass.XLeventType.XL_RECEIVE_MSG)
                                    {
                                        if ((xlReceivedEvent.tagData.can_Msg.flags & (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_OVERRUN) != 0)
                                        {
                                            //throw new VectorCANcaseXLException("XL_CAN_MSG_FLAG_OVERRUN");
                                        }
                                        if ((xlReceivedEvent.tagData.can_Msg.flags & (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME) == (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                                        {
                                            //throw new VectorCANcaseXLException("ERROR FRAME");
                                        }
                                        else if ((xlReceivedEvent.tagData.can_Msg.flags & (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME) == (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                                        {
                                            //throw new VectorCANcaseXLException("REMOTE FRAME");
                                        }
                                        else if (
                                            ((xlReceivedEvent.tagData.can_Msg.flags & (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_NERR) == 0) &&
                                            ((xlReceivedEvent.tagData.can_Msg.flags & (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_TX_COMPLETED) == 0) &&
                                            ((xlReceivedEvent.tagData.can_Msg.flags & (ushort)XLClass.XLmessageFlags.XL_CAN_MSG_FLAG_TX_REQUEST) == 0)
                                            )
                                        {
                                            foreach (CMessageViewModel msg in SolutionCurrentProject.Messages)
                                            {
                                                if (msg.MessageId == xlReceivedEvent.tagData.can_Msg.id)
                                                    msg.MessageData = xlReceivedEvent.tagData.can_Msg.data;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        bwRx.ReportProgress(1, ++time);
                    }
                    Thread.Sleep(1);
                }
            };
            bwRx.ProgressChanged += delegate (object s, ProgressChangedEventArgs args)
            {
                SolutionCurrentObservationTime = (int)args.UserState;
                //SolutionCurrentProject.RaisePropertyChanged("Messages");

            };
            bwRx.RunWorkerAsync();
        }
        bool CanObserveExecute()
        {
            return !bwRx.IsBusy;
        }
        public ICommand ObserveStop { get { return new CRelatedCommandNoParam(ObserveStopExecute, CanObserveStopExecute); } }
        void ObserveStopExecute()
        {
            bwRx.CancelAsync();
        }
        bool CanObserveStopExecute()
        {
            return bwRx.WorkerSupportsCancellation && bwRx.IsBusy;
        }

        public ICommand SendMessage { get { return new CRelatedCommandParam(SendMflMessageExecute, CanSendMflMessageExecute); } }
        void SendMflMessageExecute(object IdDlcStartBitLengthValueInterval)
        {
            string[] parameters = ((String)IdDlcStartBitLengthValueInterval).Split('_');

            UInt16 id = Convert.ToUInt16(parameters[0]);
            UInt16 dlc = Convert.ToUInt16(parameters[1]);
            UInt16 length = Convert.ToUInt16(parameters[2]);
            int startBit = Convert.ToUInt16(parameters[3]);
            int value = Convert.ToInt32(parameters[4]);
            int interval = Convert.ToInt32(parameters[5]);

            int _iEventHandleLocal = -1;
            int _iPortHandleLocal = -1;
            UInt64 _ui64TransmitMaskLocal = 0;
            xlStartCan(ref _iEventHandleLocal, ref _iPortHandleLocal, ref _ui64TransmitMaskLocal);
            //XLClass.xl_can_message xlMsg = new XLClass.xl_can_message();
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(0);
            XLClass.xl_event xlEvent;
            xlEventCollection.xlEvent.Clear();
            xlEvent = new XLClass.xl_event();
            xlEvent.tagData.can_Msg.id = id;
            xlEvent.tagData.can_Msg.dlc = dlc;
            xlEvent.tagData.can_Msg.data = getMessageValue(new byte[dlc], value, startBit, length);
            xlEvent.tag = (byte)XLClass.XLeventType.XL_TRANSMIT_MSG;
            xlEventCollection.xlEvent.Add(xlEvent);
            xlEventCollection.messageCount = (uint)xlEventCollection.xlEvent.Count;

            for (int idx = 0; idx < interval; idx++)
            {
                _xlDriverTx.XL_CanTransmit(_iPortHandleLocal, /* _ui64TransmitMaskLocal*/1, xlEventCollection);
                Thread.Sleep(1);
            }
            xlEventCollection.xlEvent.Clear();
            xlEvent.tagData.can_Msg.data = xlEvent.tagData.can_Msg.data = getMessageValue(new byte[dlc], 0, startBit, length);
            xlEventCollection.xlEvent.Add(xlEvent);
            _xlDriverTx.XL_CanTransmit(_iPortHandleLocal,  /* _ui64TransmitMaskLocal*/ 1, xlEventCollection);

            _xlDriverTx.XL_ClosePort(_iPortHandleLocal);
            _xlDriverTx.XL_CloseDriver();
        }
        bool CanSendMflMessageExecute()
        {
            return true;
        }

        public ICommand AddProject { get { return new CRelatedCommandParam(AddProjectExecute, CanAddProjectExecute); } }
        void AddProjectExecute(object name)
        {
            if (!String.IsNullOrEmpty((String)name))
            {
                Projects.Add(new CProjectViewModel { ProjectName = (String)name });
            }
            else
            {
                MessageBox.Show("Add the name of project to the textbox next to the button");
            }
        }
        bool CanAddProjectExecute()
        {
            return true;
        }
        public ICommand LoadSolution { get { return new CRelatedCommandNoParam(LoadSolutionExecute, CanLoadSolutionExecute); } }
        void LoadSolutionExecute()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".gun";
                dlg.Filter = "GUN Files (*.gun)|*.gun";
                if (dlg.ShowDialog() == true)
                {
                    String sPath = string.Empty;
                    sPath = dlg.FileName;
                    CProject ieProject = new CProject();
                    BinaryFormatter bfProjekt = new BinaryFormatter();
                    FileStream fsProject = new FileStream((String)sPath, FileMode.Open);
                    Projects = (ObservableCollection<CProjectViewModel>)bfProjekt.Deserialize(fsProject);
                    RaisePropertyChanged("Projects");
                    fsProject.Close();
                    log(string.Format("Solution {0} loaded.", (String)sPath));
                    Path = sPath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        bool CanLoadSolutionExecute()
        {
            return true;
        }
        public ICommand SaveSolution { get { return new CRelatedCommandParam(SaveSolutionExecute, SaveSolutionExecute); } }
        void SaveSolutionExecute(object path)
        {
            try
            {
                if (File.Exists((String)path))
                {
                    if (MessageBox.Show(string.Format("Replace file: {0} ?", (String)path), "Save Soution", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }
                BinaryFormatter bfProjekt = new BinaryFormatter();
                FileStream fsProject = new FileStream((String)path, FileMode.Create);
                bfProjekt.Serialize(fsProject, _projects);
                fsProject.Close();
                log(string.Format("Solution {0} saved.", (String)path));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        bool SaveSolutionExecute()
        {
            return true;
        }
        public ICommand LoadRecentSolution { get { return new CRelatedCommandParam(LoadRecentSolutionExecute, LoadRecentSolutionExecute); } }
        void LoadRecentSolutionExecute(object path)
        {
            try
            {
                if (File.Exists((String)path))
                {
                    String sPath = (String)path;
                    CProject ieProject = new CProject();
                    BinaryFormatter bfProjekt = new BinaryFormatter();
                    FileStream fsProject = new FileStream((String)sPath, FileMode.Open);
                    Projects = (ObservableCollection<CProjectViewModel>)bfProjekt.Deserialize(fsProject);
                    RaisePropertyChanged("Projects");
                    fsProject.Close();
                    log(string.Format("Solution {0} loaded.", (String)sPath));
                    Path = sPath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        bool LoadRecentSolutionExecute()
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
