:: This batch file is executed as a postbuild step of the ClearCanvas.Desktop.Executable
:: project.  It copies all common ClearCanvas files into the appropriate directories,
:: then runs a solution specific batch file to copy files specific to that solution.
:: 
:: Assumptions:
:: 1) The name of the solution specific batch file is PostBuild.bat
:: 2) PostBuild.bat is located in the same folder as the solution file

echo Executing ClearCanvas post-build step

:: Delete contents of plugin directory
del /Q ".\plugins\*.*"

:: Make required directories
md ".\common"
md ".\plugins"
md ".\logs"
md ".\datastore"

:: Copy shared config files
copy "..\..\Logging.config" "."

:: Copy shared libraries
copy "..\..\..\..\..\Common\bin\%3\*.*" ".\Common"
copy "..\..\..\..\..\ReferencedAssemblies\nunit.framework.dll" ".\Common" 
copy "..\..\..\..\..\ReferencedAssemblies\log4net.dll" ".\Common"
copy "..\..\..\..\..\ReferencedAssemblies\Iesi.Collections.dll" ".\Common"
copy "..\..\..\..\..\ReferencedAssemblies\Nhibernate.dll" ".\Common"
copy "..\..\..\..\..\ReferencedAssemblies\Castle.DynamicProxy.dll" ".\Common"

:: Run the solution specific batch file
call ""%1\%2.bat"" %3