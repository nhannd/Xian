#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.View.WinForms
{
	public partial class FusionControlPanelComponentControl : UserControl
	{
		public FusionControlPanelComponentControl(FusionControlPanelComponent component)
		{
			InitializeComponent();

			var layerBinding = new Binding("Checked", component, "ActiveLayer", true, DataSourceUpdateMode.OnPropertyChanged);
			layerBinding.Format += ConvertControlRadioButtonChecked;
			layerBinding.Parse += ConvertControlRadioButtonChecked;
			radioButton1.DataBindings.Add(layerBinding);

			var alphaBinding = new Binding("Value", component, "OverlayOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			alphaBinding.Format += ConvertSliderValuePercent;
			alphaBinding.Parse += ConvertSliderValuePercent;
			_sliderAlpha.DataBindings.Add(alphaBinding);

			checkBox1.DataBindings.Add("Checked", component, "HideOverlayBackground", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private static void ConvertControlRadioButtonChecked(object sender, ConvertEventArgs e)
		{
			if (e.Value is bool && e.DesiredType == typeof (FusionPresentationImageLayer))
			{
				bool fValue = (bool)e.Value;
				e.Value = fValue ? FusionPresentationImageLayer.Base : FusionPresentationImageLayer.Overlay;
			}
			else if (e.Value is FusionPresentationImageLayer && e.DesiredType == typeof (bool))
			{
				FusionPresentationImageLayer eValue = (FusionPresentationImageLayer)e.Value;
				e.Value = eValue == FusionPresentationImageLayer.Base;
			}
		}

		private static void ConvertSliderValuePercent(object sender, ConvertEventArgs e)
		{
			if (e.Value is int && e.DesiredType == typeof (float))
			{
				int iValue = (int) e.Value;
				e.Value = iValue/1000f;
			}
			else if (e.Value is float && e.DesiredType == typeof (int))
			{
				float fValue = (float) e.Value;
				e.Value = (int) (fValue*1000);
			}
		}
	}
}