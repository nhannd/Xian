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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	public class StoredDisplaySetCreationOptions : INotifyPropertyChanged
	{
		private readonly string _modality;
		
		private bool _createSingleImageDisplaySets;
		private bool _splitMultiEchoSeries;
		private bool _showOriginalMultiEchoSeries;

		private bool _splitMixedMultiframes;
		private bool _showOriginalMixedMultiframeSeries;

		private event PropertyChangedEventHandler _propertyChanged;

		internal StoredDisplaySetCreationOptions(string modality)
		{
			_modality = modality;
			SetDefaults();
		}

		private void SetDefaults()
		{
			if (CreateSingleImageDisplaySetsEnabled)
				CreateSingleImageDisplaySets = true;

			if (SplitMixedMultiframesEnabled)
			{
				SplitMixedMultiframes = true;
				ShowOriginalMixedMultiframeSeries = false;
			}

			if (SplitMultiEchoSeriesEnabled)
			{
				SplitMultiEchoSeries = true;
				ShowOriginalMultiEchoSeries = false;
			}
		}

		public string Modality
		{
			get { return _modality; }
		}

		public bool CreateSingleImageDisplaySets
		{
			get { return _createSingleImageDisplaySets; }
			set
			{
				if (_createSingleImageDisplaySets != value)
				{
					_createSingleImageDisplaySets = value;
					NotifyPropertyChanged("CreateSingleImageDisplaySets");
				}
			}
		}

		public bool CreateSingleImageDisplaySetsEnabled
		{
			get { return DisplaySetCreationSettings.Default.GetSingleImageModalities().Contains(_modality); }	
		}

		public bool SplitMultiEchoSeries
		{
			get { return _splitMultiEchoSeries; }
			set
			{
				if (_splitMultiEchoSeries != value)
				{
					_splitMultiEchoSeries = value;
					NotifyPropertyChanged("SplitMultiEchoSeries");
					NotifyPropertyChanged("ShowOriginalMultiEchoSeriesEnabled");
				}
			}
		}

		public bool SplitMultiEchoSeriesEnabled
		{
			get { return _modality == "MR"; }
		}

		public bool ShowOriginalMultiEchoSeries
		{
			get { return _showOriginalMultiEchoSeries; }
			set
			{
				if (_showOriginalMultiEchoSeries != value)
				{
					_showOriginalMultiEchoSeries = value;
					NotifyPropertyChanged("ShowOriginalMultiEchoSeries");
				}
			}
		}

		public bool ShowOriginalMultiEchoSeriesEnabled
		{
			get { return SplitMultiEchoSeries && SplitMultiEchoSeriesEnabled; }	
		}

		public bool SplitMixedMultiframes
		{
			get { return _splitMixedMultiframes; }
			set
			{
				if (_splitMixedMultiframes != value)
				{
					_splitMixedMultiframes = value;
					NotifyPropertyChanged("SplitMixedMultiframes");
					NotifyPropertyChanged("ShowOriginalMixedMultiframeSeriesEnabled");
				}
			}
		}

		public bool SplitMixedMultiframesEnabled
		{
			get { return DisplaySetCreationSettings.Default.GetMixedMultiframeModalities().Contains(_modality); }
		}

		public bool ShowOriginalMixedMultiframeSeries
		{
			get { return _showOriginalMixedMultiframeSeries; }
			set
			{
				if (_showOriginalMixedMultiframeSeries != value)
				{
					_showOriginalMixedMultiframeSeries = value;
					NotifyPropertyChanged("ShowOriginalMixedMultiframeSeries");
				}
			}
		}

		public bool ShowOriginalMixedMultiframeSeriesEnabled
		{
			get { return SplitMixedMultiframes && SplitMixedMultiframesEnabled; }
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		#endregion
	}

	[SettingsGroupDescription("Stores user options for how display sets are created.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DisplaySetCreationSettings
	{
		private DisplaySetCreationSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public List<string> GetSingleImageModalities()
		{
			return CollectionUtils.Map(SingleImageModalities.Split(','),
			   delegate(string s) { return s.Trim(); });
		}

		public List<string> GetMixedMultiframeModalities()
		{
			return CollectionUtils.Map(MixedMultiframeModalities.Split(','),
			   delegate(string s) { return s.Trim(); });
		}

		public List<StoredDisplaySetCreationOptions> GetStoredOptions()
		{
			XmlDocument document = this.DisplaySetCreationSettingsXml;
			if (document == null)
			{
				document = new XmlDocument();
				Stream stream = new ResourceResolver(this.GetType(), false).OpenResource("DisplaySetCreationSettingsDefaults.xml");
				document.Load(stream);
				stream.Close();
			}

			XmlNodeList optionsNodes = document.SelectNodes("//display-set-creation-options");
			if (optionsNodes == null || optionsNodes.Count == 0)
			{
				document = new XmlDocument();
				Stream stream = new ResourceResolver(this.GetType(), false).OpenResource("DisplaySetCreationSettingsDefaults.xml");
				document.Load(stream);
				stream.Close();
				optionsNodes = document.SelectNodes("//display-set-creation-options");
			}

			List<string> missingModalities = new List<string>(StandardModalities.Modalities);
			List<StoredDisplaySetCreationOptions> storedDisplaySetOptions = new List<StoredDisplaySetCreationOptions>();

			foreach (XmlElement optionsNode in optionsNodes)
			{
				XmlAttribute attribute = optionsNode.Attributes["modality"];
				string modality = "";
				if (attribute != null)
				{
					modality = attribute.Value ?? "";
					missingModalities.Remove(modality);
				}

				if (!String.IsNullOrEmpty(modality))
				{
					XmlNodeList optionNodes = optionsNode.SelectNodes("display-set-creation-option");
					storedDisplaySetOptions.Add(GetStoredOptions(modality, optionNodes));
				}
			}

			foreach (string missingModality in missingModalities)
				storedDisplaySetOptions.Add(GetStoredOptions(missingModality, null));

			return storedDisplaySetOptions;
		}

		private static StoredDisplaySetCreationOptions GetStoredOptions(string modality, XmlNodeList optionNodes)
		{
			StoredDisplaySetCreationOptions storedOptions = new StoredDisplaySetCreationOptions(modality);

			if (optionNodes != null)
			{
				//TODO: maybe one of these days, make this extensible and add an "options tree"
				foreach (XmlNode optionNode in optionNodes)
				{
					XmlAttribute identifierAttribute = optionNode.Attributes["identifier"];
					if (identifierAttribute != null)
					{
						XmlAttribute valueAttribute;
						XmlAttribute showOriginalAttribute;

						switch (identifierAttribute.Value)
						{
							case "CreateSingleImageDisplaySets":
								if (storedOptions.CreateSingleImageDisplaySetsEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										storedOptions.CreateSingleImageDisplaySets = valueAttribute.Value == "True";
								}
								break;
							case "SplitEchos":
								if (storedOptions.SplitMultiEchoSeriesEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										storedOptions.SplitMultiEchoSeries = (valueAttribute.Value == "True");

									showOriginalAttribute = optionNode.Attributes["show-original"];
									if (showOriginalAttribute != null)
										storedOptions.ShowOriginalMultiEchoSeries = showOriginalAttribute.Value == "True";
								}
								break;
							case "SplitMixedMultiframes":
								if (storedOptions.SplitMixedMultiframesEnabled)
								{
									valueAttribute = optionNode.Attributes["value"];
									if (valueAttribute != null)
										storedOptions.SplitMixedMultiframes = (valueAttribute.Value == "True");

									showOriginalAttribute = optionNode.Attributes["show-original"];
									if (showOriginalAttribute != null)
										storedOptions.ShowOriginalMixedMultiframeSeries = showOriginalAttribute.Value == "True";
								}
								break;
							
							default:break;
						}
					}
				}
			}

			return storedOptions;
		}

		internal void Save(IEnumerable<StoredDisplaySetCreationOptions> storedOptions)
		{
			XmlDocument document = new XmlDocument();
			XmlElement root = document.CreateElement("display-set-creation-settings");
			document.AppendChild(root);

			foreach (StoredDisplaySetCreationOptions storedOption in storedOptions)
			{
				XmlElement optionsElement = document.CreateElement("display-set-creation-options");
				optionsElement.SetAttribute("modality", storedOption.Modality);

				bool append = false;
				if (storedOption.CreateSingleImageDisplaySetsEnabled)
				{
					append = true;
					XmlElement createSingleImageDisplaySetsElement = document.CreateElement("display-set-creation-option");
					createSingleImageDisplaySetsElement.SetAttribute("identifier", "CreateSingleImageDisplaySets");
					createSingleImageDisplaySetsElement.SetAttribute("value", storedOption.CreateSingleImageDisplaySets ? "True" : "False");
					optionsElement.AppendChild(createSingleImageDisplaySetsElement);
				}

				if (storedOption.SplitMultiEchoSeriesEnabled)
				{
					append = true;
					XmlElement splitEchosElement = document.CreateElement("display-set-creation-option");
					splitEchosElement.SetAttribute("identifier", "SplitEchos");
					splitEchosElement.SetAttribute("value", storedOption.SplitMultiEchoSeries ? "True" : "False");
					splitEchosElement.SetAttribute("show-original", storedOption.ShowOriginalMultiEchoSeries ? "True" : "False");
					optionsElement.AppendChild(splitEchosElement);
				}

				if (storedOption.SplitMixedMultiframesEnabled)
				{
					append = true;
					XmlElement splitMultiframesElement = document.CreateElement("display-set-creation-option");
					splitMultiframesElement.SetAttribute("identifier", "SplitMixedMultiframes");
					splitMultiframesElement.SetAttribute("value", storedOption.SplitMixedMultiframes ? "True" : "False");
					splitMultiframesElement.SetAttribute("show-original", storedOption.ShowOriginalMixedMultiframeSeries ? "True" : "False");
					optionsElement.AppendChild(splitMultiframesElement);
				}

				if (append)
					root.AppendChild(optionsElement);
			}

			this.DisplaySetCreationSettingsXml = document;
			this.Save();
		}
	}
}
