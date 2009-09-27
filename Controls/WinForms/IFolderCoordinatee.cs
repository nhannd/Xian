using System;

namespace ClearCanvas.Controls.WinForms
{
	internal interface IFolderCoordinatee
	{
		/// <summary>
		/// Gets the <see cref="ClearCanvas.Controls.WinForms.Pidl"/> to which the coordinatee is synchronized.
		/// </summary>
		/// <remarks>
		/// Implementations should <b>NOT</b> return a new instance of a <see cref="ClearCanvas.Controls.WinForms.Pidl"/>,
		/// as consumers will not take over ownership and disposal responsibility of the returned object.
		/// </remarks>
		Pidl Pidl { get; }

		/// <summary>
		/// Fired when <see cref="Pidl"/> changes.
		/// </summary>
		event EventHandler PidlChanged;

		/// <summary>
		/// Synchronizes the coordinatee to the specified <see cref="ClearCanvas.Controls.WinForms.Pidl"/>.
		/// </summary>
		/// <remarks>
		/// Implementations should <b>NOT</b> use the provided <paramref name="pidl"/> as is, but rather
		/// clone a new instance for which it will assume ownership and responsibility.
		/// </remarks>
		/// <param name="pidl">The <see cref="ClearCanvas.Controls.WinForms.Pidl"/> to which the coordinatee should be synchronized.</param>
		void BrowseTo(Pidl pidl);

		/// <summary>
		/// Reloads the coordinatee's view of the currently synchronized <see cref="Pidl"/>.
		/// </summary>
		void Reload();
	}
}