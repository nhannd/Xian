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
	/// <summary>
	/// Defines the interface to a "match criteria" contributor.
	/// </summary>
	public interface IHpApplicabilityContributor<TContext> : IHpContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(TContext context);

		/// <summary>
		/// Tests the applicability based on the specified context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		HpMatchResult Test(TContext context);
	}

	public interface IHpProtocolApplicabilityContext
	{
		Study Study { get; }
	}


	/// <summary>
	/// Defines the interface to a "protocol applicability" contributor.
	/// </summary>
	public interface IHpProtocolApplicabilityContributor : IHpApplicabilityContributor<IHpProtocolApplicabilityContext>
	{
	}

	public interface IHpLayoutApplicabilityContext
	{
	}


	/// <summary>
	/// Defines the interface to a "layout applicability" contributor.
	/// </summary>
	public interface IHpLayoutApplicabilityContributor : IHpApplicabilityContributor<IHpLayoutApplicabilityContext>
	{
	}
}
