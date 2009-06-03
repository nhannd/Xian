#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.StudyLoaders.Streaming;
using NPlot;
using System.Threading;
using Timer=System.Windows.Forms.Timer;

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StreamingAnalysisComponent"/>.
    /// </summary>
    public partial class StreamingAnalysisComponentControl : ApplicationComponentUserControl
    {
        private StreamingAnalysisComponent _component;
    	private Timer _timer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StreamingAnalysisComponentControl(StreamingAnalysisComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_addSelectedStudies.DataBindings.Add("Enabled", component, "CanAddSelectedStudies", true,
        	                                     DataSourceUpdateMode.OnPropertyChanged);

			_decompressActive.DataBindings.Add("Checked", component, "DecompressActive", true,
										 DataSourceUpdateMode.OnPropertyChanged);

        	_decompressActive.Click += delegate { _component.DecompressActive = !_component.DecompressActive; };
			
			_retrieveActive.DataBindings.Add("Checked", component, "RetrieveActive", true,
										 DataSourceUpdateMode.OnPropertyChanged);

			_retrieveActive.Click += delegate { _component.RetrieveActive = !_component.RetrieveActive; };
			
			_retrieveItems.DataBindings.Add("Value", component, "NumberOfRetrieveItems", true,
        	                                DataSourceUpdateMode.OnPropertyChanged);

			_retrieveConcurrency.DataBindings.Add("Value", component, "RetrieveConcurrency", true,
										 DataSourceUpdateMode.OnPropertyChanged);

			_decompressConcurrency.DataBindings.Add("Value", component, "DecompressConcurrency", true,
								 DataSourceUpdateMode.OnPropertyChanged);

			_decompressItems.DataBindings.Add("Value", component, "NumberOfDecompressItems", true,
									DataSourceUpdateMode.OnPropertyChanged);

			_retrievePriority.DataSource = GetThreadPriorities();
			//_retrievePriority.DataBindings.Add("SelectedValue", component, "RetrieveThreadPriority", true,
			//                                   DataSourceUpdateMode.OnPropertyChanged);

        	_retrievePriority.SelectedValueChanged +=
        		delegate { _component.RetrieveThreadPriority = (ThreadPriority)_retrievePriority.SelectedValue; };
			_retrievePriority.SelectedItem = _component.RetrieveThreadPriority;

			_decompressPriority.DataSource = GetThreadPriorities();
			//_decompressPriority.DataBindings.Add("SelectedValue", component, "DecompressThreadPriority", true,
			//                       DataSourceUpdateMode.OnPropertyChanged);

			_decompressPriority.SelectedValueChanged +=
				delegate { _component.DecompressThreadPriority = (ThreadPriority)_decompressPriority.SelectedValue; };

			_decompressPriority.SelectedItem = _component.DecompressThreadPriority;
        	

        	//_plotAverage.ValueChanged += delegate { RefreshRetrievePlot(); };

			_timer = new Timer();
        	_timer.Interval = 5000;
			_timer.Tick += new EventHandler(OnTimer);
        	_timer.Enabled = true;
		}

		private List<ThreadPriority> GetThreadPriorities()
		{
			List<ThreadPriority> priorities = new List<ThreadPriority>();
			priorities.Add(ThreadPriority.Highest);
			priorities.Add(ThreadPriority.AboveNormal);
			priorities.Add(ThreadPriority.Normal);
			priorities.Add(ThreadPriority.BelowNormal);
			priorities.Add(ThreadPriority.Lowest);
			return priorities;
		}

		void OnTimer(object sender, EventArgs e)
		{
			RefreshRetrievePlot();
		}

		private void RefreshRetrievePlot()
		{
			_retrieveSpeedPlot.Clear();
			PointPlot plot = new PointPlot();

			List<DateTime> timePoints;
			List<double> retrieveMbPerSecond;
			ComputePlotAverage(out timePoints, out retrieveMbPerSecond);

			plot.AbscissaData = timePoints;
			plot.DataSource = retrieveMbPerSecond;

			Grid grid = new Grid();
			grid.HorizontalGridType = Grid.GridType.Coarse;
			grid.VerticalGridType = Grid.GridType.Coarse;

			_retrieveSpeedPlot.Add(grid);
			_retrieveSpeedPlot.Add(plot);

			_retrieveSpeedPlot.ShowCoordinates = true;
			_retrieveSpeedPlot.YAxis1.Label = "Mb/sec";
			_retrieveSpeedPlot.YAxis1.LabelOffsetAbsolute = true;
			_retrieveSpeedPlot.YAxis1.LabelOffset = 40;
			_retrieveSpeedPlot.Padding = 5;

			//Align percent plot axes.
			DateTimeAxis ax = new DateTimeAxis(_retrieveSpeedPlot.XAxis1);
			ax.HideTickText = false;
			_retrieveSpeedPlot.XAxis1 = ax;

			_retrieveSpeedPlot.Refresh();
		}

		private void ComputePlotAverage(out List<DateTime> timePoints, out List<double> retrieveMbPerSecond)
		{
			timePoints = new List<DateTime>();
			retrieveMbPerSecond = new List<double>();

			for (int i = 0; i < _component.PerformanceInfo.Count; ++i)
			{
				DateTime minStartTime = DateTime.MaxValue;
				DateTime maxEndTime = DateTime.MinValue;
				long totalBytes = 0;
				
				for (int j = 0; j < (int)_plotAverage.Value && i < _component.PerformanceInfo.Count; ++j, ++i)
				{
					StreamingPerformanceInfo performanceInfo = _component.PerformanceInfo[i];
					totalBytes += performanceInfo.TotalBytesTransferred;

					if (performanceInfo.StartTime < minStartTime)
						minStartTime = performanceInfo.StartTime;
					if (performanceInfo.EndTime > maxEndTime)
						maxEndTime = performanceInfo.EndTime;
				}

				long averageTicks = minStartTime.Ticks + (maxEndTime.Ticks - minStartTime.Ticks)/2;
				double timeSpanSeconds = maxEndTime.Subtract(minStartTime).TotalSeconds;
				double averageMbPerSecond = totalBytes / timeSpanSeconds / 1024 / 1024 * 8;
				timePoints.Add(new DateTime(averageTicks));
				retrieveMbPerSecond.Add(averageMbPerSecond);
			}
		}

    	private void _addSelectedStudies_Click(object sender, EventArgs e)
		{
			_component.AddSelectedStudies();
		}

		private void _clearRetrieve_Click(object sender, EventArgs e)
		{
			_component.ClearRetrieveItems();
		}

		private void _clearDecompress_Click(object sender, EventArgs e)
		{
			_component.ClearDecompressItems();
		}

		private void _refreshPlot_Click(object sender, EventArgs e)
		{
			RefreshRetrievePlot();
		}
    }
}
