using Acrobat;
using FileSearchMvvm.Models.CockleTypes;
using System;

namespace FileSearchMvvm.Models.CenterPdfText
{
    class CenterCameraReadyBrief
    {
        public ViewModels.SearchViewModelFolder.CenteredCoverType BriefSize { get; set; }
        public CockleFilePdf NewFileCreated { get; set; }
        public CockleFilePdf OriginalFile { get; set; }
        public CenterCameraReadyBrief(
            CockleFilePdf _originalFile,
            string _new_file_name,
            ViewModels.SearchViewModelFolder.CenteredCoverType _brief_size)
        {
            // if already 8.5 x 11 --> to 8.5 x 11, crop, then center
            // if smaller than 8.5x11 --> center small page on 8.5x11


            // initialize properties
            BriefSize = _brief_size;
            OriginalFile = _originalFile;
            var new_file_name = _new_file_name;

            if(BriefSize == ViewModels.SearchViewModelFolder.CenteredCoverType.Letter)
            {
                centerLetterSizePdf(new_file_name);
            }
            else
            {
                centerBookletSizePdf(new_file_name);
            }
            if(System.IO.File.Exists(new_file_name))
            {
                NewFileCreated = new CockleFilePdf(new_file_name, OriginalFile, Utilities.SourceFileTypeEnum.Camera_Ready);
            }
        }

        private void centerBookletSizePdf(string new_file_name)
        {
            CAcroApp app = new AcroApp();           // Acrobat
            CAcroPDDoc doc = new AcroPDDoc();       // First document
            CAcroPDDoc docToAdd = new AcroPDDoc();  // Next documents

            // use reflection to center the pdf
            try
            {
                var opened = doc.Open(OriginalFile.FullName);
                if(opened)
                {
                    object js_object = doc.GetJSObject();
                    Type js_type = js_object.GetType();
                    object[] js_param = { };
                    string script_name = Models.Utilities.AcrobatJS.Javascripts
                                    [Models.Utilities.LocalJavascripts.arePagesLargerThanBooklet];
                    var test = js_type.InvokeMember(script_name,
                        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                        null, js_object, js_param);
                    if((bool)test)
                    {
                        script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.centerCenteredDocOnBookletPages];
                        js_type.InvokeMember(script_name,
                            System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                            null, js_object, js_param);
                    }
                    // test that the file exists
                    if(!doc.Save(1, new_file_name)) throw new Exception();
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                doc.Close();
                app.CloseAllDocs();
                app.Exit();

                doc = null;
                docToAdd = null;
                app = null;

                GC.Collect();
            }
        }

        private string generateNewFileName(string txt)
        {
            var directory = System.IO.Path.GetDirectoryName(OriginalFile.FullName);
            var ticket_atty = OriginalFile.TicketPlusAttorney;
            var _newfilename = string.Empty;
            var i = 0;
            do
            {
                if(i == 0)
                {
                    if(!string.IsNullOrEmpty(ticket_atty))
                    {
                        _newfilename = System.IO.Path.Combine(directory, ticket_atty + " " + txt + ".pdf");
                    }
                    else
                    {
                        _newfilename = System.IO.Path.Combine(directory, txt + ".pdf");
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(ticket_atty))
                    {
                        _newfilename = System.IO.Path.Combine(directory, ticket_atty + " " + txt + "_" + i + ".pdf");
                    }
                    else
                    {
                        _newfilename = System.IO.Path.Combine(directory, txt + "_" + i + ".pdf");
                    }
                }
                i++;
            } while(System.IO.File.Exists(_newfilename));
            return _newfilename;
        }
        private void centerLetterSizePdf(string new_file_name)
        {
            CAcroApp app = new AcroApp();           // Acrobat
            CAcroPDDoc doc = new AcroPDDoc();       // First document
            CAcroPDDoc docToAdd = new AcroPDDoc();  // Next documents

            // use reflection to center the pdf
            try
            {
                var opened = doc.Open(OriginalFile.FullName);
                if(opened)
                {
                    object js_object = doc.GetJSObject();
                    Type js_type = js_object.GetType();
                    object[] js_param = { };
                    string script_name = Models.Utilities.AcrobatJS.Javascripts
                                    [Models.Utilities.LocalJavascripts.arePagesSmallerThanLetter];
                    var test = js_type.InvokeMember(script_name,
                        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                        null, js_object, js_param);
                    if((bool)test)
                    {
                        script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.centerCenteredDocOnLetterPages];
                        js_type.InvokeMember(script_name,
                            System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                            null, js_object, js_param);
                    }
                    // test that the file exists
                    if(!doc.Save(1, new_file_name)) throw new Exception();
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                doc.Close();
                app.CloseAllDocs();
                app.Exit();

                doc = null;
                docToAdd = null;
                app = null;

                GC.Collect();
            }
        }
    }
}