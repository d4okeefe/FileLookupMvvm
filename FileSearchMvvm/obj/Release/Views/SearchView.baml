   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  I EFileSearchMvvm, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:FileSearchMvvm.ViewsFileSearchMvvm.Views E'clr-namespace:FileSearchMvvm.ViewModelsFileSearchMvvm.ViewModels q=clr-namespace:FileSearchMvvm.ViewModels.SearchViewModelFolder/FileSearchMvvm.ViewModels.SearchViewModelFolder e7clr-namespace:FileSearchMvvm.Views.Utilities.Converters)FileSearchMvvm.Views.Utilities.Converters W SModalContentPresenter, Version=1.0.6527.22304, Culture=neutral, PublicKeyToken=null\Bclr-namespace:BenjaminGale.Controls;assembly=ModalContentPresenterBenjaminGale.Controls O,clr-namespace:FileSearchMvvm.Views.UtilitiesFileSearchMvvm.Views.Utilities %   FileSearchMvvm.Views.SearchView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6sys&clr-namespace:System;assembly=mscorlib     4local"clr-namespace:FileSearchMvvm.Views     >
viewModels'clr-namespace:FileSearchMvvm.ViewModels     YsearchViewModel=clr-namespace:FileSearchMvvm.ViewModels.SearchViewModelFolder     N
converters7clr-namespace:FileSearchMvvm.Views.Utilities.Converters     dmodalContentPresenterBclr-namespace:BenjaminGale.Controls;assembly=ModalContentPresenter     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  <	utilities,clr-namespace:FileSearchMvvm.Views.Utilities  U QGongSolutions.WPF.DragDrop, Version=1.1.0.0, Culture=neutral, PublicKeyToken=nullddurn:gong-wpf-dragdrop  -     >�Name$  searchViewUserControl��-  'System.Windows.Input.KeyboardNavigation   TabNavigation$ Cycle=� J� InputBindings �� 
 �� Key$	 Esc�� �� Command �� *!CloseModalContentOverlayCommand+ � 	Resources G AFileSearchMvvm.Views.Utilities.Converters.SizePercentageConverter   sizePercentageConverter  	BoolToVis # SearchAndViewOptionsGroupHeader �� MergedDictionaries  Key �� 
TargetType �� BasedOn	 �� Property
 �� Value �� Triggers P� Property P� Value �� Source*  $System.Windows.Controls.DataGridCell��  �� �� PopupRadioButtonStyle " 	�# [Bd�.��� 	 � ��$	
 0 2q��� 	 � ��$
 6q��� 	 � ��$
 0q� P�  �@d�	IsChecked$
 TrueY�. ��� 	 � ��$
 DemiBold"��� $) #..\CustomControls\CustomToggle.xaml?��� $- '..\CustomControls\GroupHeaderStyle.xaml?��� $* $..\CustomControls\AndroidToggle.xaml?��� �� " ��TextBoxStyle .��� 	 �@��
IsReadOnly
 . �� 	 � ��

 ���� �� 	 � ��$
 0q��� 	 � ��$
 Center=��� 	 � ��$
 Center=��� 	 � ��$
 6q��� " ��PdfTabButtonStyle .��� 	 � ��$	
 0 2q��� 	 � ��$
 6q��� 	 � ��$
 0q��� �� "  datagridCellStyle .��� 	 � ��$
 0q��� 	 �@�FocusVisualStyle
 Z� %c   &	        &	      &	 
       �� ��Search/View Options��'��       '��       '         � DataContext E  ?FileSearchMvvm.ViewModels.SearchViewModelFolder.SearchViewModel  -   searchVM��.���� $��0q�.@��� $��Search for typeset files��{�.    	IsTabStop .  $��0q�.���  � RowDefinitions �� $��3* ��� $��8* � � ColumnDefinitions �� $��* ��� $��* ��� $��* �.+��� #��[ $��0��$��0��.���  �� $��* ��� $��* ��� $��* � �� $��* ��� $��* �.+��� $��0��$��0�� �� Target ��  �� ElementName$ searchEverywhereToggle��.���� $��WrapWithOverflow=�Search everywhere?�� $��1��$��0�� �� $ collapseTicketsToggle��.���� $��WrapWithOverflow=�Collapse tickets?�� $��2��$��0�� �� $ showLatestFilesToggle��.���� $��WrapWithOverflow=�Display only latest files?d� -    d�Name$ searchEverywhereToggle��$��0��$��1��$	��0 6q�  AndroidToggle#��[ ��.   d� 	IsChecked �� *SearchEverywhere+d� -   $ collapseTicketsToggle��$��1��$��1��$	��0 6q�#��[ ��.   �� *CollapseTickets+d� -   $ showLatestFilesToggle��$��2��$��1��$	��0 6q�#��[ ��.  $��Right=� �� *ShowLatestFiles+(� $��0��$��2��$��5q�.���� $#��..\ImageFiles\cockle-logo.png���� $��Search��$��0��$��1��$��1q�.���� .+��� $��Enter _search term here�� �� $ searchComboBox���� -   $��searchComboBox��$��6q� �� 
