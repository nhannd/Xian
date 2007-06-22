:: ImageViewerSamples specific postbuild step

call "..\..\..\..\ImageViewer\ImageViewer.bat" %1

echo Executing ImageViewerSamples post-build step

copy "..\..\..\..\Desktop\Applets\WebBrowser\bin\%1\ClearCanvas.Desktop.Applets.WebBrowser.dll" ".\plugins"
copy "..\..\..\..\Desktop\Applets\WebBrowser\View\WinForms\bin\%1\ClearCanvas.Desktop.Applets.WebBrowser.View.WinForms.dll" ".\plugins"

copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\Filter\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\Filter\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter.View.WinForms.dll" ".\plugins"

copy "..\..\..\..\ReferencedAssemblies\NPlot.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\RoiHistogram\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\RoiHistogram\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram.View.WinForms.dll" ".\plugins"

copy "..\..\..\..\ImageViewer\Tools\Volume\bin\%1\ClearCanvas.ImageViewer.Tools.Volume.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Volume\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.Volume.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ReferencedAssemblies\vtk\*.*" .\plugins
