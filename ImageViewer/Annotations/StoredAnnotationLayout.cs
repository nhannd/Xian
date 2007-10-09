using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal sealed class StoredAnnotationLayout : IAnnotationLayout
	{
		private string _identifier;
		private List<StoredAnnotationBoxGroup> _annotationBoxGroups;

		public StoredAnnotationLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
			_annotationBoxGroups = new List<StoredAnnotationBoxGroup>();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public StoredAnnotationBoxGroup this [string groupId]
		{
			get
			{
				return _annotationBoxGroups.Find(delegate(StoredAnnotationBoxGroup group){ return group.Identifier == groupId; });
			}
		}

		public IList<StoredAnnotationBoxGroup> AnnotationBoxGroups
		{
			get { return _annotationBoxGroups; }
		}

		#region IAnnotationLayout

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				List<AnnotationBox> completeList = new List<AnnotationBox>();

				foreach (StoredAnnotationBoxGroup group in _annotationBoxGroups)
					completeList.AddRange(group.AnnotationBoxes);

				return completeList;
			}
		}

		#endregion
	}
}
