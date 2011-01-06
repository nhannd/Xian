#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="WorklistTimeWindowEditorComponent"/>
	/// </summary>
	public partial class WorklistTimeWindowEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly WorklistTimeWindowEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistTimeWindowEditorComponentControl(WorklistTimeWindowEditorComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_fixedWindowRadioButton.DataBindings.Add("Checked", _component, "IsFixedTimeWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_fixedWindowRadioButton.DataBindings.Add("Enabled", _component, "FixedSlidingChoiceEnabled", true, DataSourceUpdateMode.Never);
			_slidingWindowRadioButton.DataBindings.Add("Checked", _component, "IsSlidingTimeWindow", true, DataSourceUpdateMode.Never);
			_slidingWindowRadioButton.DataBindings.Add("Enabled", _component, "FixedSlidingChoiceEnabled", true, DataSourceUpdateMode.Never);

			_slidingScale.DataSource = _component.SlidingScaleChoices;
			_slidingScale.DataBindings.Add("Value", _component, "SlidingScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_slidingScale.DataBindings.Add("Enabled", _component, "SlidingScaleEnabled", true, DataSourceUpdateMode.Never);


			_fromCheckBox.DataBindings.Add("Checked", _component, "StartTimeChecked", true, DataSourceUpdateMode.OnPropertyChanged);
			_fromFixed.DataBindings.Add("Enabled", _component, "FixedStartTimeEnabled", true, DataSourceUpdateMode.Never);
			_fromFixed.DataBindings.Add("Value", _component, "FixedStartTime", true, DataSourceUpdateMode.OnPropertyChanged);

			_fromSliding.DataBindings.Add("Enabled", _component, "SlidingStartTimeEnabled", true, DataSourceUpdateMode.Never);
			_fromSliding.DataBindings.Add("Value", _component, "SlidingStartTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_fromSliding.Format = _component.FormatSlidingTime;

			_toCheckBox.DataBindings.Add("Checked", _component, "EndTimeChecked", true, DataSourceUpdateMode.OnPropertyChanged);
			_toFixed.DataBindings.Add("Enabled", _component, "FixedEndTimeEnabled", true, DataSourceUpdateMode.Never);
			_toFixed.DataBindings.Add("Value", _component, "FixedEndTime", true, DataSourceUpdateMode.OnPropertyChanged);

			_toSliding.DataBindings.Add("Enabled", _component, "SlidingEndTimeEnabled", true, DataSourceUpdateMode.Never);
			_toSliding.DataBindings.Add("Value", _component, "SlidingEndTime", true, DataSourceUpdateMode.OnPropertyChanged);
			_toSliding.Format = _component.FormatSlidingTime;
		}
	}
}
