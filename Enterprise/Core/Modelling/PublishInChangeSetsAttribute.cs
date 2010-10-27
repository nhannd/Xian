#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// When applied to an entity class, specifies whether that class should be published in entity change-sets.
	/// </summary>
	/// <remarks>
	/// Entity classes are published in change sets by default.  Therefore this attribute need only be applied for the purpose
	/// of excluding an entity class from change-sets.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class PublishInChangeSetsAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isPublishable"></param>
		public PublishInChangeSetsAttribute(bool isPublishable)
		{
			IsPublishable = isPublishable;
		}

		/// <summary>
		/// Gets a value indicating whether the entity is publishable.
		/// </summary>
		public bool IsPublishable { get; private set; }
	}
}
