using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileSearchMvvm.Models.CockleTypes
{
    public class CockleFileInCameraReady: INotifyPropertyChanged
    {
        public CockleFileInCameraReady(string f)
        {
            FileName = f;

            // run regex on ticket and attorney here
            re = new System.Text.RegularExpressions.Regex(pattern);
            mc = re.Matches(System.IO.Path.GetFileNameWithoutExtension(f));
        }
        private System.Text.RegularExpressions.MatchCollection mc;
        //private string pattern = @"(\d{5}) (\w+) (\d{1,2}-\d{1,2}-\d{1,2}) (\d+) {0,2}(am|pm).*(.pdf)";
        private string pattern = @"^(\d{5}) ([A-Za-z]+)";
        private System.Text.RegularExpressions.Regex re;
        //public DateTime? Date
        //{
        //    get
        //    {
        //        if (mc == null || mc.Count == 0) { return null; }

        //        var split = mc[0].Groups[3].Value.Split('-');
        //        return new DateTime(
        //            int.Parse("20" + split[2].ToString()),
        //            int.Parse(split[0].ToString()),
        //            int.Parse(split[1].ToString()));
        //    }
        //}
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                RaisePropertyChanged();
            }
        }
        public string Ticket
        {
            get
            {
                if (mc == null || mc.Count == 0) { return string.Empty; }
                return mc[0].Groups[1].Value;
            }
        }
        public string Attorney
        {
            get
            {
                if (mc == null || mc.Count == 0) { return string.Empty; }
                return mc[0].Groups[2].Value;
            }
        }
        public string FileName { get; set; }
        public string ShortFileName
        {
            get
            {
                return System.IO.Path.GetFileName(FileName);
            }
        }
        // INTERFACE ONLY
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
