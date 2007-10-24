using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A simple way to implement an ImageOperation, using delegates.
	/// </summary>
	public class BasicImageOperation : ImageOperation
	{
		/// <summary>
		/// Defines a delegate used to get the originator for a given <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate IMemorable GetOriginatorDelegate(IPresentationImage image);

		/// <summary>
		/// Defines a delegate used to apply an undoable operation to an <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate void ApplyDelegate(IPresentationImage image);

		private readonly GetOriginatorDelegate _getOriginatorDelegate;
		private readonly ApplyDelegate _applyDelegate;

		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public BasicImageOperation(GetOriginatorDelegate getOriginatorDelegate, ApplyDelegate applyDelegate)
		{
			Platform.CheckForNullReference(getOriginatorDelegate, "getOriginatorDelegate");
			Platform.CheckForNullReference(applyDelegate, "applyDelegate");

			_getOriginatorDelegate = getOriginatorDelegate;
			_applyDelegate = applyDelegate;
		}

		/// <summary>
		/// Gets the originator for the input <see cref="IPresentationImage"/>, which must be <see cref="IMemorable"/>.
		/// </summary>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return _getOriginatorDelegate(image);
		}

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		public sealed override void Apply(IPresentationImage image)
		{
			_applyDelegate(image);
		}
	}
}
