﻿#pragma checksum "..\..\..\..\Views\SearchView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7B532524C854221DBE7E9344C60E98DD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FileSearchMvvm.Converters;
using FileSearchMvvm.ViewModels;
using FileSearchMvvm.Views;
using FileSearchMvvm.Views.Utilities;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace FileSearchMvvm.Views {
    
    
    /// <summary>
    /// SearchView
    /// </summary>
    public partial class SearchView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FileSearchMvvm.Views.SearchView searchViewUserControl;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FileSearchMvvm.ViewModels.SearchViewModel searchViewModel;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton searchEverywhereToggle;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton collapseTicketsToggle;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton showLatestFilesToggle;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox searchComboBox;
        
        #line default
        #line hidden
        
        
        #line 221 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar searchProgressBar;
        
        #line default
        #line hidden
        
        
        #line 231 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid searchDataGrid;
        
        #line default
        #line hidden
        
        
        #line 311 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas PdfImage2;
        
        #line default
        #line hidden
        
        
        #line 321 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ManCameraImage;
        
        #line default
        #line hidden
        
        
        #line 330 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock cameraReadyFolderTextBlock;
        
        #line default
        #line hidden
        
        
        #line 344 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas appbar_page_pdf;
        
        #line default
        #line hidden
        
        
        #line 366 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock pdfSourceFolderTextBlock;
        
        #line default
        #line hidden
        
        
        #line 398 "..\..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid pdfDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FileSearchMvvm;component/views/searchview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\SearchView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.searchViewUserControl = ((FileSearchMvvm.Views.SearchView)(target));
            return;
            case 2:
            this.searchViewModel = ((FileSearchMvvm.ViewModels.SearchViewModel)(target));
            return;
            case 3:
            this.searchEverywhereToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 4:
            this.collapseTicketsToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 5:
            this.showLatestFilesToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 6:
            this.searchComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            
            #line 213 "..\..\..\..\Views\SearchView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.searchProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 9:
            this.searchDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 10:
            this.PdfImage2 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 11:
            this.ManCameraImage = ((System.Windows.Controls.Canvas)(target));
            return;
            case 12:
            this.cameraReadyFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.appbar_page_pdf = ((System.Windows.Controls.Canvas)(target));
            return;
            case 14:
            this.pdfSourceFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 15:
            this.pdfDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
