﻿#pragma checksum "..\..\..\..\Views\PdfView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "65E4BF5C3FCB4DC72F58DB9EAA9F7734"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FileSearchMvvm.ViewModels;
using FileSearchMvvm.ViewModels.Utilities;
using FileSearchMvvm.Views;
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
    /// PdfView
    /// </summary>
    public partial class PdfView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FileSearchMvvm.Views.PdfView pdfViewUserControl;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FileSearchMvvm.ViewModels.PdfViewModel pdfViewModel;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas PdfImage2;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ManCameraImage;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock cameraReadyFolderTextBlock;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas appbar_page_pdf;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\..\Views\PdfView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock pdfSourceFolderTextBlock;
        
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
            System.Uri resourceLocater = new System.Uri("/FileSearchMvvm;component/views/pdfview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\PdfView.xaml"
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
            this.pdfViewUserControl = ((FileSearchMvvm.Views.PdfView)(target));
            return;
            case 2:
            this.pdfViewModel = ((FileSearchMvvm.ViewModels.PdfViewModel)(target));
            return;
            case 3:
            this.PdfImage2 = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.ManCameraImage = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.cameraReadyFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.appbar_page_pdf = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.pdfSourceFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

