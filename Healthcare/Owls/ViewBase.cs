#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Abstract base class for implementations of <see cref="IView"/>.
	/// </summary>
	public abstract class ViewBase : IView
	{
		/// <summary>
		/// Gets the builder for this view.
		/// </summary>
		/// <returns></returns>
		public abstract IViewBuilder CreateBuilder();

		/// <summary>
		/// Gets the updater for this view.
		/// </summary>
		/// <returns></returns>
		public abstract IViewUpdater CreateUpdater();

		/// <summary>
		/// Gets the shrinker for this view.
		/// </summary>
		/// <returns></returns>
		public abstract IViewShrinker CreateShrinker();
	}
}
