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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal sealed class AnnotationLayoutStore
	{
		private static readonly AnnotationLayoutStore _instance = new AnnotationLayoutStore();

		private readonly object _syncLock = new object();
		private XmlDocument _document;
		private event EventHandler _storeChanged;

		private AnnotationLayoutStore()
		{
			AnnotationLayoutStoreSettings.Default.PropertyChanged += 
			delegate
			{
				this.Initialize(true);
			};
		}

		public event EventHandler StoreChanged
		{
			add { _storeChanged += value; }
			remove { _storeChanged -= value; }
		}

		public static AnnotationLayoutStore Instance
		{
			get{ return _instance; }
		}

		public void Clear()
		{
			lock (_syncLock)
			{
				SaveSettings("");
				Initialize(true);
			}
		}

		public void RemoveLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");

			lock (_syncLock)
			{
				Initialize(false);

				string xPath = "annotation-configuration/annotation-layouts";
				XmlElement layoutsNode = (XmlElement) _document.SelectSingleNode(xPath);
				if (layoutsNode == null)
					throw new InvalidDataException(
						String.Format(SR.ExceptionInvalidAnnotationLayoutXml, "'annotation-layouts' node does not exist"));

				xPath = String.Format("annotation-layout[@id='{0}']", identifier);
				XmlNodeList matchingNodes = layoutsNode.SelectNodes(xPath);
				foreach (XmlElement matchingNode in matchingNodes)
					layoutsNode.RemoveChild(matchingNode);

				SaveSettings(_document.OuterXml);
			}
		}

		public IList<StoredAnnotationLayout> GetLayouts(IEnumerable<IAnnotationItem> availableAnnotationItems)
		{
			lock (_syncLock)
			{
				Initialize(false);

				string xPath = "annotation-configuration/annotation-layouts/annotation-layout";
				XmlNodeList layoutNodes = _document.SelectNodes(xPath);

				StoredAnnotationLayoutDeserializer deserializer = new StoredAnnotationLayoutDeserializer(availableAnnotationItems);
				List<StoredAnnotationLayout> layouts = new List<StoredAnnotationLayout>();

				foreach (XmlElement layoutNode in layoutNodes)
                    layouts.Add(deserializer.DeserializeLayout(layoutNode));

				return layouts;
			}
		}

		public StoredAnnotationLayout GetLayout(string identifier, IEnumerable<IAnnotationItem> availableAnnotationItems)
		{
			if (String.IsNullOrEmpty(identifier))
				return null;

            string xPath = String.Format("annotation-configuration/annotation-layouts/annotation-layout[@id='{0}']", identifier);
            
            lock (_syncLock)
			{
				Initialize(false);
				XmlElement layoutNode = _document.SelectSingleNode(xPath) as XmlElement;
				if (layoutNode == null)
					return null;

				return new StoredAnnotationLayoutDeserializer(availableAnnotationItems).DeserializeLayout(layoutNode);
			}
		}

		public void Update(StoredAnnotationLayout layout)
		{
			Platform.CheckForNullReference(layout, "layout");
			Platform.CheckForEmptyString(layout.Identifier, "layout.Identifier");

			lock (_syncLock)
			{
				Initialize(false);

				try
				{
					new StoredAnnotationLayoutSerializer().SerializeLayout(layout);
					SaveSettings(_document.OuterXml);
				}
				catch
				{
					//undo any changes you may have just made.
					Initialize(true);
					throw;
				}
			}
		}

		public void Update(IEnumerable<StoredAnnotationLayout> layouts)
		{
			lock (_syncLock)
			{
				Initialize(false);

				try
				{
					StoredAnnotationLayoutSerializer serializer = new StoredAnnotationLayoutSerializer();
					foreach (StoredAnnotationLayout layout in layouts)
					{
						Platform.CheckForNullReference(layout, "layout");
						Platform.CheckForEmptyString(layout.Identifier, "layout.Identifier");

                        serializer.SerializeLayout(layout);
					}

					SaveSettings(_document.OuterXml);
				}
				catch
				{
					//undo any changes you may have just made.
					Initialize(true);
					throw;
				}
			}
		}

		private void SaveSettings(string settingsXml)
		{
			AnnotationLayoutStoreSettings.Default.LayoutSettingsXml = settingsXml;
			AnnotationLayoutStoreSettings.Default.Save();

			if (_storeChanged != null)
				_storeChanged(this, EventArgs.Empty);
		}

		private void Initialize(bool reloadSettings)
		{
			lock (_syncLock)
			{
				if (_document != null && !reloadSettings)
					return;

				try
				{
					_document = new XmlDocument();

					if (!String.IsNullOrEmpty(AnnotationLayoutStoreSettings.Default.LayoutSettingsXml))
					{
						_document.LoadXml(AnnotationLayoutStoreSettings.Default.LayoutSettingsXml);
					}
					else
					{
						XmlElement root = _document.CreateElement("annotation-configuration");
						_document.AppendChild(root);
						root.AppendChild(_document.CreateElement("annotation-layouts"));

						SaveSettings(_document.OuterXml);
					}
				}
				catch
				{
					_document = null;
					throw;
				}
			}
		}

		private class StoredAnnotationLayoutDeserializer
		{
			private readonly IEnumerable<IAnnotationItem> _availableAnnotationItems;
			
			public StoredAnnotationLayoutDeserializer(IEnumerable<IAnnotationItem> availableAnnotationItems)
			{
				Platform.CheckForNullReference(availableAnnotationItems, "availableAnnotationItems");

				_availableAnnotationItems = availableAnnotationItems;
			}

			public StoredAnnotationLayout DeserializeLayout(XmlElement layoutNode)
			{
				Platform.CheckForNullReference(layoutNode, "layoutNode");

				StoredAnnotationLayout layout = new StoredAnnotationLayout(layoutNode.GetAttribute("id"));
				XmlNodeList annotationBoxGroupNodes = layoutNode.SelectNodes("annotation-box-groups/annotation-box-group");
				if (annotationBoxGroupNodes != null)
					DeserializeAnnotationBoxGroups(layout, annotationBoxGroupNodes);

				return layout;
			}

			private void DeserializeAnnotationBoxGroups(StoredAnnotationLayout layout, XmlNodeList groupNodes)
			{
				foreach (XmlElement groupNode in groupNodes)
				{
					string newGroupId = groupNode.GetAttribute("id");
					StoredAnnotationBoxGroup newGroup = new StoredAnnotationBoxGroup(newGroupId);

					XmlElement defaultBoxSettingsNode = (XmlElement)groupNode.SelectSingleNode("default-box-settings");

					if (defaultBoxSettingsNode != null)
						DeserializeBoxSettings(newGroup.DefaultBoxSettings, defaultBoxSettingsNode);

					XmlNodeList annotationBoxNodes = groupNode.SelectNodes("annotation-boxes/annotation-box");
					if (annotationBoxNodes != null)
						DeserializeAnnotationBoxes(newGroup, annotationBoxNodes);

					layout.AnnotationBoxGroups.Add(newGroup);
				}
			}

			private void DeserializeAnnotationBoxes(StoredAnnotationBoxGroup group, XmlNodeList annotationBoxNodes)
			{
				foreach (XmlElement annotationBoxNode in annotationBoxNodes)
				{
					string normalizedRectangleString = annotationBoxNode.GetAttribute("normalized-rectangle");

					RectangleF normalizedRectangle;
					if (!DeserializeNormalizedRectangle(normalizedRectangleString, out normalizedRectangle))
						continue;

					XmlElement boxSettingsNode = (XmlElement)annotationBoxNode.SelectSingleNode("box-settings");

					AnnotationBox newBox = group.DefaultBoxSettings.Clone();
					newBox.NormalizedRectangle = normalizedRectangle;

					if (boxSettingsNode != null)
						DeserializeBoxSettings(newBox, boxSettingsNode);

					string annotationItemIdentifier = annotationBoxNode.GetAttribute("annotation-item-id");
					foreach (IAnnotationItem item in _availableAnnotationItems)
					{
						if (item.GetIdentifier() == annotationItemIdentifier)
						{
							newBox.AnnotationItem = item;
							break;
						}
					}

					group.AnnotationBoxes.Add(newBox);
				}
			}

			private void DeserializeBoxSettings(AnnotationBox boxSettings, XmlElement boxSettingsNode)
			{
				string font = boxSettingsNode.GetAttribute("font");
				string color = boxSettingsNode.GetAttribute("color");
				string italics = boxSettingsNode.GetAttribute("italics");
				string bold = boxSettingsNode.GetAttribute("bold");
				string numberOfLines = boxSettingsNode.GetAttribute("number-of-lines");
				string truncation = boxSettingsNode.GetAttribute("truncation");
				string justification = boxSettingsNode.GetAttribute("justification");
				string verticalAlignment = boxSettingsNode.GetAttribute("vertical-alignment");
				string fitWidth = boxSettingsNode.GetAttribute("fit-width");
				string alwaysVisible = boxSettingsNode.GetAttribute("always-visible");

				if (!String.IsNullOrEmpty(font))
					boxSettings.Font = font;
				if (!String.IsNullOrEmpty(color))
					boxSettings.Color = color;
				if (!String.IsNullOrEmpty(italics))
					boxSettings.Italics = (String.Compare("true", italics, true) == 0);
				if (!String.IsNullOrEmpty(bold))
					boxSettings.Bold = (String.Compare("true", bold, true) == 0);
				if (!String.IsNullOrEmpty(numberOfLines))
				{
					byte result;
					if (!byte.TryParse(numberOfLines, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result))
						result = 1;

					boxSettings.NumberOfLines = result;
				}

				if (!String.IsNullOrEmpty(fitWidth))
					boxSettings.FitWidth = (String.Compare("true", fitWidth) == 0);

				if (!String.IsNullOrEmpty(alwaysVisible))
					boxSettings.AlwaysVisible = (String.Compare("true", alwaysVisible, true) == 0);

				if (!String.IsNullOrEmpty(truncation))
				{
					AnnotationBox.TruncationBehaviour fromString = boxSettings.Truncation;
					EnumConverter converter = new EnumConverter(typeof(AnnotationBox.TruncationBehaviour));
					if (converter.IsValid(truncation))
						boxSettings.Truncation = (AnnotationBox.TruncationBehaviour)converter.ConvertFromString(truncation);
				}

				if (!String.IsNullOrEmpty(justification))
				{
					AnnotationBox.JustificationBehaviour fromString = boxSettings.Justification;
					EnumConverter converter = new EnumConverter(typeof(AnnotationBox.JustificationBehaviour));
					if (converter.IsValid(justification))
						boxSettings.Justification = (AnnotationBox.JustificationBehaviour)converter.ConvertFromString(justification);
				}

				if (!String.IsNullOrEmpty(verticalAlignment))
				{
					AnnotationBox.VerticalAlignmentBehaviour fromString = boxSettings.VerticalAlignment;
					EnumConverter converter = new EnumConverter(typeof(AnnotationBox.VerticalAlignmentBehaviour));
					if (converter.IsValid(verticalAlignment))
						boxSettings.VerticalAlignment = (AnnotationBox.VerticalAlignmentBehaviour)converter.ConvertFromString(verticalAlignment);
				}

				XmlElement configurationSettings = (XmlElement)boxSettingsNode.SelectSingleNode("configuration-settings");
				if (configurationSettings != null)
				{
					string showLabel = configurationSettings.GetAttribute("show-label");
					string showLabelIfEmpty = configurationSettings.GetAttribute("show-label-if-empty");
					if (!String.IsNullOrEmpty(showLabel))
						boxSettings.ConfigurationOptions.ShowLabel = (String.Compare("true", showLabel, true) == 0);
					if (!String.IsNullOrEmpty(showLabelIfEmpty))
						boxSettings.ConfigurationOptions.ShowLabelIfValueEmpty = (String.Compare("true", showLabelIfEmpty, true) == 0);
				}
			}

			private bool DeserializeNormalizedRectangle(string normalizedRectangleString, out RectangleF normalizedRectangle)
			{
				normalizedRectangle = new RectangleF();
				
				string[] rectangleComponents = normalizedRectangleString.Split('\\');
				if (rectangleComponents.Length != 4)
					return false;

				float left, right, top, bottom;
				if (!float.TryParse(rectangleComponents[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out left))
					return false;
				if (!float.TryParse(rectangleComponents[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out top))
					return false;
				if (!float.TryParse(rectangleComponents[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out right))
					return false;
				if (!float.TryParse(rectangleComponents[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out bottom))
					return false;

				if (left >= right)
					return false;
				if (top >= bottom)
					return false;
				if (left < 0F || left > 1.0F)
					return false;
				if (top < 0F || top > 1.0F)
					return false;
				if (right < 0F || right > 1.0F)
					return false;
				if (bottom < 0F || bottom > 1.0F)
					return false;

				normalizedRectangle = RectangleF.FromLTRB(left, top, right, bottom);
				return RectangleUtilities.IsRectangleNormalized(normalizedRectangle);
			}
		}

		private class StoredAnnotationLayoutSerializer
		{
			public StoredAnnotationLayoutSerializer()
			{
			}

			private XmlDocument Document
			{
				get { return _instance._document; }
			}

			public void SerializeLayout(StoredAnnotationLayout layout)
			{
				string xPath = "annotation-configuration/annotation-layouts";
				XmlElement layoutsNode = (XmlElement)this.Document.SelectSingleNode(xPath);
				if (layoutsNode == null)
					throw new InvalidDataException(String.Format(SR.ExceptionInvalidAnnotationLayoutXml, "'annotation-layouts' node does not exist"));

				XmlElement newLayoutNode = this.Document.CreateElement("annotation-layout");
				newLayoutNode.SetAttribute("id", layout.Identifier);

				XmlElement groupsNode = this.Document.CreateElement("annotation-box-groups");
				newLayoutNode.AppendChild(groupsNode);

				SerializeAnnotationBoxGroups(groupsNode, layout.AnnotationBoxGroups);

				xPath = String.Format("annotation-layout[@id='{0}']", layout.Identifier);
				XmlElement existingLayoutNode = (XmlElement)layoutsNode.SelectSingleNode(xPath);

				if (existingLayoutNode != null)
					layoutsNode.ReplaceChild(newLayoutNode, existingLayoutNode);
				else
					layoutsNode.AppendChild(newLayoutNode);
			}

			private void SerializeAnnotationBoxGroups(XmlElement groupsNode, IEnumerable<StoredAnnotationBoxGroup> annotationBoxGroups)
			{
				foreach (StoredAnnotationBoxGroup group in annotationBoxGroups)
				{
					XmlElement groupNode = this.Document.CreateElement("annotation-box-group");
					groupsNode.AppendChild(groupNode);

					groupNode.SetAttribute("id", group.Identifier);

					XmlElement defaultBoxSettingsNode = this.Document.CreateElement("default-box-settings");
					SerializeAnnotationBoxSettings(group.DefaultBoxSettings, new AnnotationBox(), defaultBoxSettingsNode);

					if (defaultBoxSettingsNode.ChildNodes.Count > 0 || defaultBoxSettingsNode.Attributes.Count > 0) 
						groupNode.AppendChild(defaultBoxSettingsNode);

					SerializeAnnotationBoxes(group.AnnotationBoxes, group.DefaultBoxSettings, groupNode);
				}
			}

			private void SerializeAnnotationBoxes(IList<AnnotationBox> annotationBoxes, AnnotationBox defaultBoxSettings, XmlElement groupNode)
			{
				XmlElement boxesNode = this.Document.CreateElement("annotation-boxes");
				groupNode.AppendChild(boxesNode);

				foreach (AnnotationBox box in annotationBoxes)
				{
					XmlElement boxNode = this.Document.CreateElement("annotation-box");
					boxesNode.AppendChild(boxNode);

					string normalizedRectangle = String.Format("{0:F6}\\{1:F6}\\{2:F6}\\{3:F6}", box.NormalizedRectangle.Left, box.NormalizedRectangle.Top, box.NormalizedRectangle.Right, box.NormalizedRectangle.Bottom);

					boxNode.SetAttribute("normalized-rectangle", normalizedRectangle);
					boxNode.SetAttribute("annotation-item-id", (box.AnnotationItem == null) ? "" : box.AnnotationItem.GetIdentifier());

					XmlElement settingsNode = this.Document.CreateElement("box-settings");
					SerializeAnnotationBoxSettings(box, defaultBoxSettings, settingsNode);

					if (settingsNode.ChildNodes.Count > 0 || settingsNode.Attributes.Count > 0)
						boxNode.AppendChild(settingsNode);
				}
			}

			private void SerializeAnnotationBoxSettings(AnnotationBox annotationBox, AnnotationBox defaultSettings, XmlElement boxSettingsNode)
			{
				//only save values that are different from the defaults.
				if (annotationBox.Bold != defaultSettings.Bold)
					boxSettingsNode.SetAttribute("bold", annotationBox.Bold ? "true" : "false");
				if (annotationBox.Italics != defaultSettings.Italics)
					boxSettingsNode.SetAttribute("italics", annotationBox.Italics ? "true" : "false");
				if (annotationBox.Font != defaultSettings.Font)
					boxSettingsNode.SetAttribute("font", annotationBox.Font);
				if (annotationBox.Color != defaultSettings.Color)
					boxSettingsNode.SetAttribute("color", annotationBox.Color);
				if (annotationBox.NumberOfLines != defaultSettings.NumberOfLines)
					boxSettingsNode.SetAttribute("number-of-lines", annotationBox.NumberOfLines.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (annotationBox.Truncation != defaultSettings.Truncation)
					boxSettingsNode.SetAttribute("truncation", annotationBox.Truncation.ToString());
				if (annotationBox.Justification != defaultSettings.Justification)
					boxSettingsNode.SetAttribute("justification", annotationBox.Justification.ToString());
				if (annotationBox.VerticalAlignment != defaultSettings.VerticalAlignment)
					boxSettingsNode.SetAttribute("vertical-alignment", annotationBox.VerticalAlignment.ToString());
				if (annotationBox.FitWidth != defaultSettings.FitWidth)
					boxSettingsNode.SetAttribute("fit-width", annotationBox.FitWidth ? "true" : "false");

				XmlElement configurationSettingsNode = this.Document.CreateElement("configuration-settings");
				if (annotationBox.ConfigurationOptions.ShowLabel != defaultSettings.ConfigurationOptions.ShowLabel)
					configurationSettingsNode.SetAttribute("show-label", annotationBox.ConfigurationOptions.ShowLabel ? "true" : "false");

				if (annotationBox.ConfigurationOptions.ShowLabelIfValueEmpty != defaultSettings.ConfigurationOptions.ShowLabelIfValueEmpty)
					configurationSettingsNode.SetAttribute("show-label-if-empty", annotationBox.ConfigurationOptions.ShowLabelIfValueEmpty ? "true" : "false");

				if (configurationSettingsNode.Attributes.Count > 0)
					boxSettingsNode.AppendChild(configurationSettingsNode);
			}
		}
	}
}