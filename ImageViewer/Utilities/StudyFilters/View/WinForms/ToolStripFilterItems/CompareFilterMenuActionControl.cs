#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public partial class CompareFilterMenuActionControl : UserControl
	{
		private readonly Size _defaultSize;
		private readonly CompareFilterMenuAction _action;

		public CompareFilterMenuActionControl(CompareFilterMenuAction action)
		{
			InitializeComponent();
			_defaultSize = base.Size;

			_action = action;

			_txtValue.Text = _action.Value;

			UpdateMode();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			return new Size(Math.Max(_defaultSize.Width, size.Width), Math.Max(_defaultSize.Height, size.Height));
		}

		private void UpdateMode()
		{
			// tooltips are buggy since they stay on screen after the mouse moves off the control (provider cannot detect when to clear it)
			switch (_action.CurrentMode)
			{
				case CompareFilterMode.LessThan:
					_modeToggle.Text = Resources.LabelLessThanOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipLessThanOperator);
					break;
				case CompareFilterMode.LessThenOrEquals:
					_modeToggle.Text = Resources.LabelLessThanOrEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipLessThanOrEqualsOperator);
					break;
				case CompareFilterMode.GreaterThan:
					_modeToggle.Text = Resources.LabelGreaterThanOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipGreaterThanOperator);
					break;
				case CompareFilterMode.GreaterThanOrEquals:
					_modeToggle.Text = Resources.LabelGreaterThanOrEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipGreaterThanOrEqualsOperator);
					break;
				case CompareFilterMode.NotEquals:
					_modeToggle.Text = Resources.LabelNotEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipNotEqualsOperator);
					break;
				case CompareFilterMode.Equals:
				default:
					_modeToggle.Text = Resources.LabelEqualsOperator;
					//_tooltipProvider.SetToolTip(_modeToggle, Resources.TooltipEqualsOperator);
					break;
			}
		}

		private void modeToggle_Click(object sender, EventArgs e)
		{
			IList<CompareFilterMode> list = _action.AllowedModes;
			int index = (Math.Max(-1, list.IndexOf(_action.CurrentMode)) + 1)%list.Count;
			_action.CurrentMode = list[index];
			UpdateMode();
		}

		private void _txtValue_TextChanged(object sender, EventArgs e)
		{
			try
			{
				_action.Value = _txtValue.Text;
				_errorProvider.Clear();
			}
			catch (Exception)
			{
				_errorProvider.SetError(this, Resources.MessageParsingFailed);
			}
		}
	}
}