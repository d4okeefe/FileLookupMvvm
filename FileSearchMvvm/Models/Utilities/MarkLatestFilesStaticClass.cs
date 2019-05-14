using FileSearchMvvm.Models.CockleTypes;
using System.Collections.Generic;
using System.Linq;

namespace FileSearchMvvm.Models.Utilities
{
    class MarkLatestFilesStaticClass
    {
        public static List<CockleFile> MarkLatestFiles(List<CockleFile> OriginalList)
        {
            // check that list exists
            if (OriginalList == null || OriginalList.Count == 0) { return null; }

            // get list of unique tickets
            HashSet<int?> tickets = new HashSet<int?>();
            OriginalList.ForEach(f => tickets.Add(f.TicketNumber));

            // create List<CockleFile> to hold latest filenames
            var latest_files = new List<CockleFile>();

            // cycle through tickets
            foreach (var tic in tickets)
            {
                // create list of CockleFiles in ticket
                var cocklefilesforticket = OriginalList.Where(f => tic == f.TicketNumber);

                // create list of types for each CockleFile
                HashSet<string> types = new HashSet<string>();
                foreach (var typ in cocklefilesforticket)
                {
                    types.Add(typ.FileTypeCode);
                }

                // cycle through each type
                if (types != null && types.Count > 0)
                {
                    foreach (var tic_type in types)
                    {
                        if (tic_type == null) continue; // skip if null
                        var single_type = cocklefilesforticket
                            .Where(f => tic_type.Equals(f.FileTypeCode))
                            .OrderBy(f => f.Version).ToArray();
                        single_type[single_type.Length - 1].IsLatestFile = true;
                        latest_files.Add(single_type[single_type.Length - 1]);
                    }
                }
            }
            return latest_files;
        }
    }
}
