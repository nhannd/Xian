#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an arc graphic.
	/// </summary>
	public interface IArcGraphic : IBoundableGraphic
	{
		/// <summary>
		/// Gets or sets the angle at which the arc begins.
		/// </summary>
		/// <remarks>
		/// It is good practice to set the <see cref="StartAngle"/> before the <see cref="SweepAngle"/>
		/// because in the case where a graphic is scaled differently in x than in y, the conversion
		/// of the <see cref="SweepAngle"/> from <see cref="CoordinateSystem.Source"/> to
		/// <see cref="CoordinateSystem.Destination"/> coordinates is dependent upon the <see cref="StartAngle"/>.
		/// However, under normal circumstances, where the scale in x and y are the same, the <see cref="StartAngle"/>
		/// and <see cref="SweepAngle"/> can be set independently.
		/// </remarks>
		float StartAngle { get; set; }

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		/// <remarks>
		/// See <see cref="StartAngle"/> for information on setting the <see cref="SweepAngle"/>.
		/// </remarks>
		float SweepAngle { get; set; }
	}
}