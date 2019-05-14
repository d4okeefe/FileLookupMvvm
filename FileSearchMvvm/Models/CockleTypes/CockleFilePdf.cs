using FileSearchMvvm.Models.Utilities;
using FileSearchMvvm.Models.Utilities.iTextSharpUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/***
 * NOTES: What kinds of files will be CockleFilePdf type ???
 * 
 * 1) Simple conversions from Word to Pdf
 * 2) Combined Pdf types
 * 3) Files saved in Pdf form from Current to scratch
 * 4) Files picked up straight out of scratch or docs ready to print for analysis
 * 5) Files picked up from CAMERA READY for analysis
 * 6) ANY OTHERS
 * 
 * */
namespace FileSearchMvvm.Models.CockleTypes
{
    public class CockleFilePdf : ICockleFile
    {
        #region Interface implementation
        public string FullName { get; set; }
        public string Filename
        {
            get { return System.IO.Path.GetFileName(this.FullName); }
            set { this.Filename = System.IO.Path.GetFileName(this.FullName); }
        }
        public FileLocationEnum Location
        {
            get
            {
                // should only be scratch: all files of this type are saved to scratch
                if (System.IO.Path.GetDirectoryName(this.FullName).ToLower()
                    .Contains(@"C:\scratch".ToLower()))
                { return FileLocationEnum.Scratch; }
                else { return FileLocationEnum.Unknown; }
            }
        }
        public int? TicketNumber { get; set; }
        public string Attorney { get; set; }
        public int? Version { get; set; }
        public SourceFileTypeEnum FileType { get; set; }
        public string FileTypeString
        {
            get
            {
                var type = FileType.GetType();
                var memInfo = type?.GetMember(FileType.ToString());
                return memInfo[0]?.Name;
            }
        }
        public string TicketPlusAttorney
        {
            get { return this.TicketNumber == null ? this.Attorney : string.Format("{0} {1}", this.TicketNumber, this.Attorney); }
            set { this.TicketPlusAttorney = this.TicketNumber == null ? this.Attorney : string.Format("{0} {1}", this.TicketNumber, this.Attorney); }
        }
        #endregion

        #region CockleFilePdf Properties
        public bool PdfACompliant { get; set; }
        public string FileTypeCode { get; private set; }
        public int? CoverLength { get; private set; }
        public int Rank { get; set; }
        public bool NeedsBlankPage { get; set; }
        public PageRangePdf PageRange { get; set; }
        #endregion

        #region Constructors
        // One that captures original, another that creates files from folder

