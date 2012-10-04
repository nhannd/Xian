#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace ClickOncePublisher
{
	public class PackageEnumerator
	{
		public static List<string> GetPackages(string packagesDirectory)
		{
			List<string> productNames = new List<string>();

			if (!Directory.Exists(packagesDirectory))
				return productNames;

			string[] directories = Directory.GetDirectories(packagesDirectory);

			foreach (string directory in directories)
			{
				string productXmlFilename = Path.Combine(directory, "product.xml");

				if (!File.Exists(productXmlFilename))
					continue;

				XmlDocument doc = new XmlDocument();
				doc.Load(productXmlFilename);

				XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
				manager.AddNamespace("bootstrapper", "http://schemas.microsoft.com/developer/2004/01/bootstrapper");

				XmlElement node = doc.SelectSingleNode("/bootstrapper:Product", manager) as XmlElement;

				productNames.Add(node.Attributes["ProductCode"].Value);
			}

			return productNames;
		}
	}
}
