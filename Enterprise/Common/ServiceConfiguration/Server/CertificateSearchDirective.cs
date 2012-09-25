#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Cryptography.X509Certificates;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	/// <summary>
	/// Specifies how to find the certificate to host the service
	/// </summary>
	public class CertificateSearchDirective
	{
		public static CertificateSearchDirective CreateDefault(X509FindType findType, Uri hostUri)
		{
			return new CertificateSearchDirective
					{
						FindType = findType,
						FindValue = GetDefaultCertificateFindValue(findType, hostUri)
					};
		}

		public static CertificateSearchDirective CreateCustom(X509FindType findType, object customFindValue)
		{
			return new CertificateSearchDirective
			{
				FindType = findType,
				FindValue = customFindValue
			};
		}


		private X509FindType _findType;

		private CertificateSearchDirective()
		{
			// set defaults consistent with behaviour prior to #8219
			this.StoreLocation = StoreLocation.LocalMachine;
			this.StoreName = StoreName.My;
			this.FindType = X509FindType.FindBySubjectName;
		}

		public StoreLocation StoreLocation { get; private set; }

		public StoreName StoreName { get; private set; }

		public X509FindType FindType
		{
			get { return _findType; }
			set
			{
				if (value == X509FindType.FindByTimeExpired ||
					value == X509FindType.FindByTimeNotYetValid ||
					value == X509FindType.FindByTimeValid)
				{
					// for these, FindValue must be a DateTime value
					throw new NotSupportedException(string.Format("{0} is not supported", value));
				}
				_findType = value;
			}
		}

		public object FindValue { get; set; }

		private static string GetDefaultCertificateFindValue(X509FindType findType, Uri hostUri)
		{
			switch (findType)
			{
				case X509FindType.FindBySubjectName:
					return hostUri.Host;

				case X509FindType.FindBySubjectDistinguishedName:
					// Ideally we must include all parts in a DN (Common name, Organization, Organizational Unit, City, State, Country, Domain Component)
					// in a REAL certificate. Howerver, for simplicity, we allow people to use self-signed certificate 
					// and assume only the common name is specified in the certificate.
					return string.Format("CN={0}", hostUri.Host);

				case X509FindType.FindByTimeValid:
				case X509FindType.FindByTimeNotYetValid:
				case X509FindType.FindByTimeExpired:
					// for these, FindValue must be a DateTime value
					throw new NotSupportedException(string.Format("{0} is not supported", findType));

				default:
					return hostUri.Host;
			}
		}
	}
}
