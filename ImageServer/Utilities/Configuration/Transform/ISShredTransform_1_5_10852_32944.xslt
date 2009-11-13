<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="current"/>
  
	<xsl:template match="/">
		<configuration>
			<configSections>
				<sectionGroup name="applicationSettings">
					<section name="ClearCanvas.Common.ProductSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.Common.ExtensionSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.Enterprise.Common.RemoteCoreServiceSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Common.WebServicesSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Common.RemoteImageServerServiceSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.Dicom.ServiceModel.Streaming.StreamingSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.Dicom.Network.NetworkSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.Dicom.DicomSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Common.Diagnostics.DiagnosticSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Common.Debug" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Common.Settings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Common.PatientNameSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Services.Dicom.DicomSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter.DirectoryImportSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
					         requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming.Settings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Services.Streaming.Shreds.ImageStreamingServerSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
					         requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Services.Archiving.Hsm.HsmSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
					<section name="ClearCanvas.ImageServer.Enterprise.SqlServer2005.SqlServerSettings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
				</sectionGroup>
				<section name="ShredHostServiceSettings" type="ClearCanvas.Server.ShredHost.ShredHostServiceSettings, ClearCanvas.Server.ShredHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" allowLocation="true" allowDefinition="Everywhere"
				         allowExeDefinition="MachineToApplication" restartOnExternalChanges="true" requirePermission="true"/>
				<section name="WorkQueueSettings" type="ClearCanvas.ImageServer.Services.WorkQueue.WorkQueueSettings, ClearCanvas.ImageServer.Services.WorkQueue, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" allowLocation="true"
				         allowDefinition="Everywhere" allowExeDefinition="MachineToApplication" restartOnExternalChanges="true" requirePermission="true"/>
			</configSections>
			<xsl:copy-of select="configuration/connectionStrings"/>
			<applicationSettings>
				<ClearCanvas.Common.ProductSettings>
				  <xsl:copy-of select="document($current)/configuration/applicationSettings/ClearCanvas.Common.ProductSettings/setting"/>
				</ClearCanvas.Common.ProductSettings>
				<ClearCanvas.Common.ExtensionSettings>
					<setting name="ExtensionConfigurationXml" serializeAs="Xml">
						<value>
							<extensions>
								<!-- DO NOT CHANGE THESE SETTINGS -->
								<!-- provide access to core enterprise services locally -->
								<extension class="ClearCanvas.Enterprise.Core.InProcessCoreServiceProvider, ClearCanvas.Enterprise.Core" enabled="false"/>
								<!-- provide access to core enterprise services remotely -->
								<extension class="ClearCanvas.Enterprise.Common.RemoteCoreServiceProvider, ClearCanvas.Enterprise.Common" enabled="false"/>
								<!-- provide access to Image Server services locally -->
								<extension class="ClearCanvas.ImageServer.Common.InProcessImageServerServiceProvider, ClearCanvas.ImageServer.Common" enabled="true"/>
								<!-- provide access to Image Server services remotely -->
								<extension class="ClearCanvas.ImageServer.Common.RemoteImageServerServiceProvider, ClearCanvas.ImageServer.Common" enabled="false"/>
								<extension class="ClearCanvas.ImageServer.Services.Common.Authentication.DefaultAuthenticationService, ClearCanvas.ImageServer.Services.Common" enabled="false"/>
								<!-- Enterprise services hosted on the Enterprise Server -->
								<extension class="ClearCanvas.Enterprise.Common.EnterpriseTimeProvider, ClearCanvas.Enterprise.Common" enabled="false"/>
								<extension class="ClearCanvas.Enterprise.Common.Audit.AuditSink, ClearCanvas.Enterprise.Common" enabled="false"/>
								<extension class="ClearCanvas.ImageServer.Services.Common.Alert.AlertService, ClearCanvas.ImageServer.Services.Common" enabled="true"/>
								<extension class="ClearCanvas.ImageServer.Services.Common.Misc.FilesystemInfoService, ClearCanvas.ImageServer.Services.Common" enabled="true"/>
							</extensions>
						</value>
					</setting>
				</ClearCanvas.Common.ExtensionSettings>
				<ClearCanvas.Enterprise.Common.RemoteCoreServiceSettings>
					<setting name="BaseUrl" serializeAs="String">
						<value>net.tcp://localhost:9999/</value>
					</setting>
					<setting name="ConfigurationClass" serializeAs="String">
						<value>ClearCanvas.Enterprise.Common.ServiceConfiguration.Client.NetTcpConfiguration, ClearCanvas.Enterprise.Common</value>
					</setting>
					<setting name="MaxReceivedMessageSize" serializeAs="String">
						<value>2000000</value>
					</setting>
					<setting name="CertificateValidationMode" serializeAs="String">
						<value>PeerOrChainTrust</value>
					</setting>
					<setting name="RevocationMode" serializeAs="String">
						<value>NoCheck</value>
					</setting>
					<setting name="UserCredentialsProviderClass" serializeAs="String">
						<value/>
					</setting>
					<setting name="FailoverBaseUrl" serializeAs="String">
						<value/>
					</setting>
				</ClearCanvas.Enterprise.Common.RemoteCoreServiceSettings>
				<ClearCanvas.ImageServer.Common.WebServicesSettings>
					<setting name="BaseUri" serializeAs="String">
						<value>http://localhost:9998</value>
					</setting>
					<setting name="SecurityMode" serializeAs="String">
						<value>None</value>
					</setting>
				</ClearCanvas.ImageServer.Common.WebServicesSettings>
				<ClearCanvas.ImageServer.Common.RemoteImageServerServiceSettings>
					<setting name="BaseUrl" serializeAs="String">
						<value>http://localhost:9998/</value>
					</setting>
					<setting name="FailoverBaseUrl" serializeAs="String">
						<value/>
					</setting>
					<setting name="ConfigurationClass" serializeAs="String">
						<value>ClearCanvas.ImageServer.Common.ClientWsHttpConfiguration, ClearCanvas.ImageServer.Common</value>
					</setting>
					<setting name="MaxReceivedMessageSize" serializeAs="String">
						<value>2000000</value>
					</setting>
					<setting name="CertificateValidationMode" serializeAs="String">
						<value>None</value>
					</setting>
					<setting name="RevocationMode" serializeAs="String">
						<value>NoCheck</value>
					</setting>
					<setting name="UserCredentialsProviderClass" serializeAs="String">
						<value/>
					</setting>
				</ClearCanvas.ImageServer.Common.RemoteImageServerServiceSettings>
				<ClearCanvas.Dicom.ServiceModel.Streaming.StreamingSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.Dicom.ServiceModel.Streaming.StreamingSettings/setting"/>
				</ClearCanvas.Dicom.ServiceModel.Streaming.StreamingSettings>
				<ClearCanvas.Dicom.Network.NetworkSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.Dicom.Network.NetworkSettings/setting"/>
				</ClearCanvas.Dicom.Network.NetworkSettings>
				<ClearCanvas.Dicom.DicomSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.Dicom.DicomSettings/setting"/>
					<setting name="IgnoreOutOfRangeTags" serializeAs="String">
						<value>True</value>
					</setting>
				</ClearCanvas.Dicom.DicomSettings>
				<ClearCanvas.ImageServer.Common.Diagnostics.DiagnosticSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Common.Diagnostics.DiagnosticSettings/setting"/>
				</ClearCanvas.ImageServer.Common.Diagnostics.DiagnosticSettings>
				<ClearCanvas.ImageServer.Common.Settings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Common.Settings/setting"/>
					<setting name="WorkQueueMaxFailureCount" serializeAs="String">
						<value>3</value>
					</setting>
					<setting name="InactiveWorkQueueMinTime" serializeAs="String">
						<value>2.00:00:00</value>
					</setting>
				</ClearCanvas.ImageServer.Common.Settings>
				<ClearCanvas.ImageServer.Common.PatientNameSettings>
					<setting name="RemoveRedundantSpaces" serializeAs="String">
						<value>True</value>
					</setting>
				</ClearCanvas.ImageServer.Common.PatientNameSettings>
				<ClearCanvas.ImageServer.Services.Dicom.DicomSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.Dicom.DicomSettings/setting"/>
				</ClearCanvas.ImageServer.Services.Dicom.DicomSettings>
				<ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter.DirectoryImportSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter.DirectoryImportSettings/setting"/>
				</ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter.DirectoryImportSettings>
				<ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='FilesystemQueueResultCount']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='FilesystemDeleteRecheckDelay']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='FilesystemDeleteCheckInterval']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='TierMigrationSpeed']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='HighWatermarkAlertInterval']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='FilesystemLossyCompressRecheckDelay']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='FilesystemLosslessCompressRecheckDelay']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='ApplicationLogCachedDays']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='ApplicationLogRecheckDelay']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='AlertCachedDays']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='AlertRecheckDelay']"/>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings/setting[@name='AlertDelete']"/>
				</ClearCanvas.ImageServer.Services.ServiceLock.ServiceLockSettings>
				<ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming.Settings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming.Settings/setting"/>
				</ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming.Settings>
				<ClearCanvas.ImageServer.Services.Streaming.Shreds.ImageStreamingServerSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.Streaming.Shreds.ImageStreamingServerSettings/setting"/>
				</ClearCanvas.ImageServer.Services.Streaming.Shreds.ImageStreamingServerSettings>
				<ClearCanvas.ImageServer.Services.Archiving.Hsm.HsmSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Services.Archiving.Hsm.HsmSettings/setting"/>
				</ClearCanvas.ImageServer.Services.Archiving.Hsm.HsmSettings>
				<ClearCanvas.ImageServer.Enterprise.SqlServer2005.SqlServerSettings>
					<xsl:copy-of select="configuration/applicationSettings/ClearCanvas.ImageServer.Enterprise.SqlServer2005.SqlServerSettings/setting"/>
				</ClearCanvas.ImageServer.Enterprise.SqlServer2005.SqlServerSettings>
			</applicationSettings>
			<system.serviceModel>
				<services>
					<service name="ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming.HeaderStreamingService" behaviorConfiguration="HeaderStreamServiceBehavior"/>
				</services>
				<behaviors>
					<serviceBehaviors>
						<behavior name="HeaderStreamServiceBehavior">
							<serviceThrottling maxConcurrentCalls="32" maxConcurrentSessions="32" maxConcurrentInstances="32"/>
						</behavior>
					</serviceBehaviors>
				</behaviors>
				<bindings>
					<basicHttpBinding>
						<binding name="BasicHttpBinding_IHeaderStreamingService" closeTimeout="00:01:00" openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00" bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard"
						         maxBufferPoolSize="524288" maxReceivedMessageSize="65536" transferMode="Streamed" messageEncoding="Mtom" textEncoding="utf-8" useDefaultWebProxy="true" allowCookies="false">
							<readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384" maxBytesPerRead="4096" maxNameTableCharCount="16384"/>
						</binding>
					</basicHttpBinding>
				</bindings>
				<client/>
			</system.serviceModel>
			<ShredHostServiceSettings ShredHostHttpPort="50220" SharedHttpPort="50221" SharedTcpPort="50222"/>
			<runtime>
				<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
					<probing privatePath="common;plugins"/>
				</assemblyBinding>
			</runtime>
			<WorkQueueSettings>
				<xsl:attribute name="WorkQueueQueryDelay">
					<xsl:value-of select="configuration/WorkQueueSettings/@WorkQueueQueryDelay"/>
				</xsl:attribute>
				<xsl:attribute name="WorkQueueThreadCount">
					<xsl:value-of select="configuration/WorkQueueSettings/@WorkQueueThreadCount"/>
				</xsl:attribute>
				<xsl:attribute name="PriorityWorkQueueThreadCount">
					<xsl:value-of select="configuration/WorkQueueSettings/@PriorityWorkQueueThreadCount"/>
				</xsl:attribute>
				<xsl:attribute name="MemoryLimitedWorkQueueThreadCount">
					<xsl:value-of select="configuration/WorkQueueSettings/@MemoryLimitedWorkQueueThreadCount"/>
				</xsl:attribute>
				<xsl:attribute name="WorkQueueMinimumFreeMemoryMB">
					<xsl:value-of select="configuration/WorkQueueSettings/@WorkQueueMinimumFreeMemoryMB"/>
				</xsl:attribute>
				<xsl:attribute name="EnableStudyIntegrityValidation">
					<xsl:value-of select="configuration/WorkQueueSettings/@EnableStudyIntegrityValidation"/>
				</xsl:attribute>
				<xsl:attribute name="TierMigrationProgressUpdateInSeconds">
					<xsl:value-of select="configuration/WorkQueueSettings/@TierMigrationProgressUpdateInSeconds"/>
				</xsl:attribute>
			</WorkQueueSettings>
		</configuration>
	</xsl:template>
</xsl:stylesheet>

