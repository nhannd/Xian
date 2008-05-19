#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Exception that is thrown when value conversion fails.
    /// </summary>
    public class TypeConversionException:Exception
    {
        public TypeConversionException(string message)
            :base(message)
        {
        }
    }

    /// <summary>
    /// Defines the interface of a function.
    /// </summary>
    /// <remarks>
    /// A function returns a value based on an input. An example usage of function 
    /// is one that retrieves the value of a dicom tag (output) in a message (input).
    /// </remarks>
    /// <typeparam name="TInput">The type of input to the function</typeparam>
    public interface IFunction<TInput>
    {
        /// <summary>
        /// Gets the value of the function, converted into the specified input.
        /// </summary>
        /// <typeparam name="T">Expected returned type</typeparam>
        /// <param name="input">Input to the function</param>
        /// <param name="defaultValue">Default value when the function evaluation fails</param>
        /// <returns></returns>
        T GetValue<T>(TInput input, T defaultValue);

        /// <summary>
        /// Gets the value of the function, converted into the specified input.
        /// </summary>
        /// <typeparam name="T">Expected returned type</typeparam>
        /// <param name="input">Input to the function</param>
        /// <returns></returns>
        /// <exception>
        /// <see cref="TypeConversionException"/> thrown when the function cannot return the value of the specified type.
        /// </exception>
        T GetValue<T>(TInput input);
    }
}