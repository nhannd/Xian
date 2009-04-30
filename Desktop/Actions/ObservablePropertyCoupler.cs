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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
    /// Couples two instances of <see cref="IObservablePropertyBinding{T}"/> such that a change to the primary property
    /// will be propagated to the secondary property.  
    /// </summary>
    /// <remarks>
	/// A bi-directional mode is also possible, where changes to the secondary
	/// property are also propagated back to the primary property.  In this case, infinite mutual
	/// recursion is prevented by only propagating the change if the values are actually different.
	/// To remove the coupling, call <see cref="Dispose()"/> on this object.
	/// </remarks>
    /// <typeparam name="T">The type of the bound property.</typeparam>
	internal class ObservablePropertyCoupler<T> : IDisposable
    {
        private IObservablePropertyBinding<T> _primary;
        private IObservablePropertyBinding<T> _secondary;
        private bool _bidirectional;

        /// <summary>
        /// Establishes the coupling between the specified primary and secondary properties.
        /// </summary>
        /// <remarks>
		/// The value of the secondary property will be initialized to the value of the primary property, 
		/// and will continue to be synchronized for the duration of the lifetime of this object.  
		/// To remove the coupling at any point in the future, retain the returned object, and 
		/// call Dispose() on it to remove the coupling.
		/// </remarks>
        /// <param name="primary">The primary property, which serves as the subject.</param>
        /// <param name="secondary">The secondary property, which observes the subject and tracks its value.</param>
        /// <returns>A property coupler object, which can optionally be retained for eventual disposal.</returns>
        public static ObservablePropertyCoupler<T> Couple(IObservablePropertyBinding<T> primary, IObservablePropertyBinding<T> secondary)
        {
            return new ObservablePropertyCoupler<T>(primary, secondary, false);
        }

        /// <summary>
        /// Establishes the coupling between the specified primary and secondary properties.
        /// </summary>
        /// <remarks>
		/// The value of the secondary property will be initialized to the value of the primary property, 
		/// and will continue to be synchronized for the duration of the lifetime of this object.  The coupling is 
		/// optionally bi-directional, in which case changes made to the secondary property are also propagated 
		/// back to the primary property.  To remove the coupling at any point in the future, retain the returned 
		/// object, and call Dispose() on it to remove the coupling.
		/// </remarks>
        /// <param name="primary">The primary property, which serves as the subject.</param>
        /// <param name="secondary">The secondary property, which observes the subject and tracks its value.</param>
        /// <param name="bidirectional">If true, the primary property will also track the value of the secondary property.</param>
        /// <returns>A property coupler object, which can optionally be retained for eventual disposal.</returns>
        public static ObservablePropertyCoupler<T> Couple(IObservablePropertyBinding<T> primary, IObservablePropertyBinding<T> secondary, bool bidirectional)
        {
            return new ObservablePropertyCoupler<T>(primary, secondary, bidirectional);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="primary">The primary property, which serves as the subject.</param>
        /// <param name="secondary">The secondary property, which observes the subject and tracks its value.</param>
        /// <param name="bidirectional">If true, the primary property will also track the value of the secondary property.</param>
        private ObservablePropertyCoupler(IObservablePropertyBinding<T> primary, IObservablePropertyBinding<T> secondary, bool bidirectional)
        {
            Platform.CheckForNullReference(primary, "primary");
            Platform.CheckForNullReference(secondary, "secondary");

            _primary = primary;
            _secondary = secondary;
            _bidirectional = bidirectional;

            // set up the coupling
            _primary.PropertyChanged += PrimaryChangedEventHandler;
            if (_bidirectional)
            {
                _secondary.PropertyChanged += SecondaryChangedEventHandler;
            }

            // initialize secondary value from primary
            _secondary.PropertyValue = _primary.PropertyValue;
        }

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			_primary.PropertyChanged -= PrimaryChangedEventHandler;
			if (_bidirectional)
			{
				_secondary.PropertyChanged -= SecondaryChangedEventHandler;
			}

			GC.SuppressFinalize(this);
		}

		#endregion

        private void PrimaryChangedEventHandler(object sender, EventArgs args)
        {
            AssignValue(_secondary, _primary);
        }

        private void SecondaryChangedEventHandler(object sender, EventArgs args)
        {
            AssignValue(_primary, _secondary);
        }

        private void AssignValue(IObservablePropertyBinding<T> dst, IObservablePropertyBinding<T> src)
        {
            // only do the assignment if the value is actually different
            // otherwise we may get infinite recursion in the bi-directional case
            if (!dst.PropertyValue.Equals(src.PropertyValue))
            {
                dst.PropertyValue = src.PropertyValue;
            }
        }
    }
}
