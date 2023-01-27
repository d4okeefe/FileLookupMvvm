using System.Collections.Generic;

namespace FileSearchMvvm.Models.Utilities
{
    enum LocalJavascripts
    {
        convert_pdfa,
        center_letter_no_cover,
        center_letter_48pica,
        center_letter_49pica,
        center_letter_50pica,
        center_letter_51pica,
        center_booklet_no_cover,
        center_booklet_48pica,
        center_booklet_49pica,
        center_booklet_50pica,
        center_booklet_51pica,
        nUpCircuitCoverOn11by19,
        nUpCircuitCoverOn8pt5by23,
        arePagesSmallerThanLetter,
        arePagesLargerThanBooklet,
        centerCenteredDocOnLetterPages,
        centerCenteredDocOnBookletPages,
        extractPagesFromDocument
    }
    static class AcrobatJS
    {
        public static bool LocalJavascriptFileExists
        {
            get { return System.IO.File.Exists(localJavascriptFile); }
        }
        // for Acrobat 10
        //private static string localJavascriptFile = @"C:\Users\" + System.Environment.UserName
        //    + @"\AppData\Roaming\Adobe\Acrobat\Privileged\10.0\JavaScripts\local_scripts.js";
        // for Acrobat DC
        private static string localJavascriptFile = @"C:\Users\" + System.Environment.UserName
            + @"\AppData\Roaming\Adobe\Acrobat\Privileged\DC\JavaScripts\local_scripts.js";

        public static Dictionary<LocalJavascripts, string> Javascripts =
            new Dictionary<LocalJavascripts, string>
        {
            { LocalJavascripts.convert_pdfa, "convertToPdfA" },
            { LocalJavascripts.center_letter_no_cover, "centerPdfOnLetterPageNoCover" },
            { LocalJavascripts.center_letter_48pica, "centerPdfOnLetterPageWithCover48" },
            { LocalJavascripts.center_letter_49pica, "centerPdfOnLetterPageWithCover49" },
            { LocalJavascripts.center_letter_50pica, "centerPdfOnLetterPageWithCover50" },
            { LocalJavascripts.center_letter_51pica, "centerPdfOnLetterPageWithCover51" },
            { LocalJavascripts.center_booklet_no_cover, "centerPdfOnBriefPageWithNoCover" },
            { LocalJavascripts.center_booklet_48pica, "centerPdfOnBriefPageWithCover48" },
            { LocalJavascripts.center_booklet_49pica, "centerPdfOnBriefPageWithCover49" },
            { LocalJavascripts.center_booklet_50pica, "centerPdfOnBriefPageWithCover50" },
            { LocalJavascripts.center_booklet_51pica, "centerPdfOnBriefPageWithCover51" },
            { LocalJavascripts.nUpCircuitCoverOn11by19, "nUpCircuitCoverOn11by19" },
            { LocalJavascripts.nUpCircuitCoverOn8pt5by23, "nUpCircuitCoverOn8pt5by23" },
            { LocalJavascripts.arePagesSmallerThanLetter, "arePagesSmallerThanLetter"},
            { LocalJavascripts.arePagesLargerThanBooklet, "arePagesLargerThanBooklet"},
            { LocalJavascripts.centerCenteredDocOnLetterPages, "centerCenteredDocOnLetterPages"},
            { LocalJavascripts.centerCenteredDocOnBookletPages, "centerCenteredDocOnBookletPages"},
            { LocalJavascripts.extractPagesFromDocument, "extractPagesFromDocument"}
        };

        public static bool AreAcrobatJavascriptsInPlace()
        {
            /* Location of Javascript folder: check folder-level file
             * C:\Users\daniel\AppData\Roaming\Adobe\Acrobat\Privileged\10.0\JavaScripts\local_scripts.js;
             * Name of js function: convertToPdfA
             * Name of Acrobat Profile: Convert to PDF/A-1b (sRGB)
             */
            try
            {
                var username = System.Environment.UserName;
                // removed this fix 1/26/2023
                // if(username == "Printer") { username = "todd"; } // weird here, but necessary
                var local_script_file = @"C:\Users\" + username + @"\AppData\Roaming\Adobe\Acrobat\Privileged\10.0\JavaScripts\local_scripts.js";
                var local_script = string.Empty;
                System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using(var _textStreamReader = new System.IO.StreamReader(
                    _assembly.GetManifestResourceStream("FileSearchMvvm.Models.Utilities.AcrobatJavascripts.js")))
                {
                    var js_txt = _textStreamReader.ReadToEnd();
                    local_script = js_txt;
                }
                var need_to_create_new_file = false;
                if(!System.IO.File.Exists(local_script_file))
                {
                    need_to_create_new_file = true;
                }
                else
                {
                    var current_script = System.IO.File.ReadAllText(local_script_file);
                    if(!local_script.Equals(current_script))
                    {
                        need_to_create_new_file = true;
                    }
                }
                if(need_to_create_new_file)
                {
                    System.IO.File.WriteAllText(local_script_file, local_script);
                }

                if(System.IO.File.Exists(local_script_file)) { return true; }
                else { return false; }
            }
            catch(System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool AreAcrobatSequencesInPlace()
        {
            /* Location of Javascript sequences: 
             * C:\Users\daniel\AppData\Roaming\Adobe\Acrobat\10.0\Sequences\
             * Name of centering sequence: Center Typeset Document.sequ
             */
            try
            {
                var username = System.Environment.UserName;
                var local_script_file = @"C:\Users\" + username + @"\AppData\Roaming\Adobe\Acrobat\10.0\Sequences\Center Typeset Document.sequ";
                var local_script = string.Empty;
                System.Reflection.Assembly _assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using(var _textStreamReader = new System.IO.StreamReader(
                    _assembly.GetManifestResourceStream("FileSearchMvvm.Models.Utilities.Center Typeset Document.sequ")))
                {
                    var js_txt = _textStreamReader.ReadToEnd();
                    local_script = js_txt;
                }
                var need_to_create_new_file = false;
                if(!System.IO.File.Exists(local_script_file))
                {
                    need_to_create_new_file = true;
                }
                else
                {
                    var current_script = System.IO.File.ReadAllText(local_script_file);
                    if(!local_script.Equals(current_script))
                    {
                        need_to_create_new_file = true;
                    }
                }
                if(need_to_create_new_file)
                {
                    System.IO.File.WriteAllText(local_script_file, local_script);
                }

                if(System.IO.File.Exists(local_script_file)) { return true; }
                else { return false; }
            }
            catch(System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}