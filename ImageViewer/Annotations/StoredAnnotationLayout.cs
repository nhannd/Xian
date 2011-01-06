#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	[Cloneable]
	internal sealed class StoredAnnotationLayout : IAnnotationLayout
	{
		private readonly string _identifier;
		private readonly List<StoredAnnotationBoxGroup> _annotationBoxGroups = new List<StoredAnnotationBoxGroup>();
		private bool _visible = true;

		public StoredAnnotationLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">This parameter is unused.</param>
		private StoredAnnotationLayout(StoredAnnotationLayout source, ICloningContext context)
		{
			this._identifier = source._identifier;
			this._visible = source._visible;
			foreach (StoredAnnotationBoxGroup group in source._annotationBoxGroups)
			{
				if (group == null)
					continue;
				this._annotationBoxGroups.Add(group.Clone());
			}
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public IList<StoredAnnotationBoxGroup> AnnotationBoxGroups
		{
			get { return _annotationBoxGroups; }
		}

		#region IAnnotationLayout Members

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				foreach (StoredAnnotationBoxGroup group in _annotationBoxGroups)
				{
					foreach (AnnotationBox box in group.AnnotationBoxes)
						yield return box;
				}
			}
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		#endregion

		public StoredAnnotationLayout Clone()
		{
			return new StoredAnnotationLayout(this, null);
		}
	}
}
