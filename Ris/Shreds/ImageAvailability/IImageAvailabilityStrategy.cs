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

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	/// <summary>
	/// Defines an interface to a strategy for determining the Image Availability of a Procedure.
	/// </summary>
	public interface IImageAvailabilityStrategy
	{
		/// <summary>
		/// Computes the <see cref="Healthcare.ImageAvailability"/> for a given <see cref="Procedure"/>.
		/// </summary>
		/// <remarks>
		/// This method must compute and return the image availability for the specified procedure.  The persistence-context
		/// is provided in case it is necessary to query parts of the model that are not reachable from the procedure,
		/// however, the model should not be updated (this method should be free of side-effects).
		/// </remarks>
		/// <param name="procedure"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		Healthcare.ImageAvailability ComputeProcedureImageAvailability(Procedure procedure, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class ImageAvailabilityStrategyExtensionPoint : ExtensionPoint<IImageAvailabilityStrategy>
	{
	}
}