        // PRIMARY COCKLEFILEPDF CONSTRUCTOR
        // USES 1. CockleFile 2. new file name 3. cover length
        public CockleFilePdf(CockleFile cockleFile, string newFileName, int? len_cover)
        {
            // THIS IS THE MOST IMPORTANT CONSTRUCTOR: MUCH MORE CONSISTENT !!!

            // set ICockleFile Properties
            this.FullName = newFileName;
            this.TicketNumber = cockleFile.TicketNumber;
            this.Attorney = cockleFile.Attorney;
            this.FileType = cockleFile.FileType;
            this.Version = cockleFile.Version;

            // set properties in common with CockleFile
            this.FileTypeCode = cockleFile.FileTypeCode;

            // set cover length
            if (this.FileType == SourceFileTypeEnum.Cover
                && null != len_cover)
            { this.CoverLength = len_cover; }
            else { this.CoverLength = null; }

            // get page ranges
            PageRange = new PageRangePdf(this.FullName, this.FileType);

            // initialize properties that need more info
            this.Rank = -1;
            this.NeedsBlankPage = false;
        }
        // SECONDARY METHOD:
        // USED FOR COMBINED PDF
        public CockleFilePdf(string filename, string atty, int? ticket, SourceFileTypeEnum file_type, string file_type_code, int? len_cover)
        {
            this.FullName = filename;
            this.TicketNumber = ticket;
            this.Attorney = atty;
            this.FileType = file_type;
            this.FileTypeCode = file_type_code;
            this.Version = null;

            // set cover length
            if (this.FileType == SourceFileTypeEnum.Cover) { this.CoverLength = len_cover; }
            else { this.CoverLength = null; }

            // get page ranges
            PageRange = new PageRangePdf(this.FullName, this.FileType);

            // initialize properties that need more info
            this.Rank = -1;
            this.NeedsBlankPage = false;
        }
        // SECONDARY METHOD
        // USED FOR CREATION OF PDF WITH FOLDOUTS
        public CockleFilePdf(string filename, string atty, int? ticket, SourceFileTypeEnum file_type, string file_type_code)
        {
            this.FullName = filename;
            this.TicketNumber = ticket;
            this.Attorney = atty;
            this.FileType = file_type;
            this.FileTypeCode = file_type_code;
            this.Version = null;
            this.CoverLength = null;

            // get page ranges
            //PageRange = new PageRangePdf(this.FullName, this.FileType);

            // initialize properties that need more info
            this.Rank = -1;
            this.NeedsBlankPage = false;
        }
        // SECONDARY METHOD:
        // USED FOR FILES CAPTURED FROM CAMERA READY, DOCS READY TO PRINT, ETC
        public CockleFilePdf(string filename, SourceFileTypeEnum type = SourceFileTypeEnum.Unrecognized)
        {
            this.FullName = filename;
            this.FileType = type;
            this.FileTypeCode = "";
            parseFileNameCollectedFromScratch();
            //this.FileType = type;
            // incomplete here: use for files straight for current of camera ready
        }
        public CockleFilePdf(string fileName, CockleFilePdf originalCockleFilePdf, SourceFileTypeEnum type)
        {
            this.FullName = fileName;
            this.Attorney = originalCockleFilePdf.Attorney;
            this.CoverLength = originalCockleFilePdf.CoverLength;
            this.FileType = type;
            this.FileTypeCode = originalCockleFilePdf.FileTypeCode;
            this.PageRange = originalCockleFilePdf.PageRange;
            this.Rank = originalCockleFilePdf.Rank;
            this.TicketNumber = originalCockleFilePdf.TicketNumber;
            this.Version = originalCockleFilePdf.Version;
        }
        #endregion

        #region public methods
        public void AssignNeedsBlankPage(List<CockleFilePdf> list, int i)
        {
            // tracks list position of item that is searched
            int index = -1;
            // if odd number of pages, then mark file as needing a blank
            if (i % 2 != 0)
            {
                if (this.FileType == SourceFileTypeEnum.Index)
                {
                    index = list.FindIndex(x => x.FileType == SourceFileTypeEnum.Brief
                        || /*for Joint Appendix*/x.FileType == SourceFileTypeEnum.App_File);
                    if (index != -1)
                    {
                        this.NeedsBlankPage = true;
                    }
                }
                else if (this.FileType == SourceFileTypeEnum.Brief)
                {
                    index = list.FindIndex(x => x.FileType == SourceFileTypeEnum.App_Index
                        || x.FileType == SourceFileTypeEnum.App_File);
                    if (index != -1)
                    {
                        this.NeedsBlankPage = true;
                    }
                }
                else if (this.FileType == SourceFileTypeEnum.Motion
                    || this.FileType == SourceFileTypeEnum.App_Index
                    || this.FileType == SourceFileTypeEnum.Divider_Page)
                {
                    this.NeedsBlankPage = true;
                }
            }
            else { this.NeedsBlankPage = false; }
        }
        public void GetPageRangeForFile()
        {
            this.PageRange = new PageRangePdf(this.FullName, this.FileType);
            if (this.PageRange.FirstPage == -1 || this.PageRange.LastPage == -1)
            {
                this.PageRange = null;
            }
        }
        #endregion

