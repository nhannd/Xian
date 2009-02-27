using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	[Cloneable]
	public class RoiCalloutGraphic : CalloutGraphic
	{
		[CloneIgnore]
		private readonly List<IRoiAnalyzer> _roiAnalyzers = new List<IRoiAnalyzer>();

		public RoiCalloutGraphic() : base()
		{
			_roiAnalyzers.AddRange(RoiAnalyzerExtensionPoint.RoiAnalyzers);
		}

		protected RoiCalloutGraphic(RoiCalloutGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
			_roiAnalyzers.AddRange(source._roiAnalyzers);
		}

		public IList<IRoiAnalyzer> RoiAnalyzers
		{
			get { return _roiAnalyzers; }
		}

		public void Update(Roi roi)
		{
			this.Update(roi, RoiAnalysisMode.Normal);
		}

		public void Update(Roi roi, RoiAnalysisMode mode)
		{
			string text = null;

			if (_roiAnalyzers.Count > 0)
			{
				StringBuilder builder = new StringBuilder();

				try
				{
					foreach (IRoiAnalyzer analyzer in _roiAnalyzers)
					{
						if (analyzer.SupportsRoi(roi))
						{
							string analysis = analyzer.Analyze(roi, mode);
							if (!string.IsNullOrEmpty(analysis))
								builder.AppendLine(analysis);
						}
					}

					text = builder.ToString();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					text = SR.MessageRoiAnalysisError;
				}
			}

			base.Text = text ?? SR.StringNoValue;
		}
	}
}