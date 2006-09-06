using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public class AnnotationManager
	{
		private AnnotationItemProviderManager _annotationItemProviderManager;
		private AnnotationConfigurationManager _annotationConfigurationManager;

		public AnnotationManager()
		{
			//load the provider extensions. 
			_annotationItemProviderManager = new AnnotationItemProviderManager();

			List<IAnnotationItem> listAllAnnotationItems = new List<IAnnotationItem>();

			foreach (IAnnotationItemProvider provider in _annotationItemProviderManager.ProviderCollection)
			{
				IEnumerable<IAnnotationItem> annotationItems = provider.GetAnnotationItems();
				foreach (IAnnotationItem item in annotationItems)
					listAllAnnotationItems.Add(item);
			}

			_annotationConfigurationManager = new AnnotationConfigurationManager(listAllAnnotationItems);
		}

		public IEnumerable<AnnotationBox> GetAnnotationBoxes(PresentationImage presentationImage)
		{
			foreach(AnnotationConfiguration annotationConfiguration in _annotationConfigurationManager.AnnotationConfigurations)
			{
				//Find the first annotation configuration that is appropriate for the given image.
				IEnumerable<AnnotationBox> annotationBoxes = annotationConfiguration.GetAnnotationBoxes(presentationImage);
				if (annotationBoxes != null)
					return annotationBoxes;
			}

			//return the default configuration, if there is one.
			AnnotationConfiguration defaultConfiguration = _annotationConfigurationManager.DefaultConfiguration;
			if (defaultConfiguration != null)
				return defaultConfiguration.AnnotationBoxes;

			return null;
		}
	}
}