        #region private methods
        private void parseFileNameCollectedFromScratch()
        {
            //(?<= )([0 - 9]{ 1,2})(?=\.)

            //(cv|icv|brmo|mo|in|br|ain|div|[a-z]{2}) [0-1]?[0-9]{1,2}\.
            //(?<= )[0-9]{1,2}(?=\n)
            //(\d{5})(.*) (cv|icv|brmo|mo|in|br|ain|div|\w{1,2}) (\d{1,2})(.pdf)

            // get filename 
            var f = System.IO.Path.GetFileNameWithoutExtension(this.FullName);

            // 1. check ends in ".pdf"
            if (System.IO.Path.GetExtension(this.FullName).Equals(".pdf"))
            {
                // 2. check filename begins with ticket number
                var pattern1 = @"^(\d{5}) (.*)$";
                var re1 = new System.Text.RegularExpressions.Regex(pattern1);
                var mc1 = re1.Matches(f);
                if (mc1.Count > 0)
                {
                    TicketNumber = int.Parse(mc1[0].Groups[1].Value);

                    // 3a. check if "pdf is in title
                    var sub_string1 = mc1[0].Groups[2].Value;
                    var pattern2 = @"^(pdf)(( \w+)+)";
                    var re2 = new System.Text.RegularExpressions.Regex(pattern2);
                    var mc2 = re2.Matches(sub_string1);
                    if (mc2.Count > 0)
                    {
                        var sub_string2 = mc2[0].Groups[2].Value;
                        var pattern3 = @"( .*) (br|app|appendix|brief)";
                        var re3 = new System.Text.RegularExpressions.Regex(pattern3);
                        var mc3 = re3.Matches(sub_string2);
                        if (mc3.Count > 0)
                        {
                            var sub_string3 = mc3[0].Groups[2].Value;
                            if (sub_string3.Equals("br") || sub_string3.Equals("brief"))
                            {
                                Attorney = mc3[0].Groups[1].Value.Trim();
                                FileType = SourceFileTypeEnum.Combined_Brief;
                            }
                            else if (sub_string3.Equals("app") || sub_string3.Equals("appendix"))
                            {
                                Attorney = mc3[0].Groups[1].Value.Trim();
                                FileType = SourceFileTypeEnum.Combined_Appendix;
                            }
                        }
                        else
                        {
                            Attorney = sub_string2;
                            FileType = SourceFileTypeEnum.Combined_Pdf;
                        }

                    }

                    // 3b. check if it's a 'print ready' file
                    var pattern4 = @"^((\w+ )+)(pr|A|a|B|b)$";
                    var re4 = new System.Text.RegularExpressions.Regex(pattern4);
                    var mc4 = re4.Matches(sub_string1);
                    if (mc4.Count > 0)
                    {
                        FileType = SourceFileTypeEnum.Imposed_Pdf;
                    }

                    // 3c. check if it's camera ready
                    //     (i.e. date entered matches style our office input)
                    var pattern6 = @"(\d{1,2}-\d{1,2}-\d{2})\W(\d{3,4})\W*(am|pm)";
                    var re6 = new System.Text.RegularExpressions.Regex(pattern6);
                    var mc6 = re6.Matches(sub_string1);
                    if (mc6.Count > 0)
                    {
                        FileType = SourceFileTypeEnum.Camera_Ready;

                        // try to capture atty name:
                        var pattern6a = @"^((\w+\s)+)(?=\d)";
                        var re6a = new System.Text.RegularExpressions.Regex(pattern6a);
                        var mc6a = re6a.Matches(sub_string1);
                        if(mc6a.Count > 0)
                        {
                            Attorney = mc6a[0].Groups[1].Value.Trim();
                        }
                    }

                    // 3d. original file type: from Word conversion
                    if (this.FileType == SourceFileTypeEnum.Unrecognized)
                    {
                        //ok, no "pdf" in title of document
                        //so, try to id using old methods
                        string[] split = System.IO.Path.GetFileNameWithoutExtension(this.FullName).Split(' ');

                        if (split.Length == 4)
                        {
                            // get (1) ticket number
                            int out_ticket; // create local variable, b/c cannot pass property as "out" variable
                            if (!int.TryParse(split[0], out out_ticket)) { this.TicketNumber = null; }
                            else { this.TicketNumber = out_ticket; }

                            // get (1) attorney
                            this.Attorney = split[1];

                            // get (3) type
                            this.FileTypeCode = split[split.Length - 2];
                            this.FileType = getValueOfType(this.FileTypeCode);

                            // get (4) version
                            int out_version; // create local variable, b/c cannot pass property as "out" variable
                            if (!int.TryParse(split[split.Length - 1], out out_version)) { this.Version = null; }
                            else { this.Version = out_version; }
                        }
                        else if (split.Length > 4)
                        {
                            string ticket_version_capture = System.IO.Path.GetFileNameWithoutExtension(this.FullName);

                            int out_ticket;
                            if (int.TryParse(ticket_version_capture.Substring(
                                0, ticket_version_capture.IndexOf(' ')), out out_ticket))
                            { this.TicketNumber = out_ticket; }
                            else { this.TicketNumber = null; }

                            int out_version;
                            string ver = ticket_version_capture.Substring(ticket_version_capture.LastIndexOf(' '),
                                ticket_version_capture.Length - ticket_version_capture.LastIndexOf(' '));
                            if (int.TryParse(ver, out out_version))
                            { this.Version = out_version; }
                            else { this.Version = null; }

                            // alternative attorney and filetype capture
                            if (this.TicketNumber != null && this.Version != null)
                            {
                                // remove ticket and version
                                string atty_plus_typeOfFile = System.IO.Path.GetFileNameWithoutExtension(this.FullName);
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(this.TicketNumber.ToString(), "");
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(ver, "");
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Trim(' ');

                                // extract first word as atty
                                this.Attorney = atty_plus_typeOfFile.Substring(0, atty_plus_typeOfFile.IndexOf(' '));
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(this.Attorney, "");
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Trim(' ');

                                // extract last word as filetype
                                this.FileTypeCode = atty_plus_typeOfFile.Substring(atty_plus_typeOfFile.LastIndexOf(' '),
                                    atty_plus_typeOfFile.Length - atty_plus_typeOfFile.LastIndexOf(' '));
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(this.FileTypeCode, "");
                                atty_plus_typeOfFile = atty_plus_typeOfFile.Trim(' ');

                                // create new split
                                string[] split2 = atty_plus_typeOfFile.Split(' ');

                                // extract based on upper and lower case
                                int i = 0;
                                bool foundLowerCase = false;
                                this.Attorney = this.Attorney.Trim();
                                this.FileTypeCode = this.FileTypeCode.Trim();
                                while (i < split2.Length)
                                {
                                    string str = split2[i];
                                    if (char.IsUpper(str[0]) && !foundLowerCase)
                                    {
                                        this.Attorney = this.Attorney + " " + str;
                                    }
                                    else
                                    {
                                        this.FileTypeCode = str + " " + this.FileTypeCode;
                                        foundLowerCase = true;
                                    }
                                    i++;
                                }
                                this.Attorney = this.Attorney.Trim();
                                this.FileTypeCode = this.FileTypeCode.Trim();
                                this.FileType = getValueOfType(this.FileTypeCode);
                            }
                        }
                        else if (split.Length < 4)
                        {
                            // not sure what to do here: probably the file has an error
                        }
                    }
                    else // triggers if style is not '12345 _____.pdf'
                    {

                    }
                }
            }
        }

