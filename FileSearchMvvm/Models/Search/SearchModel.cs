using FileSearchMvvm.Models.CockleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSearchMvvm.Models.Search
{
    class SearchModel
    {
        internal async static Task<List<CockleFile>> SearchCurrentAndBackupT(
            string searchText,
            bool searchEverywhere,
            bool isSingleTicket,
            IProgress<string> progress,
            System.Threading.CancellationToken cancellationToken,
            string error_issue)
        {
            try
            {
                bool exceptionThrownInAwait = false; // tracks excptn in await
                var _files = await Task.Run(() =>
                {
                    List<CockleFile> collectedFiles;

                        // get directories
                        var current = @"\\clbdc02\Current";
                    var dir_current = new System.IO.DirectoryInfo(current);
                    var backup_typesetting = @"\\clbdc02\Backup_Typesetting";
                    var dir_backup = new System.IO.DirectoryInfo(backup_typesetting);
                        //var _files_current2 = dir_current.GetFiles("*" + SearchText + "*", System.IO.SearchOption.AllDirectories)
                        //    .Where(f => f.Name.Contains(SearchText))
                        //    .Where(f => !f.Name.Contains('~'))
                        //    .Where(f => !f.Name.Contains(".xxx"))
                        //    .Select(f => f.FullName);

                        var _files_current = (from file in dir_current.EnumerateFiles(
                                              "*" + searchText + "*", System.IO.SearchOption.AllDirectories)
                                          where file.Name.Contains(searchText) && !file.Name.Contains('~') && !file.Name.Contains(".xxx")
                                          select file.FullName).ToList();



                        // exit if ticket found
                        if (!searchEverywhere)
                    {
                        if (isSingleTicket && _files_current.Count() > 0)
                        {
                            collectedFiles = new List<CockleFile>();
                                //_files_current.ToList().ForEach(f => collectedFiles.Add(new CockleFile(f)));
                                _files_current.ForEach(f => collectedFiles.Add(new CockleFile(f)));
                            return collectedFiles;
                        }
                    }

                        // collect files from backup
                        IEnumerable<string> _files_backup = null;
                    for (var current_year = DateTime.Now.Year; current_year >= 2003; current_year--)
                    {
                            // update ui
                            progress?.Report("Searching files from " + current_year +
                            '\n' + $"{_files_backup?.Count() + _files_current?.Count()}" + " files found");
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        catch (OperationCanceledException)
                        {
                            exceptionThrownInAwait = true;
                            return null;
                        }
                            // search
                            var dir_backup_year = new System.IO.DirectoryInfo(
                                        System.IO.Path.Combine(dir_backup.FullName, current_year.ToString()));
                            //var _temp_backup = dir_backup_year.GetFiles("*" + SearchText + "*", System.IO.SearchOption.AllDirectories)
                            //    .Where(f => f.Name.Contains(SearchText))
                            //    .Where(f => !f.Name.Contains('~'))
                            //    .Where(f => !f.Name.Contains(".xxx"))
                            //    .Select(f => f.FullName);

                            var _temp_backup = from file in dir_backup_year.GetFiles(
                                                 "*" + searchText + "*", System.IO.SearchOption.AllDirectories)
                                           where file.Name.Contains(searchText) && !file.Name.Contains('~') && !file.Name.Contains(".xxx")
                                           select file.FullName;

                            // combine groups
                            if (_files_backup == null) { _files_backup = _temp_backup; }
                        else { _files_backup = _files_backup.Union(_temp_backup); }
                            // return if ticket found
                            if (!searchEverywhere)
                        {
                            if (isSingleTicket && _files_backup.Count() > 0)
                            {
                                collectedFiles = new List<CockleFile>();
                                    //_files_backup.ToList().ForEach(f => collectedFiles.Add(new CockleFile(f)));
                                    _files_backup.ToList().ForEach(f => collectedFiles.Add(new CockleFile(f)));
                                return collectedFiles;
                            }
                        }
                    }
                        // combine current and backup files
                        var _combinedFiles = _files_current.Union(_files_backup);

                    collectedFiles = new List<CockleFile>();
                    _combinedFiles.ToList().ForEach(f => collectedFiles.Add(new CockleFile(f)));
                    return collectedFiles;
                });

                // throw exception again, caught in caller
                if (exceptionThrownInAwait) { cancellationToken.ThrowIfCancellationRequested(); }

                // update ui
                progress?.Report("Search complete");
                return _files;


                //return null;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
        }
    }
}