using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	partial class WorkItem
	{
		public WorkItemRequest Request
		{
			get
			{
				return Deserialize<WorkItemRequest>(this.SerializedRequest);
			}
		}

		private T Deserialize<T>(object value)
		{
			// need to figure out exactly which serialization format we're using
			throw new NotImplementedException();
		}
	}
}
