using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileSearchMvvm.Views.Utilities
{
    class PageRangeValidationRule : ValidationRule
    {
        private int _min;
        private int _max;
        public PageRangeValidationRule() { }
        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }
        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int page_num = 0;

            // check for empty box
            if(string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(true, null);
            }

            try
            {
                if(((string)value).Length > 0)
                    page_num = Int32.Parse((String)value);
            }
            catch(Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if((page_num < Min) || (page_num > Max))
            {
                return new ValidationResult(false,
                  "Please enter an page in the range: " + Min + " - " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
