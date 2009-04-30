#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Server.ShredHost;
using System.IO;
using ClearCanvas.Common.Utilities;
using System.Xml;
using System.Collections;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	#region Custom Configuration classes

	public class SopClassConfigurationElement : ConfigurationElement
	{
		public SopClassConfigurationElement()
		{
		}

		public SopClassConfigurationElement(string uid, string description)
		{
			Uid = uid;
			Description = description;
		}

		[ConfigurationProperty("Uid", IsKey = true, IsRequired = true)]
		public string Uid
		{
			get { return this["Uid"] as string; }
			set { this["Uid"] = value; }
		}

		[ConfigurationProperty("Description", IsKey = false, IsRequired = false)]
		public string Description
		{
			get { return this["Description"] as string; }
			set { this["Description"] = value; }
		}
	}

	public class ImageSopClassConfigurationElementCollection : SopClassConfigurationElementCollection
	{
		public ImageSopClassConfigurationElementCollection()
		{
		}

		protected override void InitializeDefault()
		{
			base.Initialize("ImageSopClasses.xml");
		}

		internal static ImageSopClassConfigurationElementCollection Create()
		{
			ImageSopClassConfigurationElementCollection config = new ImageSopClassConfigurationElementCollection();
			config.Initialize("ImageSopClasses.xml");
			return config;
		}
	}

	public class NonImageSopClassConfigurationElementCollection : SopClassConfigurationElementCollection
	{
		public NonImageSopClassConfigurationElementCollection()
		{
		}

		protected override void InitializeDefault()
		{
			base.Initialize("NonImageSopClasses.xml");
		}

		internal static NonImageSopClassConfigurationElementCollection Create()
		{
			NonImageSopClassConfigurationElementCollection config = new NonImageSopClassConfigurationElementCollection();
			config.Initialize("NonImageSopClasses.xml");
			return config;
		}
	}

	public abstract class SopClassConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new SopClassConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SopClassConfigurationElement)element).Uid;
		}

		protected void Initialize(string xmlResourceName)
		{
			using (Stream stream = new ResourceResolver(typeof(SopClassConfigurationElementCollection).Assembly).OpenResource(xmlResourceName))
			{
				XmlDocument document = new XmlDocument();
				document.Load(stream);
				foreach (XmlElement sopClass in document.SelectNodes("//sop-class"))
					Add(sopClass.Attributes["uid"].InnerText, sopClass.Attributes["description"].InnerText);

				stream.Close();
			}
		}

		public void Add(string uid, string description)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);

			base.BaseAdd(new SopClassConfigurationElement(uid, description), false);
		}

		public void Remove(string uid)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);
		}
	}

	public class TransferSyntaxConfigurationElement : ConfigurationElement
	{
		public TransferSyntaxConfigurationElement()
		{
		}

		public TransferSyntaxConfigurationElement(string uid, string description)
		{
			Uid = uid;
			Description = description;
		}

		[ConfigurationProperty("Uid", IsKey = true, IsRequired = true)]
		public string Uid
		{
			get { return this["Uid"] as string; }
			set { this["Uid"] = value; }
		}

		[ConfigurationProperty("Description", IsKey = false, IsRequired = false)]
		public string Description
		{
			get { return this["Description"] as string; }
			set { this["Description"] = value; }
		}
	}

	public class TransferSyntaxConfigurationElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TransferSyntaxConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TransferSyntaxConfigurationElement)element).Uid;
		}

		protected override void InitializeDefault()
		{
			using (Stream stream = new ResourceResolver(typeof(SopClassConfigurationElementCollection).Assembly).OpenResource("TransferSyntaxes.xml"))
			{
				XmlDocument document = new XmlDocument();
				document.Load(stream);
				foreach (XmlElement transferSyntax in document.SelectNodes("//transfer-syntax"))
					Add(transferSyntax.Attributes["uid"].InnerText, transferSyntax.Attributes["description"].InnerText);
			}
		}

		public void Add(string uid, string description)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);

			base.BaseAdd(new TransferSyntaxConfigurationElement(uid, description), false);
		}

		public void Remove(string uid)
		{
			if (base.BaseGet(uid) != null)
				base.BaseRemove(uid);
		}

		public static TransferSyntaxConfigurationElementCollection Create()
		{
			TransferSyntaxConfigurationElementCollection config = new TransferSyntaxConfigurationElementCollection();
			config.InitializeDefault();
			return config;
		}
	}

	#endregion

	internal class DicomServerSettings : ShredConfigSection
    {
        private static DicomServerSettings _instance;

		private DicomServerSettings()
        {
        }

        public static string SettingName
        {
            get { return "DicomServerSettings"; }
        }

        public static DicomServerSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ShredConfigManager.GetConfigSection(DicomServerSettings.SettingName) as DicomServerSettings;
                    if (_instance == null)
                    {
                        _instance = new DicomServerSettings();
                        ShredConfigManager.UpdateConfigSection(DicomServerSettings.SettingName, _instance);
                    }
                }

                return _instance;
            }
        }

        public static void Save()
        {
            ShredConfigManager.UpdateConfigSection(DicomServerSettings.SettingName, _instance);
        }

        #region Public Properties

        [ConfigurationProperty("HostName", DefaultValue = "localhost")]
        public string HostName
        {
            get { return (string)this["HostName"]; }
            set { this["HostName"] = value; }
        }

        [ConfigurationProperty("AETitle", DefaultValue = "CLEARCANVAS")]
        public string AETitle
        {
            get { return (string)this["AETitle"]; }
            set { this["AETitle"] = value; }
        }

        [ConfigurationProperty("Port", DefaultValue = "104")]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("InterimStorageDirectory", DefaultValue = ".\\dicom_interim")]
        public string InterimStorageDirectory
        {
            get { return (string)this["InterimStorageDirectory"]; }
            set { this["InterimStorageDirectory"] = value; }
        }

		[ConfigurationProperty("ImageStorageSopClasses")]
		public ImageSopClassConfigurationElementCollection ImageStorageSopClasses
		{
			get { return (ImageSopClassConfigurationElementCollection)this["ImageStorageSopClasses"]; }
			set { this["ImageStorageSopClasses"] = value; }
		}

		[ConfigurationProperty("NonImageStorageSopClasses")]
		public NonImageSopClassConfigurationElementCollection NonImageStorageSopClasses
		{
			get { return (NonImageSopClassConfigurationElementCollection)this["NonImageStorageSopClasses"]; }
			set { this["NonImageStorageSopClasses"] = value; }
		}

		[ConfigurationProperty("StorageTransferSyntaxes")]
		public TransferSyntaxConfigurationElementCollection StorageTransferSyntaxes
		{
			get { return (TransferSyntaxConfigurationElementCollection)this["StorageTransferSyntaxes"]; }
			set { this["StorageTransferSyntaxes"] = value; }
		}

		#endregion

        public override object Clone()
        {
            DicomServerSettings clone = new DicomServerSettings();

            clone.HostName = _instance.HostName;
            clone.AETitle = _instance.AETitle;
            clone.Port = _instance.Port;
            
			clone.InterimStorageDirectory = _instance.InterimStorageDirectory;
			clone.StorageTransferSyntaxes = _instance.StorageTransferSyntaxes;
			clone.ImageStorageSopClasses = _instance.ImageStorageSopClasses;
			clone.NonImageStorageSopClasses = _instance.NonImageStorageSopClasses;

            return clone;
        }
    }
}
