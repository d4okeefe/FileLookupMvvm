using Acrobat;
using FileSearchMvvm.Models.CockleTypes;
using System;

namespace FileSearchMvvm.Models.Imposition
{
    class NUpCircuitCourtCoverOn8pt5x23
    {

            private CockleFilePdf selectedPdfFile;

            public CockleFilePdf NewFileCreated { get; set; }

            public NUpCircuitCourtCoverOn8pt5x23(CockleFilePdf _selectedFile)
            {
                //if(!Utilities.AcrobatJS.AreAcrobatJavascriptsInPlace()) { throw new Exception(); }

                selectedPdfFile = _selectedFile;

                // develop new file name
                var directory = System.IO.Path.GetDirectoryName(selectedPdfFile.FullName);
                var ticket_atty = selectedPdfFile.TicketPlusAttorney;
                var _newfilename = PdfUtilities.GenerateFilenameForNewPdf(directory, "nUpOn8pt5x23", ticket_atty);

                nUpCoverDoc(_newfilename);

                if(System.IO.File.Exists(_newfilename))
                {
                    NewFileCreated = new CockleFilePdf(_newfilename, selectedPdfFile, Utilities.SourceFileTypeEnum.UnrecognizedCentered);
                }
            }
            private void nUpCoverDoc(string new_file_name)
            {
                CAcroApp app = new AcroApp();           // Acrobat
                CAcroPDDoc doc = new AcroPDDoc();       // First document
                CAcroPDDoc docToAdd = new AcroPDDoc();  // Next documents

                // use reflection to center the pdf
                try
                {
                    var opened = doc.Open(selectedPdfFile.FullName);
                    if(opened)
                    {
                        object js_object = doc.GetJSObject();
                        Type js_type = js_object.GetType();
                        object[] js_param = { };
                        string script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.nUpCircuitCoverOn8pt5by23];
                        js_type.InvokeMember(script_name,
                            System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                            null, js_object, js_param);
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
