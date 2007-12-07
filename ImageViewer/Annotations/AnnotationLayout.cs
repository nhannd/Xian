using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal class EmptyAnnotationLayout : AnnotationLayout
	{
		public EmptyAnnotationLayout()
		{
		}

		public override IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get { yield break; }
		}
	}

	/// <summary>
	/// Abstract base class for <see cref="IAnnotationLayout"/>.
	/// </summary>
	public abstract class AnnotationLayout : IAnnotationLayout
	{
		private static readonly IAnnotationLayout _empty = new EmptyAnnotationLayout();

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AnnotationLayout()
		{
		}

		/// <summary>
		/// Gets an empty <see cref="IAnnotationLayout"/>.
		/// </summary>
		public static IAnnotationLayout Empty
		{
			get { return _empty; }	
		}

		#region IAnnotationLayout Members

		/// <summary>
		/// Gets the entire set of <see cref="AnnotationBox"/>es.
		/// </summary>
		public abstract IEnumerable<AnnotationBox> AnnotationBoxes { get; }

		#endregion
	}
}
