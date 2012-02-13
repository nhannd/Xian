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
	/// Defines the interface to an applicability contributor.
	/// </summary>
	public interface IHpApplicabilityContributor : IHpContributor
	{
	}

	public interface IHpProtocolApplicabilityContext
	{
        /// <summary>
        /// The "primary" study for which the protocol applicability will be tested.
        /// </summary>
		Study PrimaryStudy { get; }
	}


	/// <summary>
	/// Defines the interface to a "protocol applicability" contributor.
	/// </summary>
	public interface IHpProtocolApplicabilityContributor : IHpApplicabilityContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpProtocolApplicabilityContext context);

		/// <summary>
		/// Tests the applicability based on the specified context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		HpApplicabilityResult Test(IHpProtocolApplicabilityContext context);
	}

	public interface IHpLayoutApplicabilityContext : IHpProtocolApplicabilityContext
	{
        StudyTree StudyTree { get; }
		ILogicalWorkspace LogicalWorkspace { get; }
	}


	/// <summary>
	/// Defines the interface to a "layout applicability" contributor.
	/// </summary>
	public interface IHpLayoutApplicabilityContributor : IHpApplicabilityContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpLayoutApplicabilityContext context);

		/// <summary>
		/// Tests the applicability based on the specified context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool Test(IHpLayoutApplicabilityContext context);
	}
}
