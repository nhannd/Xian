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

			OnSubjectChanged(null, EventArgs.Empty);
			_component.SubjectChanged += new EventHandler(OnSubjectChanged);
        }

		void OnSubjectChanged(object sender, EventArgs e)
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
