:: RIS-client specific postbuild step

echo Executing RIS-client post-build step

:: Copy config files
copy "..\..\..\..\Ris\Integration\actionmodels.xml "."


:: Copy Shared libraries	
copy "..\..\..\..\Enterprise\Common\bin\%1\ClearCanvas.Enterprise.Common.dll" .\plugins
copy "..\..\..\..\Ris\Application\Common\bin\%1\ClearCanvas.Ris.Application.Common.dll" .\plugins

:: Client modules
copy "..\..\..\..\Jscript\bin\%1\ClearCanvas.Jscript.dll" .\plugins
copy "..\..\..\..\Ris\Client\bin\%1\ClearCanvas.Ris.Client.dll" .\plugins
copy "..\..\..\..\Ris\Client\View\WinForms\bin\%1\ClearCanvas.Ris.Client.View.WinForms.dll" .\plugins

copy "..\..\..\..\Ris\Client\Reporting\bin\%1\ClearCanvas.Ris.Client.Reporting.dll" .\plugins
copy "..\..\..\..\Ris\Client\Reporting\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Reporting.View.WinForms.dll" .\plugins
