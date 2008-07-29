using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines a base interface for providing extension pages to the application.
	/// </summary>
	/// <typeparam name="TPage">The interface to the extension page.</typeparam>
	/// <typeparam name="TContext">The interface to the context which is passed to the extension page.</typeparam>
	public interface IExtensionPageProvider<TPage, TContext>
		where TPage : IExtensionPage
	{
		TPage[] GetPages(TContext context);
	}

	/// <summary>
	/// Defines a base interface to an extension page.
	/// </summary>
	public interface IExtensionPage
	{
		/// <summary>
		/// Gets the path to the extension page.  The meaning of this path depends upon the type of container
		/// in which the page is displayed.
		/// </summary>
		Path Path { get; }

		/// <summary>
		/// Gets the application component that implements the extension page functionality.  This method
		/// will be called exactly once during the lifetime of the page.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetComponent();
	}

}
