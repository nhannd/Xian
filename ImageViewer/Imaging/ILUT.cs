#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IModalityLut : IComposableLut
	{
		new IModalityLut Clone();
	}

	[Cloneable(true)]
	public abstract class ComposableModalityLut : ComposableLut, IModalityLut
	{
		public new IModalityLut Clone()
		{
			return (IModalityLut) base.Clone();
		}
	}

	[Cloneable(true)]
	public abstract class GeneratedDataModalityLut : GeneratedDataLut, IModalityLut
	{
		public new IModalityLut Clone()
		{
			return (IModalityLut) base.Clone();
		}

		IComposableLut IComposableLut.Clone()
		{
			return Clone();
		}
	}

	[Cloneable]
	public abstract class SimpleDataModalityLut : SimpleDataLut, IModalityLut
	{
		protected SimpleDataModalityLut(int firstMappedPixelValue, int[] data, int minOutputValue, int maxOutputValue, string key, string description)
			: base(firstMappedPixelValue, data, minOutputValue, maxOutputValue, key, description) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected SimpleDataModalityLut(SimpleDataModalityLut source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new IModalityLut Clone()
		{
			return (IModalityLut) base.Clone();
		}

		IComposableLut IComposableLut.Clone()
		{
			return Clone();
		}
	}

	public interface IVoiLut : IComposableLut
	{
		new IVoiLut Clone();
	}

	[Cloneable(true)]
	public abstract class ComposableVoiLut : ComposableLut, IVoiLut
	{
		public new IVoiLut Clone()
		{
			return (IVoiLut) base.Clone();
		}
	}

	[Cloneable(true)]
	public abstract class DataVoiLut : DataLut, IVoiLut
	{
		public new IVoiLut Clone()
		{
			return (IVoiLut) base.Clone();
		}

		IComposableLut IComposableLut.Clone()
		{
			return Clone();
		}
	}
}