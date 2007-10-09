using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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

		private class RendererProxy : IRenderer, IReferenceCountable
		{
			private RendererBase _realRenderer;
			private int _referenceCount;

			public RendererProxy(RendererBase realRenderer)
			{
				_realRenderer = realRenderer;
				_referenceCount = 0;
			}

			~RendererProxy()
			{
				Dispose(false);
			}

			#region IReferenceCountable Members

			public void IncrementReferenceCount()
			{
				++_referenceCount;
			}

			public void DecrementReferenceCount()
			{
				--_referenceCount;
			}

			public bool IsReferenceCountZero
			{
				get { return 0 == _referenceCount; }	
			}

			public int ReferenceCount
			{
				get { return _referenceCount; }
			}

			#endregion

			#region IRenderer Members

			public void Draw(DrawArgs drawArgs)
			{
				_realRenderer.Draw(drawArgs);
			}

			public IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
			{
				return _realRenderer.GetRenderingSurface(windowID, width, height);
			}

			#endregion

			#region Disposal

			private void Dispose(bool disposing)
			{
				if (_realRenderer != null)
				{
					_realRenderer.Dispose();
					_realRenderer = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					DecrementReferenceCount();
					if (ReferenceCount > 0)
						return;

					_proxy = null;
					
					Dispose(true);
					GC.SuppressFinalize(this);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
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

			_proxy.IncrementReferenceCount();
			return _proxy;
		}

		#endregion
	}
}
