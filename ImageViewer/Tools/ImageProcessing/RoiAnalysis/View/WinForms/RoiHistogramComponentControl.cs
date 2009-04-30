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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using NPlot;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RoiHistogramComponent"/>
    /// </summary>
    public partial class RoiHistogramComponentControl : ApplicationComponentUserControl
    {
        private RoiHistogramComponent _component;
		private BindingSource _bindingSource;

		/// <summary>
        /// Constructor
        /// </summary>
        public RoiHistogramComponentControl(RoiHistogramComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_minUpDown.Minimum = Decimal.MinValue;
			_minUpDown.Maximum = Decimal.MaxValue;
			_minUpDown.Increment = 10;
			_minUpDown.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
			_minUpDown.Accelerations.Add(new NumericUpDownAcceleration(5, 100));

			_maxUpDown.Minimum = Decimal.MinValue;
			_maxUpDown.Maximum = Decimal.MaxValue;
			_maxUpDown.Increment = 10;
			_maxUpDown.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
			_maxUpDown.Accelerations.Add(new NumericUpDownAcceleration(5, 100));

			_numBinsUpDown.Minimum = 1;
			_numBinsUpDown.Maximum = 200;
			_numBinsUpDown.Increment = 5;

			_minUpDown.DataBindings.Add("Value", _bindingSource, "MinBin", true, DataSourceUpdateMode.OnPropertyChanged);
			_minUpDown.DataBindings.Add("Enabled", _bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_maxUpDown.DataBindings.Add("Value", _bindingSource, "MaxBin", true, DataSourceUpdateMode.OnPropertyChanged);
			_maxUpDown.DataBindings.Add("Enabled", _bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_numBinsUpDown.DataBindings.Add("Value", _bindingSource, "NumBins", true, DataSourceUpdateMode.OnPropertyChanged);
			_numBinsUpDown.DataBindings.Add("Enabled", _bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);

			Refresh(null, EventArgs.Empty);
			_component.AllPropertiesChanged += new EventHandler(Refresh);
        }

		void Refresh(object sender, EventArgs e)
		{
			_plotSurface.Clear();
			_plotSurface.BackColor = Color.Black;

			if (!_component.ComputeHistogram())
			{
				_plotSurface.Refresh();
				return;
			}

			HistogramPlot histogram = new HistogramPlot();
			histogram.AbscissaData = _component.BinLabels;
			histogram.OrdinateData = _component.Bins;
			histogram.Center = false;
			histogram.BaseWidth = 1.0f;
			histogram.Filled = true;
			histogram.Pen = new Pen(ClearCanvasStyle.ClearCanvasBlue);
			histogram.RectangleBrush = new RectangleBrushes.Solid(ClearCanvasStyle.ClearCanvasBlue);

			_plotSurface.Add(histogram);
			_plotSurface.PlotBackColor = Color.Black;
			_plotSurface.XAxis1.Color = Color.White;
			_plotSurface.YAxis1.Color = Color.White;
			_plotSurface.Refresh();
		}
    }
}
