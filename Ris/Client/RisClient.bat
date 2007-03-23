:: RIS-client specific postbuild step

echo Executing RIS-client post-build step

:: Copy config files
copy "..\..\..\..\Ris\Client\Adt\actionmodels.xml "."


:: Copy Shared libraries	
copy "..\..\..\..\Enterprise\Common\bin\%1\ClearCanvas.Enterprise.Common.dll" .\plugins
copy "..\..\..\..\Ris\Application\Common\bin\%1\ClearCanvas.Ris.Application.Common.dll" .\plugins

:: Client modules
copy "..\..\..\..\Jscript\bin\%1\ClearCanvas.Jscript.dll" .\plugins
copy "..\..\..\..\Ris\Client\bin\%1\ClearCanvas.Ris.Client.dll" .\plugins
copy "..\..\..\..\Ris\Client\View\WinForms\bin\%1\ClearCanvas.Ris.Client.View.WinForms.dll" .\plugins
copy "..\..\..\..\Ris\Client\Admin\bin\%1\ClearCanvas.Ris.Client.Admin.dll" .\plugins
copy "..\..\..\..\Ris\Client\Admin\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Admin.View.WinForms.dll" .\plugins
copy "..\..\..\..\Ris\Client\Adt\bin\%1\ClearCanvas.Ris.Client.Adt.dll" .\plugins
copy "..\..\..\..\Ris\Client\Adt\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Adt.View.WinForms.dll" .\plugins

::copy "..\..\..\..\Ris\Client\Modality\bin\%1\ClearCanvas.Ris.Client.Modality.dll" .\plugins
::copy "..\..\..\..\Ris\Client\Modality\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Modality.View.WinForms.dll" .\plugins
