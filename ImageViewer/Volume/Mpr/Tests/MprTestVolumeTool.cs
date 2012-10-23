#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	[ExtensionOf(typeof (DesktopToolExtensionPoint), Enabled = false)]
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
					ResourceResolver rr = new ApplicationThemeResourceResolver(this.GetType().Assembly);
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

			protected virtual void OnBeforeExecute() {}
			protected virtual void OnAfterExecute() {}

			public void Execute()
			{
				this.OnBeforeExecute();
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
				this.OnAfterExecute();
			}
		}

		private class XAxialRotationGantryTiledTestVolume : TestVolume
		{
			private readonly float _tiltDegrees;
			private readonly DataSetOrientation _dataSetOrientation;

			public XAxialRotationGantryTiledTestVolume(VolumeFunction function, float tiltDegrees) : base(function)
			{
				_tiltDegrees = tiltDegrees;
				_dataSetOrientation = DataSetOrientation.CreateGantryTiltedAboutX(tiltDegrees);
			}

			public override string Name
			{
				get { return string.Format("{0} (Tilt: {1}\u00B0 about X)", base.Name, _tiltDegrees); }
			}

			protected override void OnBeforeExecute()
			{
				base.OnBeforeExecute();
				_dataSetOrientation.ResetImagePositionPatient();
			}

			protected override void InitializeSopDataSource(ISopDataSource sopDataSource)
			{
				base.InitializeSopDataSource(sopDataSource);
				_dataSetOrientation.Initialize(sopDataSource);
			}
		}
	}
}

#endif