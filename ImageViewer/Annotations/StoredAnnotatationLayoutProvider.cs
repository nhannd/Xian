using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal abstract class StoredAnnotatationLayoutProvider : AnnotationLayoutProvider
	{
		protected StoredAnnotatationLayoutProvider()
		{
		}

		protected abstract string StoredLayoutId { get; }

		#region IAnnotationLayoutProvider Members

		public override IAnnotationLayout  AnnotationLayout
		{
			get
			{
				IAnnotationLayout returnLayout;

				try
				{
					returnLayout = AnnotationLayoutStore.Instance.GetLayout(this.StoredLayoutId, this.AvailableAnnotationItems);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);

					StoredAnnotationLayout layout = new StoredAnnotationLayout("error");
					layout.AnnotationBoxGroups.Add(new StoredAnnotationBoxGroup("errorgroup"));
					IAnnotationItem item = new BasicTextAnnotationItem("errorbox", "errorbox", SR.LabelError, SR.MessageErrorLoadingAnnotationLayout);

					AnnotationBox box = new AnnotationBox(new RectangleF(0.5F,0.90F, 0.5F, 0.10F), item);
					box.Bold = true;
					box.Color = "Red";
					box.Justification = AnnotationBox.JustificationBehaviour.Far;
					box.NumberOfLines = 5;
					box.VerticalAlignment = AnnotationBox.VerticalAlignmentBehaviour.Bottom;

					layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(box);
					returnLayout = layout;
				}

				return returnLayout;
			}
		}

		#endregion
	}
}
