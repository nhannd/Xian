#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

//#define WRITE_UPDATEINFO

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Reflection;

namespace ClearCanvas.Distribution.Update.Services
{
	[WebService(Namespace = "http://www.clearcanvas.ca/services/update")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class UpdateInformationService : WebService
	{
		private static readonly XmlSerializer _serializer;

		static UpdateInformationService()
		{
			var types = new List<Type> { typeof(ComponentInfo), typeof(EditionInfo), typeof(UpgradeRule) };
			Assembly assembly = typeof(UpdateInformationService).Assembly;
			foreach (var type in assembly.GetTypes())
			{
				if (typeof(ComponentUpgradeTest).IsAssignableFrom(type))
					types.Add(type);
			}

			_serializer = new XmlSerializer(typeof(LatestComponents), types.ToArray());
		}

		[WebMethod(false)]
		public UpdateInformationResult GetUpdateInformation(UpdateInformationRequest request)
		{
			try
			{
				Logger.Debug("Received request for update information.");

				if (request == null || request.InstalledProduct == null)
				{
					const string message = "Bad request.";
					Logger.Error(message);
					throw new SoapException(message, SoapException.ClientFaultCode);
				}

				string fileName = ConfigurationManager.AppSettings["Update Information Xml FileName"];
				fileName = Server.MapPath(fileName);

				if (Logger.IsDebugEnabled)
					Logger.DebugFormat("Update info xml filename is: {0}", fileName);

				
#if WRITE_UPDATEINFO

				using (FileStream stream = File.Open(fileName, FileMode.Create, FileAccess.Write))
					_serializer.Serialize(stream, LatestComponents.CreateCurrent());

				if (Logger.IsDebugEnabled)
					Logger.DebugFormat("Wrote info xml file: {0}", fileName);
#endif
				Stream fStream = File.OpenRead(fileName);
				using (fStream)
				{
					var latestComponents = (LatestComponents)_serializer.Deserialize(fStream);

					string downloadUrl;
					var latest = latestComponents.GetUpgradeFor(request.InstalledProduct, out downloadUrl);
					var result = new UpdateInformationResult(new Product
					                                   	{
					                                   		Name = latest.Name,
					                                   		Edition = latest.Edition,
					                                   		Release = latest.Release,
					                                   		Version = latest.Version,
					                                   		VersionSuffix = latest.VersionSuffix
					                                   	});

					if (!String.IsNullOrEmpty(downloadUrl))
						result.DownloadUrl = downloadUrl;

					return result;
				}
			}
			catch (SoapException se)
			{
				Logger.Error("A soap exception has occurred.", se);
				throw;
			}
			catch (Exception e)
			{
				Logger.Error("An unexpected exception has occurred.", e);
				throw new SoapException("The service is unable to process the request due to an internal error.",
				                        SoapException.ServerFaultCode);
			}
		}
	}

	[Serializable]
	public class UpdateInformationRequest
	{
		public Product InstalledProduct;
	}

	[Serializable]
	public class UpdateInformationResult
	{
		public string DownloadUrl = DownloadUrls.Default;
		public Product InstalledProduct;

		public UpdateInformationResult()
		{
		}

		internal UpdateInformationResult(Product installedProduct)
		{
			InstalledProduct = installedProduct;
		}
	}
}