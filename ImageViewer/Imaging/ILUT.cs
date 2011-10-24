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
	}

	[Cloneable(true)]
	public abstract class SimpleDataModalityLut : SimpleDataLut, IModalityLut
	{
		protected SimpleDataModalityLut(int firstMappedPixelValue, int[] data, int minOutputValue, int maxOutputValue, string key, string description)
			: base(firstMappedPixelValue, data, minOutputValue, maxOutputValue, key, description) {}

		protected SimpleDataModalityLut(SimpleDataLut source, ICloningContext context)
			: base(source, context) {}

		public new IModalityLut Clone()
		{
			return (IModalityLut) base.Clone();
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
	}
}