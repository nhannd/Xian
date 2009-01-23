using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface IFrameReference : IImageSopProvider, IDisposable
	{
		IFrameReference Clone();
	}

	public partial class Frame
	{
		private class FrameReference : IFrameReference
		{
			private readonly Frame _frame;
			private ISopReference _sopReference;

			public FrameReference(Frame frame)
			{
				_frame = frame;
				_sopReference = _frame.ParentImageSop.CreateTransientReference();
			}

			#region IFrameReference Members

			public IFrameReference Clone()
			{
				return new FrameReference(_frame);
			}

			#endregion

			#region IImageSopProvider Members

			public ImageSop ImageSop
			{
				get { return _frame.ParentImageSop; }
			}

			public Frame Frame
			{
				get { return _frame; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_sopReference != null)
				{
					_sopReference.Dispose();
					_sopReference = null;
				}
			}

			#endregion
		}
	}
}
