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
using System.Diagnostics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	public abstract class AbstractMprTest
	{
		protected delegate void InitializeSopDataSourceDelegate(ISopDataSource sopDataSource);

		protected delegate void TestVolumeDelegate(Volume volume);

		protected static void TestVolume(VolumeFunction function, InitializeSopDataSourceDelegate initializer, TestVolumeDelegate testMethod)
		{
			function = function.Normalize(100);
			List<ImageSop> images = new List<ImageSop>();
			try
			{
				foreach (ISopDataSource sopDataSource in function.CreateSops(100, 100, 100, false))
				{
					if (initializer != null)
						initializer.Invoke(sopDataSource);
					images.Add(ImageSop.Create(sopDataSource));
				}

				using (Volume volume = Volume.Create(EnumerateFrames(images)))
				{
					if (testMethod != null)
						testMethod.Invoke(volume);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("Thrown: {0}", ex.GetType().Name));
				throw;
			}
			finally
			{
				DisposeAll(images);
			}
		}

		protected static IEnumerable<Frame> EnumerateFrames(IEnumerable<ImageSop> imageSops)
		{
			foreach (ImageSop imageSop in imageSops)
				foreach (Frame frame in imageSop.Frames)
					yield return frame;
		}

		protected static void DisposeAll<T>(IEnumerable<T> disposables) where T : class, IDisposable
		{
			foreach (T disposable in disposables)
				if (disposable != null)
					disposable.Dispose();
		}

		private static IList<KnownSample> _planetsKnownSamples;

		protected static IList<KnownSample> PlanetsKnownSamples
		{
			get
			{
				if (_planetsKnownSamples == null)
				{
					List<KnownSample> samplePoints = new List<KnownSample>();
					samplePoints.Add(new KnownSample(new Vector3D(15, 15, 15), (int) (65535/3f))); // The sphere at (15,15,15) is coloured 1/3 of full scale
					samplePoints.Add(new KnownSample(new Vector3D(75, 25, 50), (int) (65535*2/3f))); // The sphere at (75,25,50) is coloured 2/3 of full scale
					samplePoints.Add(new KnownSample(new Vector3D(15, 85, 15), 65535)); // The sphere at (15,85,15) is coloured 3/3 of full scale
					samplePoints.Add(new KnownSample(new Vector3D(50, 50, 50), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(75, 75, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 25, 25), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 50, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(15, 50, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 15, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 50, 15), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(25, 85, 75), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(85, 50, 15), 0)); // anything else should be 0
					samplePoints.Add(new KnownSample(new Vector3D(15, 50, 85), 0)); // anything else should be 0
					_planetsKnownSamples = samplePoints.AsReadOnly();
				}
				return _planetsKnownSamples;
			}
		}

		protected struct KnownSample
		{
			public readonly Vector3D Point;
			public readonly int Value;

			public KnownSample(Vector3D point, int value)
			{
				this.Point = point;
				this.Value = value;
			}
		}
	}
}

#endif