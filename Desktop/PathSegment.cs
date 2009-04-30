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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single segment of a <see cref="Path"/>.
    /// </summary>
    public class PathSegment : IEquatable<PathSegment>
    {
        private readonly string _key;
        private readonly string _localized;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="key">The resource key or unlocalized path segment string.</param>
        /// <param name="localized">The localized path segment string.</param>
        internal PathSegment(string key, string localized)
        {
            _key = key;
            _localized = localized;
        }

        /// <summary>
        /// Gets the resource key or unlocalized text.
        /// </summary>
        public string ResourceKey
        {
            get { return _key; }
        }

        /// <summary>
		/// Gets the localized text.
        /// </summary>
        public string LocalizedText
        {
            get { return _localized; }
        }


    	///<summary>
    	///</summary>
    	///<param name="pathSegment1"></param>
    	///<param name="pathSegment2"></param>
    	///<returns></returns>
    	public static bool operator !=(PathSegment pathSegment1, PathSegment pathSegment2)
    	{
    		return !Equals(pathSegment1, pathSegment2);
    	}

    	///<summary>
    	///</summary>
    	///<param name="pathSegment1"></param>
    	///<param name="pathSegment2"></param>
    	///<returns></returns>
    	public static bool operator ==(PathSegment pathSegment1, PathSegment pathSegment2)
    	{
    		return Equals(pathSegment1, pathSegment2);
    	}

    	/// <summary>
    	/// Gets whether or not <paramref name="pathSegment"/> is equal to this object.
    	/// </summary>
		public bool Equals(PathSegment pathSegment)
    	{
    		if (pathSegment == null) return false;
    		return Equals(_localized, pathSegment._localized);
    	}

		/// <summary>
		/// Gets whether or not <paramref name="obj"/> is equal to this object.
		/// </summary>
		public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as PathSegment);
    	}

		/// <summary>
		/// Gets a hash code.
		/// </summary>
    	public override int GetHashCode()
    	{
    		return _localized != null ? _localized.GetHashCode() : 0;
    	}
    }
}
