using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections.Generic;

namespace FileSearchMvvm.Models.CockleTypes
{
    public class CockleFile : ICockleFile
    {
        #region Constants
        private string[] current_folder = { @"\\clbdc03\Current\", @"H:\" };
        private string[] backup_folder = { @"\\clbdc03\Backup_Typesetting\", @"F:\" };
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

        #region Interface implementation
        public string FullName { get; set; }
        public string Filename
        {
            get { return System.IO.Path.GetFileName(FullName); }
            set { Filename = System.IO.Path.GetFileName(FullName); }
        }
        public FileLocationEnum Location
        {
            get
            {
                foreach (string s in current_folder)
                {
                    if (FullName.ToLower().Contains(s.ToLower()))
                    {
                        return FileLocationEnum.Current;
                    }
                }
                foreach (string s in backup_folder)
                {
                    if (FullName.ToLower().Contains(s.ToLower()))
                    {
                        return FileLocationEnum.Backup;
                    }
                }
                return FileLocationEnum.Unknown;
            }
        }
        public string LocalFullFilename
        {
            get
            {
                if (System.IO.Directory.Exists(@"H:\")
                    && System.IO.Directory.Exists(@"F:\"))
                {
                    var root_dir = System.IO.Directory.GetDirectoryRoot(FullName);
                    var fullname_without_root = FullName.Remove(0, root_dir.Length + 1);

                    string local_fullname = string.Empty;
                    if (root_dir.ToLower().Equals(@"\\clbdc03\Current".ToLower()))
                    {
                        local_fullname = System.IO.Path.Combine(@"H:\", fullname_without_root);
                        return local_fullname;
                    }
                    else if (root_dir.ToLower().Equals(@"\\clbdc03\Backup_Typesetting".ToLower()))
                    {
                        local_fullname = System.IO.Path.Combine(@"F:\", fullname_without_root);
                        return local_fullname;
                    }
                }
                return FullName;
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
                //var type = FileType.GetType();
                //var memInfo = type?.GetMember(FileType.ToString());
                //return memInfo[0]?.Name;

                return FileType.GetType()
                    .GetMember(FileType.ToString())[0].Name;
            }
        }
        public string TicketPlusAttorney
        {
            get
            {
                return TicketNumber == null ?
                    Attorney : string.Format("{0} {1}", TicketNumber, Attorney);
            }
        }
        #endregion

        #region CockleFile Properties
        public bool IsLatestFile { get; set; }
        public int? Year
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName)) { return null; }

                if (Location == FileLocationEnum.Current) { return DateTime.Now.Year; }

                if (Location == FileLocationEnum.Backup)
                {
                    try
                    {
                        string fn = FullName.ToLower();
                        fn = fn.Replace(backup_folder[0].ToLower(), "");
                        fn = fn.Substring(0, fn.IndexOf('\\'));
                        if (fn.Equals(FullName.ToLower()))
                        {
                            fn = FullName.Replace(backup_folder[1].ToLower(), "");
                            fn = fn.Substring(0, fn.IndexOf('\\'));
                        }
                        int out_number;
                        if (int.TryParse(fn, out out_number))
                        {
                            return out_number;
                        }
                    }
                    catch (Exception excpt)
                    {
                        System.Diagnostics.Debug.WriteLine(excpt);
                    }
                }
                return null;
            }
            set { }
        }
        public string FileTypeCode { get; private set; }
        public string SourceFolderOnServer { get { return System.IO.Path.GetDirectoryName(FullName); } }
        public string SourceFolderLocal
        {
            get
            {
                string dir = System.IO.Path.GetDirectoryName(FullName);
                // remove extra '\' from folder name
                dir = dir.Replace(current_folder[0].Substring(0, current_folder[0].Length - 1), current_folder[1].Substring(0, current_folder[1].Length - 1));
                dir = dir.Replace(backup_folder[0].Substring(0, backup_folder[0].Length - 1), backup_folder[1].Substring(0, backup_folder[1].Length - 1));
                return dir;
            }
        }
        #endregion

        #region Constructor
        public CockleFile(string f = "")
        {
            FullName = f;

            // get Location & Year
            //Year = getYear();

            // initialize variable dependent on outside values
            IsLatestFile = false;

            // method to load info:
            // (1) ticket (2) attorney (3) file type (4) version
            parseFileName();
        }
        #endregion

        #region Private Utility Methods
        private void parseFileName()
        {
            string[] split = System.IO.Path.
                GetFileNameWithoutExtension(FullName).Split(' ');

            // alter array if any split is white space or empty
            var white_space_element = 0;
            foreach (var e in split)
            {
                if (string.IsNullOrWhiteSpace(e))
                {
                    white_space_element++;
                }
            }
            if (white_space_element > 0)
            {
                string[] split2 = new string[split.Length - white_space_element];


                for (int i = 0, j = 0; i < split.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(split[i]))
                    {
                        split2[j++] = split[i];
                    }
                }
                split = split2;
            }

            if (split.Length == 4)
            {
                // get (1) ticket number
                int out_ticket; // create local variable, b/c cannot pass property as "out" variable
                if (!int.TryParse(split[0], out out_ticket)) { TicketNumber = null; }
                else { TicketNumber = out_ticket; }

                // get (1) attorney
                Attorney = split[1];

                // get (3) type
                FileTypeCode = split[split.Length - 2];
                FileType = getValueOfType(FileTypeCode);

                // get (4) version
                int out_version; // create local variable, b/c cannot pass property as "out" variable
                if (!int.TryParse(split[split.Length - 1], out out_version)) { Version = null; }
                else { Version = out_version; }
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
    }
}
