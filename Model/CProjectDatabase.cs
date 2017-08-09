using System;

namespace cangun.Model
{
    public static class CProjectDatabase
    {
        private static int _cntPrj = 0;
        private static int _cntSeq = 0;
        public static string getProjectName()
        {
            return String.Format("Project{0}", _cntPrj++);
        }
        public static string getSequenceName()
        {
            return String.Format("Sequence{0}", _cntSeq++);
        }
    }
}
