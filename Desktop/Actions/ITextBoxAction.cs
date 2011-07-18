#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that displays a text box into which the user can type.
	/// </summary>
	public interface ITextBoxAction : IAction
	{
		/// <summary>
		/// Occurs when the value of <see cref="TextValue"/> changes.
		/// </summary>
		event EventHandler TextValueChanged;

		/// <summary>
		/// Gets or sets the value of the text displayed in the text box.
		/// </summary>
		string TextValue { get; set; }


		/// <summary>
		/// Occurs when the value of <see cref="CueText"/> changes.
		/// </summary>
		event EventHandler CueTextChanged;

		/// <summary>
		/// Gets or sets the cue text displayed in the text box when it does not have focus.
		/// </summary>
		string CueText { get; set; }

	}
}
