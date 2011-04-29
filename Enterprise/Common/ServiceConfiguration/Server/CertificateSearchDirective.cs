#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Security.Cryptography.X509Certificates;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	/// <summary>
	/// Specifies how to find the certificate to host the service
	/// </summary>
	public class CertificateSearchDirective
	{
		public CertificateSearchDirective()
		{
			// set defaults consistent with behaviour prior to #8219
			this.StoreLocation = StoreLocation.LocalMachine;
			this.StoreName = StoreName.My;
			this.FindType = X509FindType.FindBySubjectName;
		}

		public StoreLocation StoreLocation { get; set; }
		public StoreName StoreName { get; set; }
		public X509FindType FindType { get; set; }
		public object FindValue { get; set; }
	}
}
