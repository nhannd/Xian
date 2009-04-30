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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Declares an observer binding for the Label value of an action.
	/// </summary>
	/// <remarks>
	/// This attribute causes the Label value of the action specified by the action ID to be
	/// bound to the state of the specified property on the class to which this attribute applies.
	/// The property name must refer to a public string property on the target class that has get access.
	/// The change event name must refer to a public event on the class that will fire whenever the value of the property
	/// changes.
	/// </remarks>
	public class LabelValueObserverAttribute : StateObserverAttribute
	{
		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="actionID">The logical action identifier to which this attribute applies.</param>
		/// <param name="propertyName">The name of the property to bind to.</param>
		/// <param name="changeEventName">The name of the property change notification event to bind to.</param>
		public LabelValueObserverAttribute(string actionID, string propertyName, string changeEventName)
			: base(actionID, propertyName, changeEventName)
		{
		}

		/// <summary>
		/// Binds the <see cref="IAction.Label"/> property and <see cref="IAction.LabelChanged"/> event 
		/// to the corresponding items on the target object, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
        public override void Apply(IActionBuildingContext builder)
		{
            Bind<string>(builder, "Label", "LabelChanged");
        }
	}

}
