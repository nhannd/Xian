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
	internal class VoiLutManagerProxy : IVoiLutManager
	{
		[CloneIgnore]
		private readonly IVoiLutManager _placeholderVoiLutManager;

		[CloneIgnore]
		private IVoiLutManager _realVoiLutManager;

		public VoiLutManagerProxy()
		{
			_placeholderVoiLutManager = new VoiLutManager(new XVoiLutInstaller(), false);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected VoiLutManagerProxy(VoiLutManagerProxy source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_placeholderVoiLutManager = new VoiLutManager(new XVoiLutInstaller(source._realVoiLutManager ?? source._placeholderVoiLutManager), false);
		}

		public void SetRealVoiLutManager(IVoiLutManager realVoiLutManager)
		{
			if (_realVoiLutManager != null)
			{
				Replicate(_realVoiLutManager, _placeholderVoiLutManager);
			}

			_realVoiLutManager = realVoiLutManager;

			if (_realVoiLutManager != null)
			{
				Replicate(_placeholderVoiLutManager, _realVoiLutManager);
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
			if (_realVoiLutManager != null)
				return _realVoiLutManager.GetLut();
			else
				return _placeholderVoiLutManager.GetLut();
		}

		[Obsolete("Use the InstallVoiLut method instead.")]
		void IVoiLutManager.InstallLut(IComposableLut voiLut)
		{
			if (_realVoiLutManager != null)
				_realVoiLutManager.InstallLut(voiLut);
			else
				_placeholderVoiLutManager.InstallLut(voiLut);
		}

		[Obsolete("Use the Invert property instead.")]
		void IVoiLutManager.ToggleInvert() {}

		bool IVoiLutManager.Enabled
		{
			get
			{
				if (_realVoiLutManager != null)
					return _realVoiLutManager.Enabled;
				else
					return _placeholderVoiLutManager.Enabled;
			}
			set
			{
				if (_realVoiLutManager != null)
					_realVoiLutManager.Enabled = value;
				else
					_placeholderVoiLutManager.Enabled = value;
			}
		}

		#endregion

		#region IVoiLutInstaller Members

		IComposableLut IVoiLutInstaller.VoiLut
		{
			get
			{
				if (_realVoiLutManager != null)
					return _realVoiLutManager.VoiLut;
				else
					return _placeholderVoiLutManager.VoiLut;
			}
		}

		void IVoiLutInstaller.InstallVoiLut(IComposableLut voiLut)
		{
			if (_realVoiLutManager != null)
				_realVoiLutManager.InstallVoiLut(voiLut);
			else
				_placeholderVoiLutManager.InstallVoiLut(voiLut);
		}

		bool IVoiLutInstaller.Invert
		{
			get { return false; }
			set { }
		}

		#endregion

		#region IMemorable Members

		object IMemorable.CreateMemento()
		{
			if (_realVoiLutManager != null)
				return _realVoiLutManager.CreateMemento();
			else
				return _placeholderVoiLutManager.CreateMemento();
		}

		void IMemorable.SetMemento(object memento)
		{
			if (_realVoiLutManager != null)
			{
				_realVoiLutManager.SetMemento(memento);
				_realVoiLutManager.Invert = false;
			}
			else
			{
				_placeholderVoiLutManager.SetMemento(memento);
				_placeholderVoiLutManager.Invert = false;
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
				this.VoiLut = new BasicVoiLutLinear(32768, 16384);
			}

			public XVoiLutInstaller(IVoiLutInstaller source)
			{
				this.Invert = source.Invert;
				this.VoiLut = source.VoiLut.Clone();
			}

			public void InstallVoiLut(IComposableLut voiLut)
			{
				this.VoiLut = voiLut;
			}
		}

		#endregion
	}
}