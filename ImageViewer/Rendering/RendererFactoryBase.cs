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

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Provides base implementation for a Render Factory.
	/// </summary>
	/// <remarks>
	/// Inheritors must also derive from <see cref="RendererBase"/> 
	/// in order to use this class (it calls <see cref="GetNewRenderer"/> to allocate
	/// a new <see cref="RendererBase"/> from within the <see cref="GetRenderer"/> factory
	/// method.  Note that only one <see cref="RendererBase"/> object is actually allocated
	/// per thread, and is wrapped in an internal proxy object.  This is because the 
	/// <see cref="RendererBase"/> object is purposely <b>not</b> thread-safe to make it
	/// easier for inheritors to implement.  However, there is nothing stopping a developer
	/// from deriving directly from <see cref="IRenderer"/> and creating their own Singleton
	/// thread-safe renderer.
	/// </remarks>
	public abstract class RendererFactoryBase : IRendererFactory
	{
		#region RendererProxy Class

		private class RendererProxy : IRenderer
		{
			private ReferenceCountedObjectWrapper<RendererBase> _wrapper;

			public RendererProxy(RendererBase realRenderer)
			{
				_wrapper = new ReferenceCountedObjectWrapper<RendererBase>(realRenderer);
			}

			public ReferenceCountedObjectWrapper<RendererBase> Wrapper
			{
				get { return _wrapper; }	
			}

			#region IRenderer Members

			public void Draw(DrawArgs drawArgs)
			{
				Wrapper.Item.Draw(drawArgs);
			}

			public IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
			{
				return Wrapper.Item.GetRenderingSurface(windowID, width, height);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					if (_wrapper == null)
						return;
					
					_wrapper.DecrementReferenceCount();
					if (_wrapper.IsReferenceCountAboveZero())
						return;

					_proxy = null;

					_wrapper.Item.Dispose();
					_wrapper = null;

					GC.SuppressFinalize(this);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}

		#endregion

		[ThreadStatic]private static RendererProxy _proxy;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected RendererFactoryBase()
		{
		}

		/// <summary>
		/// Allocates a new <see cref="RendererBase"/>.
		/// </summary>
		/// <remarks>
		/// Inheritors must override this method.
		/// </remarks>
		protected abstract RendererBase GetNewRenderer();

		#region IRendererFactory Members

		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// Factory method for <see cref="IRenderer"/>s.
		/// </summary>
		/// <remarks>
		/// See the remarks for <see cref="RendererFactoryBase"/> regarding how
		/// these objects are actually allocated/managed internally.
		/// </remarks>
		public IRenderer GetRenderer()
		{
			if (_proxy == null)
				_proxy = new RendererProxy(GetNewRenderer());

			_proxy.Wrapper.IncrementReferenceCount();
			return _proxy;
		}

		#endregion
	}
}
