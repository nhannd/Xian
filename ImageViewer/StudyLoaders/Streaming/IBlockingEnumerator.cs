using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	//TODO: work this into common, so that all thread pools aren't tied to queues that can't be changed.
	internal interface IBlockingEnumerator<T> : IEnumerable<T>
	{
		bool ContinueBlocking { get; set; }
	}
}
