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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for views onto <see cref="BlockingOperation"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class BlockingOperationViewExtensionPoint : ExtensionPoint<IBlockingOperationView>
	{
	}

	/// <summary>
	/// The BlockingOperation class is a static class that allows application level code to
	/// use a wait cursor without having to explicitly reference a particular Gui Toolkit's API.
	/// </summary>
	public static class BlockingOperation
	{
		/// <summary>
		/// Executes the provided operation in the view, showing a wait cursor for the duration of the call.
		/// </summary>
		/// <param name="operation">The operation to execute in the view layer.</param>
		public static void Run(BlockingOperationDelegate operation)
		{
			Platform.CheckForNullReference(operation, "operation");

			IBlockingOperationView operationView = null;

			try
			{
				operationView = (IBlockingOperationView)ViewFactory.CreateView(new BlockingOperationViewExtensionPoint());
			}
			catch (Exception e)
			{
                Platform.Log(LogLevel.Error, e);
			}

			if (operationView == null)
			{
				operation();
			}
			else
			{
				operationView.Run(operation);
			}
		}
	}
}
