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
using System.Configuration;
using System.IO;
using ClearCanvas.Desktop;
using System.Xml;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[SettingsGroupDescription("Stores the user's initial layout preferences for studies opened in the viewer")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class LayoutSettings
	{
		public static readonly int DefaultImageBoxRows = 1;
		public static readonly int DefaultImageBoxColumns = 2;
		public static readonly int DefaultTileRows = 1;
		public static readonly int DefaultTileColumns = 1;

		public static readonly int MaximumImageBoxRows = 4;
		public static readonly int MaximumImageBoxColumns = 8;
		public static readonly int MaximumTileRows = 4;
		public static readonly int MaximumTileColumns = 4;

		private LayoutSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public StoredLayout DefaultLayout
		{
			get
			{
				return this.GetDefaultLayout(this.Layouts);
			}
		}
		
		public IList<StoredLayout> Layouts
		{
			get
			{
				List<StoredLayout> layouts = new List<StoredLayout>();
				XmlDocument document = this.LayoutSettingsXml;
				if (document == null)
				{
					document = new XmlDocument();
					Stream stream = new ResourceResolver(this.GetType(), false).OpenResource("LayoutDefaults.xml");
					document.Load(stream);
					stream.Close();
				}

				StoredLayout defaultLayout = null;

				XmlNodeList layoutNodes = document.SelectNodes("//layout");
				if (layoutNodes == null || layoutNodes.Count == 0)
				{
					document = new XmlDocument();
					Stream stream = new ResourceResolver(this.GetType(), false).OpenResource("LayoutDefaults.xml");
					document.Load(stream);
					stream.Close();
					layoutNodes = document.SelectNodes("//layout");
				}

				foreach (XmlElement layoutNode in layoutNodes)
				{
					StoredLayout newLayout = ConstructDefaultLayout(layoutNode.GetAttribute("modality"));

					newLayout.ImageBoxRows = Convert.ToInt32(layoutNode.GetAttribute("image-box-rows"), System.Globalization.CultureInfo.InvariantCulture);
					newLayout.ImageBoxColumns = Convert.ToInt32(layoutNode.GetAttribute("image-box-columns"), System.Globalization.CultureInfo.InvariantCulture);
					newLayout.TileRows = Convert.ToInt32(layoutNode.GetAttribute("tile-rows"), System.Globalization.CultureInfo.InvariantCulture);
					newLayout.TileColumns = Convert.ToInt32(layoutNode.GetAttribute("tile-columns"), System.Globalization.CultureInfo.InvariantCulture);

					//push the default to the end.
					if (newLayout.IsDefault)
					{
						//make sure there's only one default!
						if (defaultLayout == null)
							defaultLayout = newLayout;

						continue;
					}

					layouts.Add(newLayout);
				}

				if (defaultLayout == null)
					defaultLayout = ConstructDefaultLayout();

				layouts.Add(defaultLayout);

				return layouts;
			}
			set
			{
				if (value != null)
				{
					XmlDocument document = new XmlDocument();
					XmlElement root = document.CreateElement("layout-configuration");
					XmlNode layoutsNode = root.AppendChild(document.CreateElement("layouts"));
					document.AppendChild(root);

					foreach (StoredLayout layout in value)
					{
						XmlElement newLayoutNode = document.CreateElement("layout");
						
						newLayoutNode.SetAttribute("modality", layout.Modality);
						newLayoutNode.SetAttribute("image-box-rows", layout.ImageBoxRows.ToString(System.Globalization.CultureInfo.InvariantCulture));
						newLayoutNode.SetAttribute("image-box-columns", layout.ImageBoxColumns.ToString(System.Globalization.CultureInfo.InvariantCulture));
						newLayoutNode.SetAttribute("tile-rows", layout.TileRows.ToString(System.Globalization.CultureInfo.InvariantCulture));
						newLayoutNode.SetAttribute("tile-columns", layout.TileColumns.ToString(System.Globalization.CultureInfo.InvariantCulture));

						layoutsNode.AppendChild(newLayoutNode);
					}

					this.LayoutSettingsXml = document;
					this.Save();
				}
			}
		}

		public StoredLayout GetLayout(string modality)
		{
			foreach (StoredLayout layout in this.Layouts)
			{
				if (!layout.IsDefault && layout.Modality == modality)
					return layout;
			}

			return GetDefaultLayout(this.Layouts);
		}

		public StoredLayout GetLayout(ImageSop imageSop)
		{
			if (imageSop == null)
				return this.DefaultLayout;

			return GetLayout(imageSop.Modality);
		}

		public StoredLayout GetLayout(IImageSopProvider imageSopProvider)
		{
			if (imageSopProvider == null)
				return this.DefaultLayout;

			return GetLayout(imageSopProvider.ImageSop);
		}

		public static StoredLayout GetMinimumLayout()
		{
			return new StoredLayout("", 1, 1, 1, 1);
		}

		private StoredLayout ConstructDefaultLayout()
		{
			return ConstructDefaultLayout("");
		}

		private static StoredLayout ConstructDefaultLayout(string modality)
		{
			return new StoredLayout(modality, DefaultImageBoxRows,
														DefaultImageBoxColumns,
														DefaultTileRows,
														DefaultTileColumns);
		}

		private StoredLayout GetDefaultLayout(IList<StoredLayout> layouts)
		{
			foreach (StoredLayout layout in layouts)
			{
				if (layout.IsDefault)
					return layout;
			}

			return ConstructDefaultLayout();
		}
	}
}
