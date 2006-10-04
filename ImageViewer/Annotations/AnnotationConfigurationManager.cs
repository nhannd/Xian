using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// NOTE: This code is completely throwaway.  Right now, it just provides 
	/// a hard-coded text overlay, but the plan for the future is to make 
	/// the overlay completely user-configurable.
	/// </summary>
	public class AnnotationConfigurationManager
	{
		internal class DicomAnnotationSetup
		{
			private string _modality;
			private string _layoutIdentifier;

			public DicomAnnotationSetup(string modality, string layoutIdentifier)
			{
				_modality = modality;
				_layoutIdentifier = layoutIdentifier;
			}

			public string Modality
			{
				get { return _modality; }
			}

			public string LayoutIdentifier
			{
				get { return _layoutIdentifier; }
			}
		}

		private static DicomAnnotationSetup[] _configurations = 
		{
			new DicomAnnotationSetup("CR", "LAYOUT1"),
			new DicomAnnotationSetup("CT", "LAYOUT2"),
			new DicomAnnotationSetup("DX", "LAYOUT3"),
			new DicomAnnotationSetup("ES", "LAYOUT4"),
			new DicomAnnotationSetup("MG", "LAYOUT5"),
			new DicomAnnotationSetup("MR", "LAYOUT6"),
			new DicomAnnotationSetup("NM", "LAYOUT7"),
			new DicomAnnotationSetup("PT", "LAYOUT8"),
			new DicomAnnotationSetup("RF", "LAYOUT9"),
			new DicomAnnotationSetup("RT", "LAYOUT10"),
			new DicomAnnotationSetup("SC", "LAYOUT11"),
			new DicomAnnotationSetup("US", "LAYOUT12"),
			new DicomAnnotationSetup("XA", "LAYOUT13"),
			new DicomAnnotationSetup("OT", "LAYOUT14")
		};

		private IEnumerable<IAnnotationItem> _annotationItemCollection;
		private List<AnnotationConfiguration> _annotationConfigurations;
		private AnnotationLayoutManager _annotationLayoutManager;

		public AnnotationConfigurationManager(IEnumerable<IAnnotationItem> annotationItemCollection)
		{
			_annotationItemCollection = annotationItemCollection;
			_annotationConfigurations = new List<AnnotationConfiguration>();
			_annotationLayoutManager = new AnnotationLayoutManager(_annotationItemCollection);
		}

		protected void LoadConfigurations()
		{
			foreach (DicomAnnotationSetup setup in _configurations)
			{
				AnnotationLayout annotationLayout = _annotationLayoutManager.GetLayout(setup.LayoutIdentifier);
				if (annotationLayout == null)
					continue;

				AnnotationConfiguration configuration = new DicomAnnotationConfiguration(setup.Modality, annotationLayout);
				_annotationConfigurations.Add(configuration);
			}
		}

		public IEnumerable<AnnotationConfiguration> AnnotationConfigurations
		{
			get
			{
				if (_annotationConfigurations.Count == 0)
				{
					LoadConfigurations();
				}

				return _annotationConfigurations;
			}
		}

		public AnnotationConfiguration DefaultConfiguration
		{ 
			get
			{
				if (_annotationConfigurations.Count == 0)
				{
					LoadConfigurations();
				}

				if (_annotationConfigurations.Count == 0)
					return null;

				//return the OT configuration.
				return _annotationConfigurations[_annotationConfigurations.Count - 1];
			}
		}
	}
}