        private void parseFileName()
        {
            string[] split = System.IO.Path
                .GetFileNameWithoutExtension(this.FullName).Split(' ');

            if (split.Length == 4)
            {
                // get (1) ticket number
                int out_ticket; // create local variable, b/c cannot pass property as "out" variable
                if (!int.TryParse(split[0], out out_ticket)) { this.TicketNumber = null; }
                else { this.TicketNumber = out_ticket; }

                // get (1) attorney
                this.Attorney = split[1];

                // get (3) type
                this.FileTypeCode = split[split.Length - 2];
                this.FileType = getValueOfType(this.FileTypeCode);

                // get (4) version
                int out_version; // create local variable, b/c cannot pass property as "out" variable
                if (!int.TryParse(split[split.Length - 1], out out_version)) { this.Version = null; }
                else { this.Version = out_version; }
            }
            else if (split.Length > 4)
            {
                string ticket_version_capture = System.IO.Path.GetFileNameWithoutExtension(this.FullName);

                int out_ticket;
                if (int.TryParse(ticket_version_capture.Substring(
                    0, ticket_version_capture.IndexOf(' ')), out out_ticket))
                { this.TicketNumber = out_ticket; }
                else { this.TicketNumber = null; }

                int out_version;
                string ver = ticket_version_capture.Substring(ticket_version_capture.LastIndexOf(' '),
                    ticket_version_capture.Length - ticket_version_capture.LastIndexOf(' '));
                if (int.TryParse(ver, out out_version))
                { this.Version = out_version; }
                else { this.Version = null; }

                // alternative attorney and filetype capture
                if (this.TicketNumber != null && this.Version != null)
                {
                    // remove ticket and version
                    string atty_plus_typeOfFile = System.IO.Path.GetFileNameWithoutExtension(this.FullName);
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(this.TicketNumber.ToString(), "");
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(ver, "");
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Trim(' ');

                    // extract first word as atty
                    this.Attorney = atty_plus_typeOfFile.Substring(0, atty_plus_typeOfFile.IndexOf(' '));
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(this.Attorney, "");
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Trim(' ');

                    // extract last word as filetype
                    this.FileTypeCode = atty_plus_typeOfFile.Substring(atty_plus_typeOfFile.LastIndexOf(' '),
                        atty_plus_typeOfFile.Length - atty_plus_typeOfFile.LastIndexOf(' '));
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Replace(this.FileTypeCode, "");
                    atty_plus_typeOfFile = atty_plus_typeOfFile.Trim(' ');

                    // create new split
                    string[] split2 = atty_plus_typeOfFile.Split(' ');

                    // extract based on upper and lower case
                    int i = 0;
                    bool foundLowerCase = false;
                    this.Attorney = this.Attorney.Trim();
                    this.FileTypeCode = this.FileTypeCode.Trim();
                    while (i < split2.Length)
                    {
                        string str = split2[i];
                        if (char.IsUpper(str[0]) && !foundLowerCase)
                        {
                            this.Attorney = this.Attorney + " " + str;
                        }
                        else
                        {
                            this.FileTypeCode = str + " " + this.FileTypeCode;
                            foundLowerCase = true;
                        }
                        i++;
                    }
                    this.Attorney = this.Attorney.Trim();
                    this.FileTypeCode = this.FileTypeCode.Trim();
                    this.FileType = getValueOfType(this.FileTypeCode);
                }
            }
            else if (split.Length < 4)
            {
                // not sure what to do here: probably the file has an error
            }
        }
        private SourceFileTypeEnum getValueOfType(string str)
        {
            // set default value
            SourceFileTypeEnum key = SourceFileTypeEnum.Unrecognized;

            // search for matches
            foreach (var p in primary_patterns)
            {
                if (System.Text.RegularExpressions.Regex.Matches(str, p.Value).Count > 0)
                { key = p.Key; }
                if (key == SourceFileTypeEnum.Motion)
                { break; }
            }

            // search for foldout in brief (probably can wrap into Regex in future)
            if (key != SourceFileTypeEnum.Unrecognized)
            {
                if (str.Contains("br"))
                {
                    if (key == SourceFileTypeEnum.App_Foldout)
                    {
                        key = SourceFileTypeEnum.Brief_Foldout;
                    }
                    else if (key == SourceFileTypeEnum.App_ZFold)
                    {
                        key = SourceFileTypeEnum.Brief_ZFold;
                    }
                }
            }
            return key;
        }
        #endregion
        #region Constant: file name patterns
        private Dictionary<SourceFileTypeEnum, string> primary_patterns =
            new Dictionary<SourceFileTypeEnum, string>
        {
            {SourceFileTypeEnum.Cover, @"^cv\s*[a-z]*$"},
            {SourceFileTypeEnum.InsideCv, @"^icv\s*[a-z]*$"},
            {SourceFileTypeEnum.Motion, @"^(br)*\s*mo$"},
            {SourceFileTypeEnum.Index, @"^in\s*[a-z]*$"},
            {SourceFileTypeEnum.Brief, @"^br\s*[a-z]*$"},
            {SourceFileTypeEnum.Divider_Page, @"^div\s*[a-z]*$"},
            {SourceFileTypeEnum.App_Index, @"^ain\s*[a-z]*$"},
            {SourceFileTypeEnum.App_File,@"^[azyw][a-z]$"},
            {SourceFileTypeEnum.App_Foldout, @"fo"},
            {SourceFileTypeEnum.App_ZFold, @"(^|\s)(z\s*fo)"},
            {SourceFileTypeEnum.SidewaysPage, @"sw"},
            {SourceFileTypeEnum.Certificate_of_Service, @"^cos$"}
        };
        #endregion
    }
}