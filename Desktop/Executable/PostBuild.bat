:: This batch file is executed as a postbuild step of the ClearCanvas.Desktop.Executable
:: project.  It copies all common ClearCanvas files into the appropriate directories,
:: then runs a solution specific batch file to copy files specific to that solution.
:: 
:: Assumptions:
:: 1) The name of the solution specific batch file is %2.bat, where %2 is the name of the solution
:: 2) %2.bat is located in the same folder as the solution file

echo Executing ClearCanvas post-build step

:: Delete contents of plugin directory
del /Q ".\plugins\*.*"

:: Make required directories
md ".\common"
md ".\plugins"
md ".\logs"

:: Copy shared config files
copy "..\..\Logging.config" "."

:: Copy shared libraries
copy "..\..\..\..\Common\bin\%3\*.*" ".\Common"
copy "..\..\..\..\Controls\WinForms\bin\%3\ClearCanvas.Controls.WinForms.dll" ".\Common"
copy "..\..\..\..\ReferencedAssemblies\nunit.framework.dll" ".\Common" 
copy "..\..\..\..\ReferencedAssemblies\log4net.dll" ".\Common"
copy "..\..\..\..\ReferencedAssemblies\DotNetMagic2005.DLL" ".\Common"
copy "..\..\..\..\ReferencedAssemblies\Iesi.Collections.dll" ".\Common"
copy "..\..\..\..\ReferencedAssemblies\Nhibernate.dll" ".\Common"
copy "..\..\..\..\ReferencedAssemblies\Castle.DynamicProxy.dll" ".\Common"

:: Copy shared plugins
copy "..\..\..\bin\%3\ClearCanvas.Desktop.dll" ".\plugins"
copy "..\..\..\Configuration\bin\%3\ClearCanvas.Desktop.Configuration.dll" .\plugins
copy "..\..\..\Configuration\View\WinForms\bin\%3\ClearCanvas.Desktop.Configuration.View.WinForms.dll" .\plugins
copy "..\..\..\View\WinForms\bin\%3\ClearCanvas.Desktop.View.WinForms.dll" ".\plugins"
copy "..\..\..\Help\bin\%3\ClearCanvas.Desktop.Help.dll" ".\plugins"
copy "..\..\..\ExtensionBrowser\bin\%3\ClearCanvas.Desktop.ExtensionBrowser.dll" ".\plugins"
copy "..\..\..\ExtensionBrowser\View\WinForms\bin\%3\ClearCanvas.Desktop.ExtensionBrowser.View.WinForms.dll" ".\plugins"

:: Copy config files
copy "..\..\..\Actions\actionmodels.xml "."

:: Copy license file
copy "..\..\..\..\License.rtf ."

:: Run the solution specific batch file
call ""%1\%2.bat"" %3
