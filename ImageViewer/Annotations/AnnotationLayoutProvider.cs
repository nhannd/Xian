using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	public abstract class AnnotationLayoutProvider : IAnnotationLayoutProvider
	{
		public AnnotationLayoutProvider()
		{ 
		}

		protected virtual IList<IAnnotationItemProvider> Providers
		{
			get { return AnnotationItemProviderManager.Instance.Providers; }
		}

		protected virtual IList<IAnnotationItem> AvailableAnnotationItems
		{
			get
			{
				List<IAnnotationItem> completeList = new List<IAnnotationItem>();

				foreach (IAnnotationItemProvider provider in this.Providers)
					completeList.AddRange(provider.GetAnnotationItems());

				return completeList;
			}
		}

		protected IAnnotationItem GetAnnotationItem(string identifier)
		{
			foreach (IAnnotationItemProvider provider in this.Providers)
			{
				foreach (IAnnotationItem item in provider.GetAnnotationItems())
				{
					if (identifier == item.GetIdentifier())
						return item;
				}
			}

			return null;
		}

		#region IAssociateAnnotationLayout Members

		public abstract IAnnotationLayout AnnotationLayout { get; } 

		#endregion
	}
}