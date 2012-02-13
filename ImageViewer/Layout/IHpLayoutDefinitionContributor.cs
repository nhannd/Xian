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
	public interface IHpLayoutDefinitionContext
	{
		/// <summary>
		/// Gets the primary study.
		/// </summary>
		Study PrimaryStudy { get; }

        /// <summary>
        /// Gets the study tree.
        /// </summary>
        StudyTree StudyTree { get; }

        /// <summary>
        /// Gets the relevant logical workspace.
        /// </summary>
        ILogicalWorkspace LogicalWorkspace { get; }
        
        /// <summary>
		/// Gets the relevant physical workspace.
		/// </summary>
		IPhysicalWorkspace PhysicalWorkspace { get; }
    }

	/// <summary>
	/// Defines the interface to a "layout definition" contributor.
	/// </summary>
	public interface IHpLayoutDefinitionContributor : IHpContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpLayoutDefinitionContext context);

		/// <summary>
		/// Applies the state to the specified context.
		/// </summary>
		/// <param name="context"></param>
		void ApplyTo(IHpLayoutDefinitionContext context);
	}
}