IsEditable .  . $��0��$	��5 0q�$��0 0 0 2q� �� Text ��  �� Path$ 
SearchText����� *UserSearchTerms+���� " �� ^�  ^� 
Conditions ��  �� Binding �� $ searchProgressBar��$ IsIndeterminate� �� Value True.0��� 	 � {�
 .  �� $��0 10 0 0q� . $��0q�$��5q����� *RunSearchAndConvert+���� .+��� $��WrapWithOverflow=�$
��Left=�$=��7One Step: Type ticket & click to find and convert files��]� $��2��$��1��.+��� $��Search��$	��Top=�.���� .+��� $	��0 2q�$��6q�$��Search��$	��150�� �� 	IsDefault .  . $��0q����� *	RunSearch+�� $	��0 2q�$��6q�$��Cancel/Reset��$	��150�� �� IsCancel .  . $��0q����� *RunCancelSearch+�� $	��0 2q�$��6q�$��Open Selected File��$	��150�� . $��0q����� *RunOpenSelectedFile+�� $	��0 2q�$��6q�$��Open Source Folder��$	��150�� . $��0q����� *RunOpenSourceFolder+�� $	��0 2q�$��6q�$$��Open Folder of Converted Files��$	��150��$��0q����� *RunOpenPdfSourceFolder+�� $	��0 2q�$��6q�$��Convert to PDF��$	��150�� . $��0q����� *RunConvertToPdf+�� $	��0 2q�$��5q�$"��Save latest files to scratch��$	��150�� . $��0q����� *RunSearchAndSaveToScratch+�� $	��0 2q�$y�	Collapsed=�$��6q�$��Convert to PDF for Proof��$	��150�� . $��0q����� *RunConvertToPdfProof+�� -   $��Close��$	��0 2q�$��6q� . $��0q��� $��0q�.���� .+���   TextBoxStyle#��[ ���� *UpdateLabel+� -   $��searchProgressBar��$	��0 2q�$��10��$��0q� � IsIndeterminate �� *IsExecutingSearch+y��� *IsExecutingSearch+  �� 	Converter#  [ � $��0��$��1��$��2��.+�&   System.Windows.Controls.DataGrid  -	   $��searchDataGrid��!   AutoGenerateColumns! .  "   CanUserAddRows" .  #   CanUserDeleteRows# .    datagridCellStyle$   	CellStyle#$ [ %   GridLinesVisibility$% 
Horizontal=�&   HeadersVisibility$& Column=�'   HorizontalGridLinesBrush
' �����$(   HorizontalScrollBarVisibility$( Disabled=�)   
IsReadOnly) . ��.   .  $
 None=�*   SelectionMode$* Extended=�+   SelectionUnit$+ FullRow=�$��Visible=����� $ FilesListCollectionView�, �� SelectedItem, �� *SelectedFile+��Z� ��Z�  �� $ Return�� �� *RunOpenSelectedFile+d� - d� Gesture$- LeftDoubleClicka� �� *RunOpenSelectedFile+ . �� Color��%C   (��       * SystemColors.HighlightBrushKey+)�� $. 	LightGray��/ �� 
