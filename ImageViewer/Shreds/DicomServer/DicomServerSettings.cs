#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Configuration;
using System.Xml;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;

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

	internal class DicomServerSettings : ShredConfigSection, IMigrateSettings
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
                    _instance = ShredConfigManager.GetConfigSection(SettingName) as DicomServerSettings;
                    if (_instance == null)
                    {
                        _instance = new DicomServerSettings();
                        ShredConfigManager.UpdateConfigSection(SettingName, _instance);
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

		[ConfigurationProperty("AllowUnknownCaller", DefaultValue = true)]
		public bool AllowUnknownCaller
		{
			get { return (bool)this["AllowUnknownCaller"]; }
			set { this["AllowUnknownCaller"] = value; }
		}

        [ConfigurationProperty("QueryResponsesInUtf8", DefaultValue = false)]
        public bool QueryResponsesInUtf8
        {
            get { return (bool)this["QueryResponsesInUtf8"]; }
            set { this["QueryResponsesInUtf8"] = value; }
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

            //TODO (Marmot): These 2 to the data contract so they could be set from the UI?
			clone.AllowUnknownCaller = _instance.AllowUnknownCaller;
            clone.QueryResponsesInUtf8 = _instance.QueryResponsesInUtf8;

            //TODO (Marmot): Should these go in the database, too?
            clone.StorageTransferSyntaxes = _instance.StorageTransferSyntaxes;
			clone.ImageStorageSopClasses = _instance.ImageStorageSopClasses;
			clone.NonImageStorageSopClasses = _instance.NonImageStorageSopClasses;

            return clone;
        }

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			switch (migrationValues.PropertyName)
			{
				case "AllowUnknownCaller":
                case "QueryResponsesInUtf8":
                    migrationValues.CurrentValue = migrationValues.PreviousValue;
					break;
                default: //Don't migrate the storage sop classes or transfer syntaxes
					break;
    }
		}

		#endregion
	}
}
