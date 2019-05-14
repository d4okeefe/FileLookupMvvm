using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearchMvvm.ViewModels
{
    static class ViewModelLocator
    {
        private static SearchViewModelFolder.SearchViewModel myViewModel = new SearchViewModelFolder.SearchViewModel();
        public static SearchViewModelFolder.SearchViewModel MyViewModel
        {
            get
            {
                return myViewModel;
            }
        }
    }
}
