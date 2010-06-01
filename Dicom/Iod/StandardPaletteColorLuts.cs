#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Reflection;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.Iod
{
	partial class PaletteColorLut
	{
		private static PaletteColorLut _hotIron;
		private static PaletteColorLut _hotMetalBlue;
		private static PaletteColorLut _pet20Step;
		private static PaletteColorLut _pet;

		/// <summary>
		/// Gets the Hot Iron standard color palette.
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2009, Part 6, Section B.1.1</remarks>
		public static PaletteColorLut HotIron
		{
			get
			{
				if (_hotIron == null)
					_hotIron = CreateFromColorPaletteSopInstanceXml("Iod.Resources.HotIronStandardColorPalette.xml");
				return _hotIron;
			}
		}

		/// <summary>
		/// Gets the Hot Metal Blue standard color palette.
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2009, Part 6, Section B.1.3</remarks>
		public static PaletteColorLut HotMetalBlue
		{
			get
			{
				if (_hotMetalBlue == null)
					_hotMetalBlue = CreateFromColorPaletteSopInstanceXml("Iod.Resources.HotMetalBlueStandardColorPalette.xml");
				return _hotMetalBlue;
			}
		}

		/// <summary>
		/// Gets the PET 20 Step standard color palette.
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2009, Part 6, Section B.1.4</remarks>
		public static PaletteColorLut PET20Step
		{
			get
			{
				if (_pet20Step == null)
					_pet20Step = CreateFromColorPaletteSopInstanceXml("Iod.Resources.PET20StepStandardColorPalette.xml");
				return _pet20Step;
			}
		}

		/// <summary>
		/// Gets the PET standard color palette.
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2009, Part 6, Section B.1.2</remarks>
		public static PaletteColorLut PET
		{
			get
			{
				if (_pet == null)
					_pet = CreateFromColorPaletteSopInstanceXml("Iod.Resources.PETStandardColorPalette.xml");
				return _pet;
			}
		}

		private static PaletteColorLut CreateFromColorPaletteSopInstanceXml(string resourceName)
		{
			try
			{
				var resourceResolver = new ResourceResolver(Assembly.GetExecutingAssembly());
				using (var xmlStream = resourceResolver.OpenResource(resourceName))
				{
					var xmlDocument = new XmlDocument();
					xmlDocument.Load(xmlStream);
					var docRootNode = CollectionUtils.FirstElement(xmlDocument.GetElementsByTagName("ClearCanvasColorPaletteDefinition")) as XmlElement;
					if (docRootNode != null)
					{
						var instanceNode = CollectionUtils.FirstElement(docRootNode.GetElementsByTagName("Instance")) as XmlElement;
						if (instanceNode != null)
						{
							var instanceXml = new InstanceXml(instanceNode, null);
							return Create(instanceXml.Collection);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to load embedded standard color palette SOP from resource {0}", resourceName);
			}
			return null;
		}
	}
}