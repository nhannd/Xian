path "C:\Program Files\Microsoft SDKs\Windows\v6.0\Bin"

svcutil.exe http://localhost:49153/LocalDataStore?wsdl /out:LocalDataStoreServiceClient.cs /config:LocalDataStoreServiceConfig.config
svcutil.exe http://localhost:49153/LocalDataStoreActivityMonitor?wsdl /out:LocalDataStoreActivityMonitorServiceClient.cs /config:LocalDataStoreActivityMonitorServiceConfig.config
svcutil.exe http://localhost:49153/DicomServer?wsdl /out:DicomServerServiceClient.cs /config:DicomServerServiceConfig.config