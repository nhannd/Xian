#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	internal sealed class StoredAnnotationBoxGroup
	{
		private readonly string _identifier;
		private readonly AnnotationBox _defaultBoxSettings;
		private readonly AnnotationBoxList _annotationBoxes;

		public StoredAnnotationBoxGroup(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
			_defaultBoxSettings = new AnnotationBox();
			_annotationBoxes = new AnnotationBoxList();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">This parameter is unused.</param>
		private StoredAnnotationBoxGroup(StoredAnnotationBoxGroup source, ICloningContext context)
		{
			this._identifier = source._identifier;
			this._defaultBoxSettings = source._defaultBoxSettings.Clone();
			this._annotationBoxes = source._annotationBoxes.Clone();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public AnnotationBox DefaultBoxSettings
		{
			get { return _defaultBoxSettings; }
		}

		public IList<AnnotationBox> AnnotationBoxes
		{
			get { return _annotationBoxes; }
		}

		public StoredAnnotationBoxGroup Clone()
		{
			return new StoredAnnotationBoxGroup(this, null);
		}
	}
}