GroupStyle/ (  "System.Windows.Controls.GroupStyle    GroupHeaderStyle0   ContainerStyle#0 [ 1   RowStyle1 �� )  #System.Windows.Controls.DataGridRow"   ^� .0��� 	 � y�$
 	Collapsed=� ��  �� $ IsLatestFile�	 False��  �� $ showLatestFilesToggle��$ 	IsChecked� TrueP�  � y�$ 	Collapsed=�. ��� 	 �@ 
IsSelected
 .  .  (System.Windows.Interactivity.Interaction2   Triggers2 /	  )System.Windows.Interactivity.EventTrigger	  3 	  	EventName$3 SelectionChanged��.
  (System.Windows.Interactivity.TriggerBase4 
  Actions.4 6  0System.Windows.Interactivity.InvokeCommandAction  5   Command5 �� *RunSelectionChanged+6   CommandParameter6 �� $ SelectedItems�$ searchDataGrid��7   Columns7 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn8   Header$8 Filename��9   MinWidth$	9 200Q�1  +System.Windows.Controls.DataGridBoundColumn:   Binding: �� $ Filename�  $8 	File type��$	9 115Q�: �� $ FileType�  $
8 Year��$	9 115Q�: �� $
 Year��� $��View PDF files�� .  $��0q�.��1  +BenjaminGale.Controls.ModalContentPresenter  ;   IsModal; �� *ModalOverlayIsVisible+<   Content.< �  �� $
��1.0* ��� $
��0.8* � �� $��1* ��� $��1* �.+�� $��0��$��0��.+��� .+��� $��Manage PDF Files��$
��Left=�$��6q����� = �� RelativeSource= �� > �� AncestorType$> 
StackPanelL�$ ActualWidth�.���� .+���   PdfTabButtonStyle#��[ ���� *RunOpenPdfSourceFolder+.���� $��WrapWithOverflow=� Open folder of converted files�� #��[ ���� *RunOpenSelectedPdfFile+.���� $��WrapWithOverflow=�Open selected file�� #��[ ���� *RunClearPdfFiles+.���� $��WrapWithOverflow=�Clear files in list�� ? �� Orientation$? 
Horizontal=�.+��� $��6q� �� $ pdfSourceFolderTextBlock��.��Source folder:�� -
   $��pdfSourceFolderTextBlock��@ �� Padding$@ 6q�$��WrapWithOverflow=����� $% DestinationFolderConvertedFiles�A �� Mode$A TwoWay=�B �� UpdateSourceTrigger$B PropertyChanged=��� $��Collect PDF Files��$
��Left=�$��6q����� = �� $> 
StackPanelL�$ ActualWidth�.���� .+��� #��[ ���� *RunGetFilesFromScratchFolder+.���� $��WrapWithOverflow=�Get files from C:\scratch�� #��[ ���� *" RunGetFilesFromCameraReadyFolder+.���� $��WrapWithOverflow=��� $$�Select files from Camera Ready��� $��0��$��1��.+��� .+��� $��Create/Alter PDF Files��$
��Left=�$��6q����� = �� $> 
StackPanelL�$ ActualWidth�.���� .+��� #��[ ���� *" CombineSelectedFilesPopupCommand+.���� $��WrapWithOverflow=�'%Combine selected files into a new pdf�� $��Letter��#��[ ���� *CenterPdfCommand+.���� $��WrapWithOverflow=�!Center typeset pdf (8½" x 11")�� $��Booklet��#��[ ���� *CenterPdfCommand+y��� $# ShowHideCenterPdf_BookletSize�#  [ .���� $��WrapWithOverflow=�#!Center typeset pdf (6⅛" x 9¼")�� #��[ ���� *ConvertToPdfACommand+.���� $��WrapWithOverflow=�Convert document to PDF/A�� $��6q�#��[ ���� *ImposePdfOptionsCommand+.���� $��WrapWithOverflow=�Impose typeset pdf�� #��[ ���� *UpdateLabelPdf+$B PropertyChanged=�  -   C  Name$C pdfDataGrid��$��1��$��2��$��2��! .  " .  # .  $( Disabled=�) . ��.   .  $
 None=�$* Extended=�$+ FullRow=�$��Visible=�$��1q�D J� 	AllowDropD . $
% None=�)  #GongSolutions.Wpf.DragDrop.DragDropE   UseDefaultDragAdornerE . F   IsDragSourceF . G   IsDropTargetG . #$ [ ���� $ FilesConvertedToPdf�, �� *SelectedPdfFile+$B PropertyChanged=���Z� 3  -FileSearchMvvm.Views.Utilities.FocusExtensionH   	IsFocusedH �� *PdfDataGridIsSelected+���� I �� EndPoint$I 0.5 4&�J � MappingMode$J RelativeToBoundingBox=�K �� 
StartPoint$K 0.5,0&�.��� L � Color$L White��M � Offset$M 0.105Q�� $
L Gray��$M 1Q� �� $ Return�� �� *RunOpenSelectedPdfFile+d� $- LeftDoubleClicka� �� *RunOpenSelectedPdfFile+7   $8 Filename��$	9 200Q�N   MaxWidth$	N 350Q�O   
FontWeight$O DemiBold"�: �� $ Filename�P   ElementStyleP �� " ��.��� 	 �@��Padding$
 3q�  $8 	File type��$	9 125Q�: �� $ FileType�P �� " ��.��� 	 �@��Padding$
 3q�  $8 
# of Pages��$9 75Q�: �� $ PageRange.TotalPages�P �� " ��.��� 	 �@��Padding$
 3q�  $8 
First page��$9 75Q�: �� $ PageRange.FirstPage�P �� " ��.��� 	 �@��Padding$
 3q�  $8 	Last page��$9 75Q�: �� $ PageRange.LastPage�P �� " ��.��� 	 �@��Padding$
 3q�2 	  $3 SelectionChanged��.4   5 �� *RunPdfSelectionChanged+6 �� $ SelectedItems�$ pdfDataGrid��Q   ModalContentQ � .+�� y��� *" SelectImpositionDetailsIsVisible+#  [ .+�� -   R �Name$!R selectImpositionOutsideGrid��
���   �S J� Opacity$	S 0.5Q� d� $- 	LeftClicka� �� *!CloseModalContentOverlayCommand+�� 
���   �$��1q�T �� CornerRadius$T 10��
�������$��50q�$��Stretch=�$��Stretch=�U �� Padding$U 10q�._��� .+�,� $��Hidden=�$
��Auto=�.��/� V /� 
FontFamily$V Segoe)�W /� FontSize$W 12(�.T�J� X �� TextAlignment$X Center=�.*�.*��� .�IMPOSE A PDF FILEJ� .*�Before imposing, make sure:�� .<��� .9�J� .*�%#that blank pages have been inserted�� .9�J� .*�31if saddle stitch, that the entire file is present�� $��Position of Original��.���  �� ��  �� �� .+�	� $��Typeset (upper left)��$��0��$��2�� 	 PopupRadioButtonStyle#��[	  �� *IsTypesetPdf+	� $��Camera ready (centered)��$��1��$��0��#��[	  �� *IsCameraReadyCenteredPdf+	� $��Camera ready (offset)��$��1��$��1��#��[	  �� *IsCameraReadyOffsetPdf+�� $��Type of bind (choose one)��.��A� Y A� Columns$Y 2��.+�	� $��Saddle stitch��#��[	  �� *IsSaddleStitchPdf+	� $��Perfect bind��#��[	  �� *IsPerfectBindPdf+�� $#��Type of document (choose one)��.���� .+�A� $Y 2��.+�	� #��[	 $��Pdf includes a cover��Z 	� 	GroupName$Z pdfTypeGroup�� �� *HasCoverPdf+	� #��[	 $��Pdf has only brief pages��$Z pdfTypeGroup�� �� *NoCoverBriefOnlyPdf+A� $Y 2��$��0 25 0 0q�.+��� #��[ ���� *ImposedPdfCommand+.��Combine files�� #��[ ���� *!CloseModalContentOverlayCommand+.��Cancel�� #��[ $��6q�[ �� TextWrapping$[ WrapWithOverflow=����� *ImposedSingleFileGuiUpdate+� y��� *SetFileOrderModalIsVisible+#  [ .+�� 
���   �$	S 0.5Q� d� $- 	LeftClicka� �� *!CloseModalContentOverlayCommand+�� $	��250��
���   �$��1q�$T 10��
�������$
��10 5q�$��Stretch=�$��Stretch=�$U 10q�._��  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$? 
Horizontal=�.+��� $��5q�$��DemiBold"�$��Are files in order?���� $��5q�$��DemiBold"�$��If not, rearrange them.��  -   $C pdfDataGridOrdered��$��1��$��20q�! .  " .  # .  $( Disabled=�) . ��.   .  $
 None=�$* Extended=�$+ FullRow=�$��Visible=�$��1q�D . $
% None=�$��14(�E . F . G . #$ [ ���� $! FilesConvertedToPdf_Ordered�, �� *SelectedPdfFile_Ordered+��Z� \ �� AlternationCount\ �� *#!FilesConvertedToPdf_Ordered.Count+2 	  $3 SelectionChanged��.4   5 �� * RunPdfSelectionChanged_Ordered+6 �� $ SelectedItems�$ pdfDataGridOrdered��1 �� "  .��� 	 � ��
 Z� ]   RowHeaderStyle] �� :  4System.Windows.Controls.Primitives.DataGridRowHeader"  .��� 	 � ��$

 10 0q��� 	 � ��
 �� $% (ItemsControl.AlternationIndex)�= �� $> DataGridRowL����� $I 0.5 4&�$J RelativeToBoundingBox=�$K 0.5,0&�.��� $L White��$M 0.105Q�� $
