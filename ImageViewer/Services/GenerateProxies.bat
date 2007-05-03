path "C:\Program Files\Microsoft SDKs\Windows\v6.0\Bin"

svcutil.exe http://localhost:50121/LocalDataStore?wsdl /out:LocalDataStoreServiceClient.cs /config:LocalDataStoreServiceConfig.config
svcutil.exe http://localhost:50121/LocalDataStoreActivityMonitor?wsdl /out:LocalDataStoreActivityMonitorServiceClient.cs /config:LocalDataStoreActivityMonitorServiceConfig.config
svcutil.exe http://localhost:50121/DicomServer?wsdl /out:DicomServerServiceClient.cs /config:DicomServerServiceConfig.config
svcutil.exe http://localhost:50121/DiskspaceManager?wsdl /out:DiskspaceManagerServiceClient.cs /config:DiskspaceManagerServiceConfig.config