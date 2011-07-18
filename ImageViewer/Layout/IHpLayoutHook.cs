#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Layout
{
	public interface IHpLayoutHookContext
	{
		/// <summary>
		/// Gets the relevant image viewer.
		/// </summary>
		IImageViewer ImageViewer { get; }

        /// <summary>
        /// Called by the hook to lay out the default physicial workspace
        /// </summary>
        void PerformDefaultPhysicalWorkspaceLayout();
        
	}

	/// <summary>
	/// Defines an interface that allows a hanging protocols service to hook into
	/// the layout procedure.
	/// </summary>
	public interface IHpLayoutHook
	{
		/// <summary>
		/// Handles the initial layout when a viewer is first opened.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool HandleLayout(IHpLayoutHookContext context);
	}
}
