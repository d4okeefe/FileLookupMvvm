namespace FileSearchMvvm.ViewModels.Utilities
{
    public static class StaticSystemTests
    {
        public static bool IsWordInstalled()
        {
            bool isInstalled = false;
            using (var regWord = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("Word.Application"))
            {
                if (regWord != null)
                {
                    isInstalled = true;
                }
            }
            return isInstalled;
        }
        public static bool IsAdobePdfPrinterAvailable()
        {
            bool isInstalled = false;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer == "Adobe PDF")
                {
                    isInstalled = true;
                    break;
                }
            }
            return isInstalled;
        }
        public static bool IsAdobePdfBookletPrinterAvailable()
        {
            bool isInstalled = false;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer == "Adobe PDF Booklet")
                {
                    isInstalled = true;
                    break;
                }
            }
            return isInstalled;
        }
        public static bool Is554KonicaPrintToFileDriverInstalled()
        {
            bool isInstalled = false;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer == "554 Print to File")
                {
                    isInstalled = true;
                    break;
                }
            }
            return isInstalled;
        }
        public static bool IsGhostscriptInstalled()
        {
            var blah = Microsoft.Win32.Registry.LocalMachine;

            //check that Ghostscript is installed
            var key1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\GPL Ghostscript");
            var key2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\AFPL Ghostscript");
            return !(key1 == null && key2 == null);
        }
    }
}
