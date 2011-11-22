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

        /// <summary>
        /// Called by the hook to fill the existing physicial workspace with images
        /// </summary>
        void PerformDefaultFillPhysicalWorkspace();
        
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

    /// <summary>
    /// Allows a hanging protocols service to hook into
    /// the layout procedure.
    /// </summary>
    public abstract class HpLayoutHook : IHpLayoutHook
    {
        /// TODO (CR Nov 2011): Rename to "Empty"
        private class Nil : IHpLayoutHook
        {
            #region IHpLayoutHook Members

            public bool HandleLayout(IHpLayoutHookContext context)
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        /// A default hook that can be used if needed; it does nothing.
        /// </summary>
        public static IHpLayoutHook Default = new Nil();

        #region IHpLayoutHook Members

        /// <summary>
        /// Handles the initial layout when a viewer is first opened.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool HandleLayout(IHpLayoutHookContext context);

        #endregion
    }
}
