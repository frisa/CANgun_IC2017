using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.OleDb;
using cangun.Model;
using cangun.Model.MR;

namespace cangun.ViewModel
{
    [Serializable]
    public class CProjectViewModel : INotifyPropertyChanged
    {
        private CProject _project;
        private String _sConnectionString;
        static private String _filterTitelString = String.Empty;
        static private String _filterContentString = String.Empty;
        static private String _filterMrString = String.Empty;
        static private String _filterStatus = "   ";
        private String _filterConstantString;
        static private String _filterOutString = "n/a";

        [field: NonSerialized] private DataRowView _drvMR;
        [field: NonSerialized] private CMR _actualMR = new CMR();
        private CProject Project
        {
            get { return _project; }
            set { _project = value; }
        }
        public string ProjectName
        {
            get { return Project.Name; }
            set
            {
                Project.Name = value;
                RaisePropertyChanged("ProjectName");
            }
        }
        public string ProjectPathMRDB
        {
            get { return Project.PathMRDB; }
            set
            {
                Project.PathMRDB = value;
                RaisePropertyChanged("ProjectPathMRDB");
            }
        }
        public DataTable ProjectDtMRs
        {
            get { return Project.dtMRs; }
            set
            {
                Project.dtMRs = value;
                RaisePropertyChanged("ProjectDtMRs");
            }
        }
        public DataTable ProjectDtMRsFav
        {
            get { return Project.dtMRsFav; }
            set
            {
                Project.dtMRsFav = value;
                RaisePropertyChanged("ProjectDtMRsFav");
            }
        }
        public DataRowView drvMR
        {
            get { return _drvMR; }
            set
            {
                if (value != null)
                {
                    _drvMR = value;
                    if (ActualMR == null) ActualMR = new CMR();
                    ActualMR.Number = _drvMR["MeldungsNr"].ToString();
                    ActualMR.Title = _drvMR["Titel"].ToString();
                    ActualMR.Content = _drvMR["Inhalt"].ToString();
                    ActualMR.Project = _drvMR["Produkt"].ToString();
                    ActualMR.Author = _drvMR["MitarbeiterKurzname"].ToString();
                    ActualMR.State = _drvMR["Zustand"].ToString();
                    ActualMR.File = _drvMR["Bezugsdokument"].ToString();

                    OleDbDataAdapter daComments = new OleDbDataAdapter("SELECT MitarbeiterKurzname, Stellungnahme, SWBaustein FROM [Stellungnahme Entwicklung] WHERE MeldungsNr=" + ActualMR.Number, _sConnectionString);
                    OleDbDataAdapter daMeassurement = new OleDbDataAdapter("SELECT MES_Author, MES_Module, MES_done FROM [MR_Measurements] WHERE MeldungsNr=" + ActualMR.Number, _sConnectionString);
                    OleDbDataAdapter daCcb = new OleDbDataAdapter("SELECT Teilnehmer, Ergebnis FROM [CCBEntscheid] WHERE MeldungsNr=" + ActualMR.Number, _sConnectionString);

                    DataTable dtComments = new DataTable();
                    DataTable dtCcb = new DataTable();
                    DataTable dtMeassurement = new DataTable();
                    try
                    {
                        daComments.Fill(dtComments);
                        daCcb.Fill(dtCcb);
                        daMeassurement.Fill(dtMeassurement);
                        ActualMR.Comments = new ObservableCollection<CMRComment>();
                        ActualMR.CCBs = new ObservableCollection<CMRCcb>();
                        ActualMR.Measurements = new ObservableCollection<CMRMeasurement>();
                        foreach (DataRow dr in dtComments.Rows)
                        {
                            CMRComment iComment = new CMRComment();
                            iComment.Author = dr["MitarbeiterKurzname"].ToString();
                            iComment.Comment = dr["Stellungnahme"].ToString();
                            iComment.Modul = dr["SWBaustein"].ToString();
                            ActualMR.Comments.Add(iComment);
                        }
                        foreach (DataRow dr in dtCcb.Rows)
                        {
                            CMRCcb iCcb = new CMRCcb();
                            iCcb.Author = dr["Teilnehmer"].ToString();
                            //iCcb.State   = dr["CCB-Entscheid"].ToString();
                            iCcb.Result = dr["Ergebnis"].ToString();
                            ActualMR.CCBs.Add(iCcb);
                        }
                        foreach (DataRow dr in dtMeassurement.Rows)
                        {
                            CMRMeasurement iMeas = new CMRMeasurement();
                            iMeas.Author = dr["MES_Author"].ToString();
                            iMeas.Module = dr["MES_Module"].ToString();
                            iMeas.State = dr["MES_done"].ToString();
                            ActualMR.Measurements.Add(iMeas);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public CMR ActualMR
        {
            get
            {
                return _actualMR;
            }
            set
            {
                _actualMR = value;
                RaisePropertyChanged("ActualMR");
            }
        }
        public string ProejctDescription
        {
            get { return _project.Description; }
            set { _project.Description = value; }
        }
        public string ProjectPathDbc
        {
            get { return _project.PathDbc; }
            set
            {
                _project.PathDbc = value;
                RaisePropertyChanged("ProjectPathDbc");
            }
        }
        public CProjectViewModel()
        {
            _project = new CProject();
        }

        AsyncObservableCollection<CMessageViewModel> _messages = new AsyncObservableCollection<CMessageViewModel>();
        public AsyncObservableCollection<CMessageViewModel> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        ObservableCollection<CSequenceViewModel> _sequences = new ObservableCollection<CSequenceViewModel>();
        public ObservableCollection<CSequenceViewModel> Sequences
        {
            get { return _sequences; }
            set { _sequences = value; }
        }

        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand UpdateProjectName { get { return new CRelatedCommandNoParam(UpdateProjectNameExecute, CanUpdateProjectNameExecute); } }
        void UpdateProjectNameExecute()
        {
            this.ProjectName = CProjectDatabase.getProjectName();
        }
        bool CanUpdateProjectNameExecute()
        {
            return true;
        }
        public ICommand AddMessage { get { return new CRelatedCommandParam(AddCustomMessageExecute, CanAddCustomMessageExecute); } }
        void AddCustomMessageExecute(object name)
        {
            _messages.Add(new CMessageViewModel() { MessageId = 1, MessageName = (String)name, MessageDlc = 8, MessageEcu = "Ecu", MessageData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 } });
        }
        bool CanAddCustomMessageExecute()
        {
            return true;
        }
        public ICommand LoadDbc { get { return new CRelatedCommandNoParam(LoadDbcExecute, CanLoadDbcExecute); } }
        void LoadDbcExecute()
        {
            try
            {
                FilterOutString = "Loading database";
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".dbc";
                dlg.Filter = "DBC Files (*.dbc)|*.dbc";
                if (dlg.ShowDialog() == true)
                {
                    String sPath = dlg.FileName;
                    if (System.IO.File.Exists(sPath))
                    {
                        const string MESSAGES_EXP = @"BO_ (?<message_id>[0-9]+) (?<message_name>[a-zA-Z0-9_]+): (?<message_dlc>[0-8]) (?<message_ecu>[a-zA-Z0-9_]+)\r\n(?<message_signals>( SG_[^\r\n]+\r\n)+)";
                        const string SIGNALS_EXP = @"SG_ (?<signal_name>[a-zA-Z0-9_]+)(?<signal_sbit>[a-zA-Z0-9_ ]*): (?<signal_sbit>[0-9]+)\|(?<signal_len>[0-9]+)\@(?<signal_intel>[0-9]+)(?<signal_signed>[\+\-]+)";

                        StreamReader sr = new StreamReader(sPath);
                        String FileContent = sr.ReadToEnd();
                        MatchCollection mcMessages = Regex.Matches(FileContent, MESSAGES_EXP, RegexOptions.Multiline);
                        Match mComment;
                        Int64 i64MsgId;
                        string sCommentPattern;
                        _messages.Clear();
                        foreach (Match mMessages in mcMessages)
                        {
                            CMessageViewModel oMsvm = new CMessageViewModel();
                            i64MsgId = Convert.ToInt64(mMessages.Groups["message_id"].Value);
                            if (i64MsgId == 1414)
                            {

                            }
                            if (i64MsgId <= Int32.MaxValue)
                            {
                                oMsvm.MessageId = Convert.ToInt32(i64MsgId);
                                oMsvm.MessageName = Convert.ToString(mMessages.Groups["message_name"].Value);
                                oMsvm.MessageDlc = Convert.ToInt32(mMessages.Groups["message_dlc"].Value);
                                oMsvm.MessageEcu = Convert.ToString(mMessages.Groups["message_ecu"].Value);
                                oMsvm.MessageData = new byte[oMsvm.MessageDlc];
                                sCommentPattern = @"CM_ BO_ " + mMessages.Groups["message_id"].Value.ToString() + @" ""(?<message_comment>.*)""";
                                mComment = Regex.Match(FileContent, sCommentPattern);
                                if (mComment.Success)
                                    oMsvm.MessageComment = mComment.Groups["message_comment"].Value;
                                MatchCollection mcSignals = Regex.Matches(mMessages.Groups["message_signals"].ToString(), SIGNALS_EXP, RegexOptions.Multiline);
                                foreach (Match mSignals in mcSignals)
                                {
                                    CSignalViewModel oSigvm = new CSignalViewModel();
                                    oSigvm.SignalName = Convert.ToString(mSignals.Groups["signal_name"].Value);
                                    oSigvm.SignalStartBit = Convert.ToInt32(mSignals.Groups["signal_sbit"].Value);
                                    oSigvm.SignalLenght = Convert.ToInt32(mSignals.Groups["signal_len"].Value);
                                    oSigvm.SignalIntel = Convert.ToString(mSignals.Groups["signal_intel"].Value);
                                    oSigvm.SignalSigned = Convert.ToString(mSignals.Groups["signal_signed"].Value);
                                    sCommentPattern = @"CM_ SG_ " + mMessages.Groups["message_id"].Value.ToString() + @" " + mSignals.Groups["signal_name"].Value.ToString() + @" ""(?<signal_comment>.*)""";
                                    mComment = Regex.Match(FileContent, sCommentPattern);
                                    if (mComment.Success)
                                        oSigvm.SignalComment = mComment.Groups["signal_comment"].Value;
                                    oMsvm.Signals.Add(oSigvm);
                                }
                                this.Messages.Add(oMsvm);
                            }
                        }
                        ProjectPathDbc = (String)sPath;
                    }
                    else
                    {
                        MessageBox.Show(String.Format("The File {0} does not exist", sPath));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            FilterOutString = "Database loaded";
        }
        bool CanLoadDbcExecute()
        {
            return true;
        }
        public ICommand AddSequence { get { return new CRelatedCommandNoParam(AddSequenceExecute, CanAddSequenceExecute); } }
        void AddSequenceExecute()
        {
            _sequences.Add(new CSequenceViewModel() { SequenceName = CProjectDatabase.getSequenceName() });
        }
        bool CanAddSequenceExecute()
        {
            return true;
        }
        public ICommand LoadMRDB { get { return new CRelatedCommandParam(LoadMRDBExecute, CanLoadMRDBExecute); } }
        public ICommand Filter { get { return new CRelatedCommandNoParam(FilterExecute, CanFilterExecute); } }
        public ICommand AddToFav { get { return new CRelatedCommandNoParam(AddToFavExecute, CanAddToFavExecute); } }
        bool CanAddToFavExecute()
        {
            return true;
        }
        void AddToFavExecute()
        {
            if (null == ProjectDtMRsFav)
            {
                ProjectDtMRsFav = new DataTable("MR Favourites");
                ProjectDtMRsFav.Columns.Add("MeldungsNr");
                ProjectDtMRsFav.Columns.Add("Titel");
                ProjectDtMRsFav.Columns.Add("Inhalt");
                ProjectDtMRsFav.Columns.Add("Produkt");
                ProjectDtMRsFav.Columns.Add("MitarbeiterKurzname");
                ProjectDtMRsFav.Columns.Add("Zustand");
            }
            DataRow newRow = ProjectDtMRsFav.NewRow();
            newRow["MeldungsNr"] = _actualMR._number;
            newRow["Titel"] = _actualMR._title;
            newRow["Inhalt"] = _actualMR._content;
            newRow["Produkt"] = _actualMR._project;
            newRow["MitarbeiterKurzname"] = _actualMR._author;
            newRow["Zustand"] = _actualMR._state;
            ProjectDtMRsFav.Rows.Add(newRow);
        }
        public ICommand ClearFilter { get { return new CRelatedCommandNoParam(ClearFilterExecute, CanClearFilterExecute); } }
        public string FilterMrString
        {
            get { return _filterMrString; }
            set
            {
                _filterMrString = value;
                RaisePropertyChanged("FilterMrString");
                RaiseFilter();
            }
        }
        public string FilterTitelString
        {
            get { return _filterTitelString; }
            set
            {
                _filterTitelString = value;
                RaisePropertyChanged("FilterTitelString");
                RaiseFilter();
            }
        }
        public string FilterContentString
        {
            get { return _filterContentString; }
            set
            {
                _filterContentString = value;
                RaisePropertyChanged("FilterContentString");
                RaiseFilter();
            }
        }
        public string FilterConstantString
        {
            get { return _filterConstantString; }
            set
            {
                _filterConstantString = value;
                RaisePropertyChanged("FilterConstantString");
                RaiseFilter();
            }
        }
        public string FilterOutString
        {
            get { return _filterOutString; }
            set
            {
                _filterOutString = value;
                RaisePropertyChanged("FilterOutString");
            }
        }
        public string FilterStatus
        {
            get { return _filterStatus; }
            set
            {
                _filterStatus = value;
                RaisePropertyChanged("FilterStatus");
                RaiseFilter();
            }
        }
        private void RaiseFilter()
        {
            if (null != ProjectDtMRs)
            {
                DataView dv = ProjectDtMRs.DefaultView;
                String sFilterString = String.Empty;
                try
                {
                    if (!String.IsNullOrWhiteSpace(_filterConstantString))
                        sFilterString = sFilterString + _filterConstantString + " AND ";

                    if (!String.IsNullOrWhiteSpace(_filterMrString))
                        sFilterString = sFilterString + "MrNr LIKE '%" + _filterMrString + "%' AND ";

                    sFilterString = sFilterString + "Titel LIKE '%" + _filterTitelString + "%' AND " +
                                    "Inhalt LIKE '%" + _filterContentString + "%'";
                    if (!String.IsNullOrWhiteSpace(_filterStatus.Substring(_filterStatus.Length - 3, 3)))
                        sFilterString = sFilterString + " AND Zustand LIKE '" + _filterStatus.Substring(_filterStatus.Length - 3, 3) + "'";
                    dv.RowFilter = sFilterString;
                    RaisePropertyChanged("ProjectDtMRs");
                }
                catch (Exception ex)
                {
                    FilterOutString = ex.Message + "FilStr: " + sFilterString;
                }
            }
        }
        void FilterExecute()
        {
            RaiseFilter();
            FilterOutString = "Filter executed";
        }
        bool CanFilterExecute()
        {
            return true;
        }
        void ClearFilterExecute()
        {
            FilterMrString = "";
            FilterTitelString = "";
            FilterContentString = "";
            FilterStatus = "   ";
            RaiseFilter();
            FilterOutString = "Filter cleared";
        }
        bool CanClearFilterExecute()
        {
            return true;
        }
        void LoadMRDBExecute(object pathMRDB)
        {
            try
            {
                String sPathMRDB = pathMRDB.ToString();
                if (File.Exists(sPathMRDB))
                {
                    _sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sPathMRDB + ";";
                    OleDbDataAdapter da = new OleDbDataAdapter("SELECT MeldungsNr, Titel, Produkt, Inhalt, MitarbeiterKurzname, Zustand, Bezugsdokument  FROM MRAntrag", _sConnectionString);

                    ProjectDtMRs = new DataTable();
                    da.Fill(ProjectDtMRs);
                    ProjectDtMRs.Columns.Add("MrNr");
                    foreach (DataRow row in ProjectDtMRs.Rows)
                    {
                        row["MrNr"] = row["MeldungsNr"].ToString();
                    }
                    RaisePropertyChanged("ProjectDtMRs");
                }
                else
                {
                    MessageBox.Show(String.Format("The File {0} does not exist", sPathMRDB));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool CanLoadMRDBExecute()
        {
            return true;
        }
    }
}
