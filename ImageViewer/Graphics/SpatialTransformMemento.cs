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

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class SpatialTransformMemento : IEquatable<SpatialTransformMemento>
	{
		private float _scale;
		private float _translationX;
		private float _translationY;
		private int _rotationXY;
		private bool _flipX;
		private bool _flipY;

		public SpatialTransformMemento()
		{
		}

		public float Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		public float TranslationX
		{
			get { return _translationX; }
			set { _translationX = value; }
		}

		public float TranslationY
		{
			get { return _translationY; }
			set { _translationY = value; }
		}

		public bool FlipX
		{
			get { return _flipX; }
			set { _flipX = value; }
		}

		public bool FlipY
		{
			get { return _flipY; }
			set { _flipY = value; }
		}

		public int RotationXY
		{
			get { return _rotationXY; }
			set { _rotationXY = value; }
		}

		public override int GetHashCode()
		{
			// Normally, you would calculate a hash code dependent on immutable
			// member fields since we've given this object value type semantics
			// because of how we've overridden Equals().  However, this is a memento
			// and thus the actualy contents of the memento are irrelevant if they 
			// were ever put in a hashtable.  We are in fact interested in the instance.
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as SpatialTransformMemento);
		}

		#region IEquatable<SpatialTransformMemento> Members

		public bool Equals(SpatialTransformMemento other)
		{
			if (other == null)
				return false;

			return (this.Scale == other.Scale &&
					this.TranslationX == other.TranslationX &&
					this.TranslationY == other.TranslationY &&
					this.FlipY == other.FlipY &&
					this.FlipX == other.FlipX &&
					this.RotationXY == other.RotationXY);
		}

		#endregion
	}
}
