using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	public abstract class StoredAnnotatationLayoutProvider : AnnotationLayoutProvider
	{
		public StoredAnnotatationLayoutProvider()
		{
		}

		protected abstract string StoredLayoutId { get; }

		#region IAnnotationLayoutProvider Members

		public override IAnnotationLayout  AnnotationLayout
		{
			get
			{
				try
				{
					return AnnotationLayoutStore.Instance.GetLayout(this.StoredLayoutId, this.AvailableAnnotationItems);
				}
				catch
				{
					throw;
				}
			}
		}

		#endregion
	}
}
