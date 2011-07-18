#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout
{
	public interface IHpMatchCriteriaContext
	{
		Study Study { get; }
	}


	/// <summary>
	/// Defines the interface to a "match criteria" contributor.
	/// </summary>
	public interface IHpMatchCriteriaContributor : IHpContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpMatchCriteriaContext context);

		/// <summary>
		/// Tests the criteria against the specified context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		HpMatchResult Test(IHpMatchCriteriaContext context);
	}
}
