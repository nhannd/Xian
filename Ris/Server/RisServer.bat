:: RIS-Server specific postbuild step

echo Executing RIS post-build step

:: Copy config files
copy "..\..\..\..\Ris\Client\Adt\actionmodels.xml "."

:: Copy setup files
copy "..\..\..\..\..\Ris\Server\setup\ddl-writer.bat" .
copy "..\..\..\..\..\Ris\Server\setup\import-authority-tokens.bat" .
copy "..\..\..\..\..\Ris\Server\setup\import-authority-groups.bat" .

copy "..\..\..\..\..\Jscript\bin\%1\ClearCanvas.Jscript.dll" .\plugins

:: Copy Enterprise	
copy "..\..\..\..\..\Enterprise\Common\bin\%1\ClearCanvas.Enterprise.Common.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Core\bin\%1\ClearCanvas.Enterprise.Core.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Hibernate\bin\%1\ClearCanvas.Enterprise.Hibernate.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\Iesi.Collections.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\Spring.Core.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\Spring.Aop.dll" .\plugins

copy "..\..\..\..\..\Enterprise\Hibernate\bin\%1\ClearCanvas.Enterprise.Hibernate.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Hibernate\DdlWriter\bin\%1\ClearCanvas.Enterprise.Hibernate.DdlWriter.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\Castle.DynamicProxy.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\HashCodeProvider.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\NHibernate.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\NHibernate.Caches.SysCache.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Hibernate\hibernate.cfg.xml" .

copy "..\..\..\..\..\Enterprise\Configuration\bin\%1\ClearCanvas.Enterprise.Configuration.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Configuration\Hibernate\bin\%1\ClearCanvas.Enterprise.Configuration.Hibernate.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Authentication\bin\%1\ClearCanvas.Enterprise.Authentication.dll" .\plugins
copy "..\..\..\..\..\Enterprise\Authentication\Hibernate\bin\%1\ClearCanvas.Enterprise.Authentication.Hibernate.dll" .\plugins




:: copy models
copy "..\..\..\..\..\Workflow\bin\%1\ClearCanvas.Workflow.dll" .\plugins
copy "..\..\..\..\..\Workflow\Hibernate\bin\%1\ClearCanvas.Workflow.Hibernate.dll" .\plugins

copy "..\..\..\..\..\Healthcare\bin\%1\ClearCanvas.Healthcare.dll" .\plugins
copy "..\..\..\..\..\Healthcare\Hibernate\bin\%1\ClearCanvas.Healthcare.Hibernate.dll" .\plugins

copy "..\..\..\..\..\HL7\bin\%1\ClearCanvas.HL7.dll" .\plugins
copy "..\..\..\..\..\HL7\Hibernate\bin\%1\ClearCanvas.HL7.Hibernate.dll" .\plugins
copy "..\..\..\..\..\HL7\NHapi\bin\%1\ClearCanvas.HL7.Hapi.dll" .\plugins
copy "..\..\..\..\..\ReferencedAssemblies\NHapi.*" .\plugins

:: Ris
copy "..\..\..\..\..\Ris\Application\Common\bin\%1\ClearCanvas.Ris.Application.Common.dll" .\plugins
copy "..\..\..\..\..\Ris\Application\Services\bin\%1\ClearCanvas.Ris.Application.Services.dll" .\plugins
copy "..\..\..\..\..\Ris\Server\bin\%1\ClearCanvas.Ris.Server.dll" .\plugins
