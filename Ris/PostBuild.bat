:: RIS specific postbuild step

echo Executing RIS post-build step

:: Copy Enterprise	
copy "..\..\..\..\Enterprise\bin\%1\ClearCanvas.Enterprise.dll" .\plugins
copy "..\..\..\..\Enterprise\refs\Iesi.Collections.dll" .\plugins
copy "..\..\..\..\Enterprise\refs\Spring.Core.dll" .\plugins
copy "..\..\..\..\Enterprise\refs\Spring.Aop.dll" .\plugins

copy "..\..\..\..\Enterprise\Hibernate\bin\%1\ClearCanvas.Enterprise.Hibernate.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\DdlWriter\bin\%1\ClearCanvas.Enterprise.Hibernate.DdlWriter.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\refs\Castle.DynamicProxy.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\refs\HashCodeProvider.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\refs\NHibernate.dll" .\plugins
copy "..\..\..\..\Enterprise\Hibernate\hibernate.cfg.xml" .


:: copy Healthcare
copy "..\..\..\..\Healthcare\bin\%1\ClearCanvas.Healthcare.dll" .\plugins
copy "..\..\..\..\Healthcare\Hibernate\bin\%1\ClearCanvas.Healthcare.Hibernate.dll" .\plugins

:: Ris
copy "..\..\..\..\Ris\Services\bin\%1\ClearCanvas.Ris.Services.dll" .\plugins
copy "..\..\..\..\Ris\Client\Admin\bin\%1\ClearCanvas.Ris.Client.Admin.dll" .\plugins
copy "..\..\..\..\Ris\Client\Admin\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Admin.View.WinForms.dll" .\plugins
copy "..\..\..\..\Ris\Client\Adt\bin\%1\ClearCanvas.Ris.Client.Adt.dll" .\plugins
copy "..\..\..\..\Ris\Client\Adt\View\WinForms\bin\%1\ClearCanvas.Ris.Client.Adt.View.WinForms.dll" .\plugins
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