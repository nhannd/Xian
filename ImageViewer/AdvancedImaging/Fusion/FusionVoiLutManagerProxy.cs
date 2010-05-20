#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal class FusionVoiLutManagerProxy : IVoiLutManager, IDisposable
	{
		private FusionPresentationImageLayer _activeLayer;

		[CloneIgnore]
		private IVoiLutManager _baseVoiLutManager;

		[CloneIgnore]
		private IVoiLutManager _overlayVoiLutManager;

		[CloneIgnore]
		private IVoiLutManager _overlayVoiLutManagerPlaceholder;

		public FusionVoiLutManagerProxy()
		{
			_activeLayer = FusionPresentationImageLayer.Base;
			_baseVoiLutManager = null;
			_overlayVoiLutManager = null;
			_overlayVoiLutManagerPlaceholder = new VoiLutManager(new XVoiLutInstaller(), false);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected FusionVoiLutManagerProxy(FusionVoiLutManagerProxy source, ICloningContext context)
		{
			_activeLayer = source._activeLayer;
			_baseVoiLutManager = null;
			_overlayVoiLutManager = null;
			_overlayVoiLutManagerPlaceholder = new VoiLutManager(new XVoiLutInstaller()
			                                                     	{
			                                                     		Invert = source._overlayVoiLutManagerPlaceholder.Invert,
			                                                     		VoiLut = source._overlayVoiLutManagerPlaceholder.VoiLut
			                                                     	}, false);
		}

		public void Dispose()
		{
			_baseVoiLutManager = null;
			_overlayVoiLutManager = null;
			_overlayVoiLutManagerPlaceholder = null;
		}

		public FusionPresentationImageLayer ActiveLayer
		{
			get { return _activeLayer; }
			set { _activeLayer = value; }
		}

		public void SetBaseVoiLutManager(IVoiLutManager baseVoiLutManager)
		{
			_baseVoiLutManager = baseVoiLutManager;
		}

		public void SetOverlayVoiLutManager(IVoiLutManager overlayVoiLutManager)
		{
			if (_overlayVoiLutManager != null)
			{
				Replicate(_overlayVoiLutManager, _overlayVoiLutManagerPlaceholder);
			}

			_overlayVoiLutManager = overlayVoiLutManager;

			if (_overlayVoiLutManager != null)
			{
				Replicate(_overlayVoiLutManagerPlaceholder, _overlayVoiLutManager);
			}
		}

		private static void Replicate<T>(T source, T destination) where T : IMemorable
		{
			destination.SetMemento(source.CreateMemento());
		}

		#region IVoiLutManager Members

		[Obsolete("Use the VoiLut property instead.")]
		IComposableLut IVoiLutManager.GetLut()
		{
			if (_activeLayer == FusionPresentationImageLayer.Base)
			{
				return _baseVoiLutManager.GetLut();
			}
			else
			{
				if (_overlayVoiLutManager != null)
					return _overlayVoiLutManager.GetLut();
				else
					return _overlayVoiLutManagerPlaceholder.GetLut();
			}
		}

		[Obsolete("Use the InstallVoiLut method instead.")]
		void IVoiLutManager.InstallLut(IComposableLut voiLut)
		{
			if (_activeLayer == FusionPresentationImageLayer.Base)
			{
				_baseVoiLutManager.InstallLut(voiLut);
			}
			else
			{
				if (_overlayVoiLutManager != null)
					_overlayVoiLutManager.InstallLut(voiLut);
				else
					_overlayVoiLutManagerPlaceholder.InstallLut(voiLut);
			}
		}

		[Obsolete("Use the Invert property instead.")]
		void IVoiLutManager.ToggleInvert()
		{
			if (_activeLayer == FusionPresentationImageLayer.Base)
			{
				_baseVoiLutManager.ToggleInvert();
			}
			else
			{
				if (_overlayVoiLutManager != null)
					_overlayVoiLutManager.ToggleInvert();
				else
					_overlayVoiLutManagerPlaceholder.ToggleInvert();
			}
		}

		bool IVoiLutManager.Enabled
		{
			get
			{
				if (_activeLayer == FusionPresentationImageLayer.Base)
				{
					return _baseVoiLutManager.Enabled;
				}
				else
				{
					if (_overlayVoiLutManager != null)
						return _overlayVoiLutManager.Enabled;
					else
						return _overlayVoiLutManagerPlaceholder.Enabled;
				}
			}
			set
			{
				if (_activeLayer == FusionPresentationImageLayer.Base)
				{
					_baseVoiLutManager.Enabled = value;
				}
				else
				{
					if (_overlayVoiLutManager != null)
						_overlayVoiLutManager.Enabled = value;
					else
						_overlayVoiLutManagerPlaceholder.Enabled = value;
				}
			}
		}

		#endregion

		#region IVoiLutInstaller Members

		IComposableLut IVoiLutInstaller.VoiLut
		{
			get
			{
				if (_activeLayer == FusionPresentationImageLayer.Base)
				{
					return _baseVoiLutManager.VoiLut;
				}
				else
				{
					if (_overlayVoiLutManager != null)
						return _overlayVoiLutManager.VoiLut;
					else
						return _overlayVoiLutManagerPlaceholder.VoiLut;
				}
			}
		}

		void IVoiLutInstaller.InstallVoiLut(IComposableLut voiLut)
		{
			if (_activeLayer == FusionPresentationImageLayer.Base)
			{
				_baseVoiLutManager.InstallVoiLut(voiLut);
			}
			else
			{
				if (_overlayVoiLutManager != null)
					_overlayVoiLutManager.InstallVoiLut(voiLut);
				else
					_overlayVoiLutManagerPlaceholder.InstallVoiLut(voiLut);
			}
		}

		bool IVoiLutInstaller.Invert
		{
			get
			{
				if (_activeLayer == FusionPresentationImageLayer.Base)
				{
					return _baseVoiLutManager.Invert;
				}
				else
				{
					if (_overlayVoiLutManager != null)
						return _overlayVoiLutManager.Invert;
					else
						return _overlayVoiLutManagerPlaceholder.Invert;
				}
			}
			set
			{
				if (_activeLayer == FusionPresentationImageLayer.Base)
				{
					_baseVoiLutManager.Invert = value;
				}
				else
				{
					if (_overlayVoiLutManager != null)
						_overlayVoiLutManager.Invert = value;
					else
						_overlayVoiLutManagerPlaceholder.Invert = value;
				}
			}
		}

		#endregion

		#region IMemorable Members

		object IMemorable.CreateMemento()
		{
			if (_activeLayer == FusionPresentationImageLayer.Base)
			{
				return _baseVoiLutManager.CreateMemento();
			}
			else
			{
				if (_overlayVoiLutManager != null)
					return _overlayVoiLutManager.CreateMemento();
				else
					return _overlayVoiLutManagerPlaceholder.CreateMemento();
			}
		}

		void IMemorable.SetMemento(object memento)
		{
			if (_activeLayer == FusionPresentationImageLayer.Base)
			{
				_baseVoiLutManager.SetMemento(memento);
			}
			else
			{
				if (_overlayVoiLutManager != null)
					_overlayVoiLutManager.SetMemento(memento);
				else
					_overlayVoiLutManagerPlaceholder.SetMemento(memento);
			}
		}

		#endregion

		#region XVoiLutInstaller Class

		private class XVoiLutInstaller : IVoiLutInstaller
		{
			public bool Invert { get; set; }
			public IComposableLut VoiLut { get; set; }

			public XVoiLutInstaller()
			{
				this.Invert = false;
				this.VoiLut = new IdentityVoiLinearLut();
			}

			public void InstallVoiLut(IComposableLut voiLut)
			{
				this.VoiLut = voiLut;
			}
		}

		#endregion
	}
}