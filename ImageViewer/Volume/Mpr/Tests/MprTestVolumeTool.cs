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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class MprTestVolumeTool : Tool<IDesktopToolContext>
	{
		private IList<TestVolume> _volumes;
		private IActionSet _actions;

		public override void Initialize()
		{
			base.Initialize();

			List<TestVolume> volumes = new List<TestVolume>();
			volumes.Add(new TestVolume(VolumeFunction.Shells));
			volumes.Add(new TestVolume(VolumeFunction.Stripes));
			volumes.Add(new TestVolume(VolumeFunction.Barcodes));
			volumes.Add(new TestVolume(VolumeFunction.Duel));
			volumes.Add(new TestVolume(VolumeFunction.Rebar));
			volumes.Add(new TestVolume(VolumeFunction.Stars));
			volumes.Add(new XAxialRotationGantryTiledTestVolume(VolumeFunction.Barcodes, 30));
			volumes.Add(new XAxialRotationGantryTiledTestVolume(VolumeFunction.StarsTilted030X, 30));
			volumes.Add(new XAxialRotationGantryTiledTestVolume(VolumeFunction.StarsTilted345X, -15));
			_volumes = volumes.AsReadOnly();
		}

		public override IActionSet Actions
		{
			get
			{
				if (_actions == null)
				{
					ResourceResolver rr = new ResourceResolver(this.GetType().Assembly);
					List<IAction> actions = new List<IAction>();
					int n = 0;
					foreach (TestVolume tv in _volumes)
					{
						int id = n++;
						MenuAction action = new MenuAction(string.Format("{0}:action{1}", this.GetType().FullName, id),
						                                   new ActionPath("global-menus/MenuDebug/MenuMpr/action" + id, rr), ClickActionFlags.None, rr);
						action.SetClickHandler(tv.Execute);
						action.Label = tv.Name;
						actions.Add(action);
					}
					_actions = new ActionSet(actions);
				}
				return _actions;
			}
		}

		private class TestVolume : AbstractMprTest
		{
			private readonly VolumeFunction _function;

			public TestVolume(VolumeFunction function)
			{
				_function = function;
			}

			public virtual string Name
			{
				get { return string.Format("Load MPR Test Volume {0}", _function.Name); }
			}

			protected virtual void InitializeSopDataSource(ISopDataSource sopDataSource) {}

			public void Execute()
			{
				List<ImageSop> images = new List<ImageSop>();
				try
				{
					VolumeFunction function = _function.Normalize(100);
					foreach (ISopDataSource sopDataSource in function.CreateSops(100, 100, 100, false))
					{
						this.InitializeSopDataSource(sopDataSource);
						images.Add(new ImageSop(sopDataSource));
					}

					MprViewerComponent component = new MprViewerComponent(Volume.Create(EnumerateFrames(images)));
					component.Layout();
					LaunchImageViewerArgs args = new LaunchImageViewerArgs(WindowBehaviour.Auto);
					args.Title = component.Title;
					MprViewerComponent.Launch(component, args);
				}
				catch (Exception ex)
				{
					ExceptionHandler.Report(ex, Application.ActiveDesktopWindow);
				}
				finally
				{
					DisposeAll(images);
				}
			}
		}

		private class XAxialRotationGantryTiledTestVolume : TestVolume
		{
			private readonly float _tiltDegrees;

			public XAxialRotationGantryTiledTestVolume(VolumeFunction function, float tiltDegrees) : base(function)
			{
				_tiltDegrees = tiltDegrees;
			}

			public override string Name
			{
				get { return string.Format("{0} (Tilt: {1}\u00B0 about X)", base.Name, _tiltDegrees); }
			}

			protected override void InitializeSopDataSource(ISopDataSource sopDataSource)
			{
				base.InitializeSopDataSource(sopDataSource);

				sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(ConvertXAxialGantryTiltToImageOrientationPatient(_tiltDegrees, true));
			}
		}
	}
}

#endif