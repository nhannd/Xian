#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Layout
{
	public interface IHpImageBoxDefinitionContext : IHpLayoutDefinitionContext // should probably inherit everything from layout level context
	{
	    //TODO (CR July 2011): Not entirely sure about this, but if we don't do it,
        //then the "prior placeholder" has values for W/L, etc even when the display set wasn't captured.
        /// <summary>
        /// Gets whether or not the display set itself was successfully captured.
        /// </summary>
        bool DisplaySetCaptured { get; set; }

		/// <summary>
		/// Gets the relevant image box.
		/// </summary>
		IImageBox ImageBox { get; }
	}

	/// <summary>
	/// Defines the interface to an "imagebox definition" contributor.
	/// </summary>
	public interface IHpImageBoxDefinitionContributor : IHpContributor
	{
		/// <summary>
		/// Captures the state from the specified context.
		/// </summary>
		/// <param name="context"></param>
		void Capture(IHpImageBoxDefinitionContext context);

		/// <summary>
		/// Applies the state to the specified context.
		/// </summary>
		/// <param name="context"></param>
		void ApplyTo(IHpImageBoxDefinitionContext context);
	}
}
