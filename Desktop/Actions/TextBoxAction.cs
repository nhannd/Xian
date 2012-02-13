#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that displays a text box into which the user can type.
	/// </summary>
	public class TextBoxAction : Action, ITextBoxAction
	{
		private string _textValue;
		private string _cueText;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID"></param>
		/// <param name="path"></param>
		/// <param name="resourceResolver"></param>
		public TextBoxAction(string actionID, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
		}

		#region Implementation of ITextBoxAction

		/// <summary>
		/// Occurs when the value of <see cref="ITextBoxAction.TextValue"/> changes.
		/// </summary>
		public event EventHandler TextValueChanged;

		/// <summary>
		/// Gets or sets the value of the text displayed in the text box.
		/// </summary>
		public string TextValue
		{
			get { return _textValue; }
			set
			{ 
				if(value != _textValue)
				{
					_textValue = value;
					EventsHelper.Fire(TextValueChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Occurs when the value of <see cref="ITextBoxAction.CueText"/> changes.
		/// </summary>
		public event EventHandler CueTextChanged;

		/// <summary>
		/// Gets or sets the cue text displayed in the text box when it does not have focus.
		/// </summary>
		public string CueText
		{
			get { return _cueText; }
			set
			{
				if (value != _cueText)
				{
					_cueText = value;
					EventsHelper.Fire(CueTextChanged, this, EventArgs.Empty);
				}
			}
		}

		#endregion
	}
}
