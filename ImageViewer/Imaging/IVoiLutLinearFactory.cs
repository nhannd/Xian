using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Implemented by object(s) that implicitly know the correct parameters to create appropriate linear Luts
	/// without the client having to provide them.
	/// </summary>
	public interface IVoiLutLinearFactory
	{
		/// <summary>
		/// Creates and returns an <see cref="IVOILUTLinear"/>.
		/// </summary>
		/// <returns>an <see cref="IVOILUTLinear"/></returns>
		IVOILUTLinear CreateLut();

		/// <summary>
		/// Creates and returns an <see cref="IStatefulVoiLutLinear"/> in the default state.
		/// </summary>
		/// <returns>an <see cref="IStatefulVoiLutLinear"/></returns>
		IStatefulVoiLutLinear CreateStatefulLut();

		/// <summary>
		/// Creates and returns an <see cref="IStatefulVoiLutLinear"/> with the provided initial state.
		/// </summary>
		/// <returns>an <see cref="IStatefulVoiLutLinear"/></returns>
		IStatefulVoiLutLinear CreateStatefulLut(IVoiLutLinearState initialState);
	}
}
