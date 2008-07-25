using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class NoOpPrefetchingStrategy : IPrefetchingStrategy
	{
		public string Name
		{
			get { return SR.PrefetchingStrategyNameNoOp; }
		}

		public string Description
		{
			get { return SR.PrefetchingStrategyDescriptionNoOp; }
		}

		public void Start(IImageViewer imageViewer)
		{
			
		}

		public void Stop()
		{

		}
	}
}
