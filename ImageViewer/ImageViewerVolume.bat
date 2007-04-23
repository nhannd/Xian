:: ImageViewer specific postbuild step

echo Executing ImageViewer post-build step

:: Copy database file

md "c:\dicom_datastore"

IF NOT EXIST "C:\dicom_datastore\viewer.sdf" copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\empty_viewer.sdf" "C:\dicom_datastore\viewer.sdf"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\empty_viewer.sdf" "C:\dicom_datastore\empty_viewer.sdf"

:: Copy Sql CE files
copy "..\..\..\..\Dicom\DataStore\NHibernateDriver\bin\%1\ClearCanvas.Dicom.DataStore.NHibernateDriver.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlceca30.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlcecompact30.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlceer30EN.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlceme30.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlceoledb30.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlceqp30.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\sqlcese30.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\System.Data.SqlServerCe.dll" ".\Common"

:: Copy Hibernate configuration file
copy "..\..\..\..\Dicom\DataStore\AuxiliaryFiles\ClearCanvas.Dicom.DataStore.cfg.xml" "."

:: Copy Lut Preset config files
copy "..\..\..\..\ImageViewer\Tools\Standard\bin\%1\LutPresets\DefaultLutPresets.xml" "."
copy "..\..\..\..\ImageViewer\Tools\Standard\bin\%1\LutPresets\DefaultLutPresetKeyAssignments.xml" "."

:: Copy Dicom assemblies	
copy "..\..\..\..\Dicom\bin\%1\ClearCanvas.Dicom.dll" ".\Common"
copy "..\..\..\..\Dicom\OffisWrapper\csharp\%1\ClearCanvas.Dicom.OffisWrapper.dll" ".\Common"
copy "..\..\..\..\Dicom\OffisWrapper\cppwrapper\%1\OffisDcm.dll" ".\Common"
copy "..\..\..\..\Dicom\DataStore\bin\%1\ClearCanvas.Dicom.DataStore.dll" ".\Common"

:: Copy ImageViewer plugins
copy "..\..\..\..\Codecs\bin\%1\ClearCanvas.Codecs.dll" ".\plugins"
copy "..\..\..\Explorer\bin\%1\ClearCanvas.Desktop.Explorer.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\bin\%1\ClearCanvas.ImageViewer.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Services\bin\%1\ClearCanvas.ImageViewer.Services.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Services\Tools\bin\%1\ClearCanvas.ImageViewer.Services.Tools.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Services\Tools\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Services.Tools.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Services\bin\%1\ClearCanvas.ImageViewer.Services.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\View\WinForms\bin\%1\ClearCanvas.ImageViewer.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Measurement\bin\%1\ClearCanvas.ImageViewer.Tools.Measurement.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Standard\bin\%1\ClearCanvas.ImageViewer.Tools.Standard.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Standard\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.dll" ".\plugins"
copy "..\..\..\Edit\bin\%1\ClearCanvas.Desktop.Edit.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Explorer\Local\bin\%1\ClearCanvas.ImageViewer.Explorer.Local.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Explorer\Local\Tools\bin\%1\ClearCanvas.ImageViewer.Explorer.Local.Tools.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Explorer\Local\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Explorer\Dicom\bin\%1\ClearCanvas.ImageViewer.Explorer.Dicom.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Explorer\Dicom\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Layout\Basic\bin\%1\ClearCanvas.ImageViewer.Layout.Basic.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Layout\Basic\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Layout.Basic.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Rendering\BilinearInterpolation\bin\%1\ClearCanvas.ImageViewer.Rendering.BilinearInterpolation.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\StudyFinders\Remote\bin\%1\ClearCanvas.ImageViewer.StudyFinders.Remote.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\StudyFinders\LocalDataStore\bin\%1\ClearCanvas.ImageViewer.StudyFinders.LocalDataStore.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\StudyLoaders\LocalDataStore\bin\%1\ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\AnnotationProviders\bin\%1\ClearCanvas.ImageViewer.AnnotationProviders.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Volume\bin\%1\ClearCanvas.ImageViewer.Tools.Volume.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Volume\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.Volume.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ReferencedAssemblies\vtk\*.*" .\plugins
copy "..\..\..\..\Utilities\DicomEditor\bin\%1\ClearCanvas.Utilities.DicomEditor.dll" ".\plugins"
copy "..\..\..\..\Utilities\DicomEditor\View\WinForms\bin\%1\ClearCanvas.Utilities.DicomEditor.View.WinForms.dll" ".\plugins"

:: Copy config files
copy "..\..\..\..\ImageViewer\actionmodels.xml "."
