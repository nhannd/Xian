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

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Base implementation of <see cref="IAnnotationItemProvider"/>.
	/// </summary>
	/// <seealso cref="IAnnotationItemProvider"/>
	public abstract class AnnotationItemProvider : IAnnotationItemProvider
	{
		private readonly string _identifier;
		private readonly string _displayName;

		/// <summary>
		/// A constructor that uses the <see cref="AnnotationItemProvider"/>'s unique identifier to determine
		/// the display name using an <see cref="IAnnotationResourceResolver"/>.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItemProvider"/>.</param>
		/// <param name="resolver">The object that will resolve the display name from 
		/// the <see cref="AnnotationItemProvider"/>'s unique identifier.</param>
		protected AnnotationItemProvider(string identifier, IAnnotationResourceResolver resolver)
			: this(identifier, resolver.ResolveDisplayName(identifier))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItemProvider"/>.</param>
		/// <param name="displayName">The <see cref="AnnotationItemProvider"/>'s display name.</param>
		protected AnnotationItemProvider(string identifier, string displayName)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(displayName, "displayName");

			_identifier = identifier;
			_displayName = displayName;
		}

		#region IAnnotationItemProvider Members

		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		public string GetIdentifier()
		{
			return _identifier;
		}

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		public string GetDisplayName()
		{
			return _displayName;
		}

		/// <summary>
		/// Gets the logical group of <see cref="IAnnotationItem"/>s.
		/// </summary>
		public abstract IEnumerable<IAnnotationItem> GetAnnotationItems();

		#endregion
	}
}