L Gray��$M 1Q�7   $8 Filename��: �� *
Filename+P �� " ��.��� 	 �@��Padding$

 15 3q��� 	 � ��$
 DemiBold"�  $8 	File type��: �� *
FileType+P �� " ��.��� 	 �@��Padding$

 15 3q�A� $Y 2��.+��� #��[ ���� *+)CombineFilesConvertedToPdf_OrderedCommand+.��Combine files�� #��[ ���� *!CloseModalContentOverlayCommand+.��Cancel� y��� *CoverLengthSelectorIsVisible+#  [ .+�� 
���   �$	S 0.5Q� d� $- 	LeftClicka� �� *!CloseModalContentOverlayCommand+�� $	��250��
���   �$��1q�$T 10��
�������$
��10 5q�$��Center=�$��Center=�$U 10q�._��� .+��� $��5q�$��DemiBold"�$��Select cover length:��� $	��100�� �� �� ��  �� �� .+��� $��0��$��0��
�������
�������$��48������ *#!CenterPdfSelectCoverLengthCommand+.��	48 pica�� $��0��$��1��
�������
�������$��49������ *#!CenterPdfSelectCoverLengthCommand+.��	49 pica�� $��1��$��0��
�������
�������$��50������ *#!CenterPdfSelectCoverLengthCommand+.��	50 pica�� $��1��$��1��
�������
�������$��51������ *#!CenterPdfSelectCoverLengthCommand+.��	51 pica�� $��2��$��0��$��2��
�������
�������$��NoCover������ *#!CenterPdfSelectCoverLengthCommand+.��Typeset (no cover)� y��� * ScratchFolderSelectorIsVisible+#  [ .+�� 
���   �$	S 0.5Q� d� $- 	LeftClicka� �� *!CloseModalContentOverlayCommand+�� $	��250��
���   �$��1q�$T 10��
�������$
��10 5q�$��Center=�$��Center=�$U 10q�._��  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��Center=�$��Folder in scratch��$
��Bold"�$��6q�$@ 6q��� $��1����. 
������ .���� ��.   .  $
 None=�^ �� SelectionMode$^ Extended=�
