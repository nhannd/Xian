using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public interface IDiagnosticLayoutManager : ILayoutManager
	{
		void AddStudy(string studyInstanceUID);
		void AddSeries(string seriesInstanceUID);
		void AddImage(string sopInstanceUID);
	}
}
