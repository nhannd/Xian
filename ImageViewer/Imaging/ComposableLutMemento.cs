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

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ComposableLutMemento : IEquatable<ComposableLutMemento>
	{
		#region Private Fields

		private readonly IComposableLut _originatingLut;
		private readonly object _innerMemento;

		#endregion

		public ComposableLutMemento(IComposableLut originatingLut)
		{
			Platform.CheckForNullReference(originatingLut, "originatingLut");
			_originatingLut = originatingLut;
			_innerMemento = originatingLut.CreateMemento();
		}

		#region Public Members

		public IComposableLut OriginatingLut
		{
			get { return _originatingLut; }
		}

		public object InnerMemento
		{
			get { return _innerMemento; }	
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}	

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is ComposableLutMemento)
				return this.Equals((ComposableLutMemento) obj);

			return false;
		}

		#region IEquatable<LutMemento> Members

		public bool Equals(ComposableLutMemento other)
		{
			if (other == null)
				return false;

			if (this._innerMemento != null)
				return this._originatingLut.Equals(other._originatingLut) && this._innerMemento.Equals(other._innerMemento);
			
			return this._originatingLut.Equals(other._originatingLut);
		}

		#endregion
		#endregion
	}
}
