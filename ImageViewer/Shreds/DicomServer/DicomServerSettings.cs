#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Xml.Serialization;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[SettingsGroupDescription("Configuration for the DICOM server shred.")]
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	internal sealed partial class DicomServerSettings : IMigrateSettings
	{
		private static Proxy _instance;

		public static Proxy Instance
		{
			get { return _instance ?? (_instance = new Proxy(Default)); }
		}

		public sealed class Proxy
		{
			private readonly DicomServerSettings _settings;

			public Proxy(DicomServerSettings settings)
			{
				_settings = settings;
			}

			private object this[string propertyName]
			{
				get { return _settings[propertyName]; }
				set { ApplicationSettingsExtensions.SetSharedPropertyValue(_settings, propertyName, value); }
			}

			[DefaultValue("localhost")]
			public string HostName
			{
				get { return (string) this["HostName"]; }
				set { this["HostName"] = value; }
			}

			[DefaultValue("CLEARCANVAS")]
			public string AETitle
			{
				get { return (string) this["AETitle"]; }
				set { this["AETitle"] = value; }
			}

			[DefaultValue("104")]
			public int Port
			{
				get { return (int) this["Port"]; }
				set { this["Port"] = value; }
			}

			[DefaultValue(".\\dicom_interim")]
			public string InterimStorageDirectory
			{
				get { return (string) this["InterimStorageDirectory"]; }
				set { this["InterimStorageDirectory"] = value; }
			}

			[DefaultValue(true)]
			public bool AllowUnknownCaller
			{
				get { return (bool) this["AllowUnknownCaller"]; }
				set { this["AllowUnknownCaller"] = value; }
			}

			public ImageSopClassConfigurationElementCollection ImageStorageSopClasses
			{
				get { return (ImageSopClassConfigurationElementCollection) this["ImageStorageSopClasses"]; }
				set { this["ImageStorageSopClasses"] = value; }
			}

			public NonImageSopClassConfigurationElementCollection NonImageStorageSopClasses
			{
				get { return (NonImageSopClassConfigurationElementCollection) this["NonImageStorageSopClasses"]; }
				set { this["NonImageStorageSopClasses"] = value; }
			}

			public TransferSyntaxConfigurationElementCollection StorageTransferSyntaxes
			{
				get { return (TransferSyntaxConfigurationElementCollection) this["StorageTransferSyntaxes"]; }
				set { this["StorageTransferSyntaxes"] = value; }
			}

			[DefaultValue(false)]
			public bool QueryResponsesInUtf8
			{
				get { return (bool) this["QueryResponsesInUtf8"]; }
				set { this["QueryResponsesInUtf8"] = value; }
			}

			public void Save()
			{
				_settings.Save();
			}
		}

		#region IMigrateSettings Members

		public void MigrateSettingsProperty(SettingsPropertyMigrationValues migrationValues)
		{
			switch (migrationValues.PropertyName)
			{
				case "HostName":
				case "AETitle":
				case "Port":
				case "InterimStorageDirectory":
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

	#region Custom Configuration classes

	[XmlType("SopClass")]
	public class SopClassConfigurationElement : IEquatable<SopClassConfigurationElement>
	{
		public SopClassConfigurationElement() {}

		public SopClassConfigurationElement(string uid, string description)
		{
			Uid = uid;
			Description = description;
		}

		[XmlAttribute("Uid")]
		public string Uid { get; set; }

		[XmlAttribute("Description")]
		public string Description { get; set; }

		public bool Equals(SopClassConfigurationElement other)
		{
			return other != null && Uid == other.Uid;
		}

		public override bool Equals(object obj)
		{
			return obj is SopClassConfigurationElement && Equals((SopClassConfigurationElement) obj);
		}

		public override int GetHashCode()
		{
			return 0x444CB7C9 ^ (Uid != null ? Uid.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format(@"{0}={1}", Uid, Description);
		}
	}

	[XmlType("ImageSopClassCollection")]
	public class ImageSopClassConfigurationElementCollection : SopClassConfigurationElementCollection {}

	[XmlType("NonImageSopClassCollection")]
	public class NonImageSopClassConfigurationElementCollection : SopClassConfigurationElementCollection {}

	[XmlType("SopClassCollection")]
	public abstract class SopClassConfigurationElementCollection : ConfigurationElementCollection<SopClassConfigurationElement>
	{
		[XmlArray(@"SopClasses")]
		public SopClassConfigurationElement[] SopClasses
		{
			get { return Items; }
			set { Items = value; }
		}
	}

	[XmlType("TransferSyntax")]
	public class TransferSyntaxConfigurationElement : IEquatable<TransferSyntaxConfigurationElement>
	{
		public TransferSyntaxConfigurationElement() {}

		public TransferSyntaxConfigurationElement(string uid, string description)
		{
			Uid = uid;
			Description = description;
		}

		[XmlAttribute("Uid")]
		public string Uid { get; set; }

		[XmlAttribute("Description")]
		public string Description { get; set; }

		public bool Equals(TransferSyntaxConfigurationElement other)
		{
			return other != null && Uid == other.Uid;
		}

		public override bool Equals(object obj)
		{
			return obj is TransferSyntaxConfigurationElement && Equals((TransferSyntaxConfigurationElement) obj);
		}

		public override int GetHashCode()
		{
			return 0x0898858D ^ (Uid != null ? Uid.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format(@"{0}={1}", Uid, Description);
		}
	}

	[XmlType("TransferSyntaxCollection")]
	public class TransferSyntaxConfigurationElementCollection : ConfigurationElementCollection<TransferSyntaxConfigurationElement>
	{
		[XmlArray(@"TransferSyntaxes")]
		public TransferSyntaxConfigurationElement[] TransferSyntaxes
		{
			get { return Items; }
			set { Items = value; }
		}
	}

	public abstract class ConfigurationElementCollection<T> : IList<T>
	{
		private readonly List<T> _items = new List<T>();

		protected T[] Items
		{
			get { return _items.ToArray(); }
			set
			{
				_items.Clear();
				if (value != null) _items.AddRange(value);
			}
		}

		public override string ToString()
		{
			return '{' + string.Join(@", ", _items.Select(i => i.ToString()).ToArray()) + '}';
		}

		#region Implementation of IList<T>

		public T this[int index]
		{
			get { return _items[index]; }
			set { _items[index] = value; }
		}

		public int Count
		{
			get { return _items.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}

		public void Add(T item)
		{
			_items.Add(item);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		public int IndexOf(T item)
		{
			return _items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_items.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return _items.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

	#endregion
}