������ 
������ ���� $ FoldersFoundInScratch�, �� *SelectedFolderInScratch+���� .R��� ���� *ShortFolderName+$A OneWay=� ��%C   (��       * SystemColors.HighlightBrushKey+)�� $. 	LightGray��� $��2��$	��0 6q� �� �� .+��� $��0��#��[ ���� *SelectFolderInScratchCommand+.��Select folder�� $��2��#��[ ���� *!CloseModalContentOverlayCommand+.��Cancel� y��� *" CameraReadyFileSelectorIsVisible+#  [ .+�� 
���   �$	S 0.5Q� d� $- 	LeftClicka� �� *!CloseModalContentOverlayCommand+�� $	��250��
���   �$��1q�$T 10��
�������$
��10 5q�$��Center=�$��Center=�$U 10q�._��  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$? 
Horizontal=�$��6q�.+��� .�� Show files with ticket number:�� $	��100��$��0 0 0 2q�$��Center=�$��Center=�$��6q�$��6q����� *CameraReadySearchText+$B PropertyChanged=���  . $��20 6 6 6q�$
��20 6q�$��0q����� *-+ShowOnlyCameraReadySearchTextTicketsCommand+.��
Find files�� $��6 6 20 6q�$
��20 6q�$��0q����� *20ShowAllCameraReadyFilesAndClearSearchTextCommand+.��Show all files  $��1��$* Extended=�! .  $+ FullRow=�$��Visible=�$��Disabled=�$% 
Horizontal=�#$ [ ���� $ FilesFoundInCameraReady�, �� *SelectedFileInCameraReady+��Z� ��Z� _   RowBackground_ Z� 7   $8 Ticket��$9 50Q�: �� $ Ticket�$A OneWay=�  $8 Attorney��$9 50Q�: �� $ Attorney�$A OneWay=�  $
8 File��: �� $ ShortFileName�$A OneWay=� ��%�   '        (�� =     * SystemColors.HighlightBrushKey+)�� "  .��� 	 �@ 
IsSelected
 �� *
IsSelected+�� $. ForestGreen�� d� $- LeftDoubleClicka� �� *$"OpenSelectedCameraReadyFileCommand+� $��2�� �� �� .+��� $��0��#��[ ���� *.,SaveSelectedCameraReadyFilesToScratchCommand+.��" Select files and save to scratch�� $��1��#��[ ���� *!CloseModalContentOverlayCommand+.��Cancel �� $	 Esc�� �� *!CloseModalContentOverlayCommand+