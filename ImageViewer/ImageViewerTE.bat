:: ImageViewerVolume specific postbuild step

call "..\..\..\..\ImageViewer\ImageViewer.bat" %1

echo Executing ImageViewerTE post-build step

copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\DynamicTe\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe.dll" ".\plugins"
