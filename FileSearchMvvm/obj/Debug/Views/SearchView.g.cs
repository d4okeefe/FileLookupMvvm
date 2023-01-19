﻿#pragma checksum "..\..\..\Views\SearchView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4F2951479108433F676D138EEAD87920D49619B7F66D3BA311EEA300B46CF0BB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using BenjaminGale.Controls;
using FileSearchMvvm.ViewModels;
using FileSearchMvvm.ViewModels.SearchViewModelFolder;
using FileSearchMvvm.Views;
using FileSearchMvvm.Views.ModalOverlays;
using FileSearchMvvm.Views.Utilities.Converters;
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
        
        
        #line 1 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FileSearchMvvm.Views.SearchView searchViewUserControl;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton searchEverywhereToggle;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton collapseTicketsToggle;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton showLatestFilesToggle;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox searchComboBox;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar searchProgressBar;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid searchDataGrid;
        
        #line default
        #line hidden
        
        
        #line 247 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid pdfDataGrid;
        
        #line default
        #line hidden
        
        
        #line 306 "..\..\..\Views\SearchView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox pdfSourceFolderTextBlock;
        
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
            
            #line 1 "..\..\..\Views\SearchView.xaml"
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
            this.searchEverywhereToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 3:
            this.collapseTicketsToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 4:
            this.showLatestFilesToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 5:
            this.searchComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            
            #line 102 "..\..\..\Views\SearchView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.searchProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 8:
            this.searchDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            this.pdfDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 10:
            this.pdfSourceFolderTextBlock = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

