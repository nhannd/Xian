:: RIS specific postbuild step

echo Executing RIS post-build step

:: Copy config files
copy "..\..\..\..\Ris\Client\Adt\actionmodels.xml "."


:: Copy Enterprise	
copy "..\..\..\..\Enterprise\bin\%1\ClearCanvas.Enterprise.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\Iesi.Collections.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\Spring.Core.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\Spring.Aop.dll" .\plugins

copy "..\..\..\..\Enterprise\Hibernate\bin\%1\ClearCanvas.Enterprise.Hibernate.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\DdlWriter\bin\%1\ClearCanvas.Enterprise.Hibernate.DdlWriter.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\Castle.DynamicProxy.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\HashCodeProvider.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\NHibernate.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\NHibernate.Caches.SysCache.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\hibernate.cfg.xml" .



:: copy models
copy "..\..\..\..\Enterprise\Authentication\bin\%1\ClearCanvas.Enterprise.Authentication.dll" .\plugins
copy "..\..\..\..\Enterprise\Authentication\Hibernate\bin\%1\ClearCanvas.Enterprise.Authentication.Hibernate.dll" .\plugins

copy "..\..\..\..\Enterprise\Configuration\bin\%1\ClearCanvas.Enterprise.Configuration.dll" .\plugins
copy "..\..\..\..\Enterprise\Configuration\Hibernate\bin\%1\ClearCanvas.Enterprise.Configuration.Hibernate.dll" .\plugins

copy "..\..\..\..\Workflow\bin\%1\ClearCanvas.Workflow.dll" .\plugins
copy "..\..\..\..\Workflow\Hibernate\bin\%1\ClearCanvas.Workflow.Hibernate.dll" .\plugins

copy "..\..\..\..\Healthcare\bin\%1\ClearCanvas.Healthcare.dll" .\plugins
copy "..\..\..\..\Healthcare\Hibernate\bin\%1\ClearCanvas.Healthcare.Hibernate.dll" .\plugins

:: Ris
copy "..\..\..\..\Ris\Services\bin\%1\ClearCanvas.Ris.Services.dll" .\plugins
copy "..\..\..\..\Ris\Client\Admin\bin\%1\ClearCanvas.Ris.Client.Admin.dll" .\plugins
copy "..\..\..\..\Ris\Client\Admin\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Admin.View.WinForms.dll" .\plugins
copy "..\..\..\..\Ris\Client\Adt\bin\%1\ClearCanvas.Ris.Client.Adt.dll" .\plugins
copy "..\..\..\..\Ris\Client\Adt\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Adt.View.WinForms.dll" .\plugins
copy "..\..\..\..\Ris\Client\Common\bin\%1\ClearCanvas.Ris.Client.Common.dll" .\plugins
copy "..\..\..\..\Ris\Client\Common\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Common.View.WinForms.dll" .\plugins
copy "..\..\..\..\Ris\Client\Modality\bin\%1\ClearCanvas.Ris.Client.Modality.dll" .\plugins
copy "..\..\..\..\Ris\Client\Modality\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Modality.View.WinForms.dll" .\plugins
copy "..\..\..\..\Jscript\bin\%1\ClearCanvas.Jscript.dll" .\plugins

:: Copy HL7
copy "..\..\..\..\HL7\bin\%1\ClearCanvas.HL7.dll" .\plugins
copy "..\..\..\..\HL7\Hibernate\bin\%1\ClearCanvas.HL7.Hibernate.dll" .\plugins
copy "..\..\..\..\HL7\InboundProcessor\bin\%1\ClearCanvas.HL7.InboundProcessor.dll" .\plugins
::copy "..\..\..\..\HL7\Xsd\bin\%1\ClearCanvas.HL7.Xsd.dll" .\plugins
::copy "..\..\..\..\HL7\Xsd\v231\bin\%1\ClearCanvas.HL7.Xsd.v231.Generated.dll" .\plugins
::copy "..\..\..\..\HL7\Xsd\v231\bin\%1\ClearCanvas.HL7.Xsd.v231.dll" .\plugins
copy "..\..\..\..\HL7\NHapi\bin\%1\ClearCanvas.HL7.Hapi.dll" .\plugins
copy "..\..\..\..\ReferencedAssemblies\NHapi.*" .\plugins