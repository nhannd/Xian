:: ImageViewerVolume specific postbuild step

call "..\..\..\..\ImageViewer\ImageViewer.bat" %1

echo Executing ImageViewerVolume post-build step

copy "..\..\..\..\ImageViewer\Tools\Volume\bin\%1\ClearCanvas.ImageViewer.Tools.Volume.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\Volume\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.Volume.View.WinForms.dll" ".\plugins"
copy "..\..\..\..\ReferencedAssemblies\vtk\*.*" .\plugins
