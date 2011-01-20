#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// Note: This utility is meant to execute without use any CC stuff.
// Do not reference any CC stuff here
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Enterprise.VersionExecutable.ServiceReference;

namespace ClearCanvas.Enterprise.VersionExecutable
{
    class VersionCheckerCommandLine
    {
        public string Host;
        public int Port;
        public string ServiceIdentity;

        public VersionCheckerCommandLine(string[] args)
        {
            if (args.Length != 3)
            {
                throw new UsageException("Invalid number of arguments.");
            }

            Host = args[0];

            if (!int.TryParse(args[1], out Port))
            {
                throw new UsageException("Invalid port");
            }

            ServiceIdentity = args[2];
        }
    }

    //Note: Do not reference any CC classes here
    class VersionChecker
    {
        public VersionChecker(string host, int port, string identity)
        {
            Host = host;
            Port = port;
            ServiceIdentity = identity;
        }

        protected string Host { get; private set; }
        protected int Port { get; private set; }
        protected string ServiceIdentity { get; set; }

        public int Run()
        {

            try
            {
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None; // see Enterprise.Common.ServiceConfiguration.Client.NetTcpConfiguration
                
                binding.MaxBufferSize = Int32.MaxValue;
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                
                string uri = string.Format("net.tcp://{0}:{1}/ClearCanvas.Enterprise.Common.ServerVersion.IVersionService", Host, Port);
                EndpointAddress address = new EndpointAddress(new Uri(uri), EndpointIdentity.CreateDnsIdentity(ServiceIdentity));
                
                VersionServiceClient client = new VersionServiceClient(binding, address);
                

                // don't authenticate server's certificate
                client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
                client.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);

                GetVersionRequest request = new GetVersionRequest();
                GetVersionResponse response = client.GetVersion(request);

                String version = String.Format("{0}.{1}.{2}.{3}", response.VersionMajor, response.VersionMinor, response.VersionBuild, response.VersionRevision);

                Console.WriteLine(version);

                return 0; // return 0 per requirement
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :{0}", ex.Message);
                return -1; // return -1 per requirement
            }
        }

       
        public static void PrintSyntax()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Expected Arguments: <hostname or ip>  <port> <serviceIdentity>");
            message.AppendLine("For example: 10.2.19.214  8000  enterprise");
            message.AppendLine("where 'enterprise' is the hostname specified under ApplicationServer.WebServicesSettings of the Enterprise server's configuration");

            Console.WriteLine(message.ToString());
        }
    }
}