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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	public partial class ImageViewerComponent
	{
		internal class SingleStudyLoader : IDisposable
		{
			private readonly ImageViewerComponent _viewer;
			private readonly LoadStudyArgs _args;
			private readonly List<Sop> _sops;
			private int _total;
			private int _failed;

			public SingleStudyLoader(ImageViewerComponent viewer, LoadStudyArgs args)
			{
				_viewer = viewer;
				_args = args;
				_sops = new List<Sop>();
				_total = 0;
				_failed = 0;
			}

			public int Total
			{
				get { return _total; }
			}

			public int Failed
			{
				get { return _failed; }
			}

			public void LoadStudy()
			{
				LoadSops();
				AddSops();
			}

			public void LoadSops()
			{
				//Use a new loader in case this call is asynchronous, we don't want to use the viewer's instance.
				IStudyLoader studyLoader = new StudyLoaderMap()[_args.StudyLoaderName];

				int total;
				try
				{
					total = studyLoader.Start(new StudyLoaderArgs(_args.StudyInstanceUid, _args.Server));
					if (total <= 0)
					{
						string message = String.Format("Study '{0}' does not appear to exist on server '{1}'",
						                               _args.StudyInstanceUid, _args.Server);

						throw new OpenStudyException(message);
					}
				}
				catch(OpenStudyException)
				{
					throw;
				}
				catch (Exception e)
				{
					string message = String.Format("Failed to load images for study '{0}'", _args.StudyInstanceUid);
					throw new OpenStudyException(message, e);
				}

				try
				{
					while (true)
					{
						Sop sop = studyLoader.LoadNextSop();
						if (sop == null)
							break;

						_sops.Add(sop);
					}
				}
				catch (Exception e)
				{
					string message = String.Format("Failed to load images for study '{0}'", _args.StudyInstanceUid);
					Platform.Log(LogLevel.Error, e, message);

					foreach (Sop sop in _sops)
					{
						try
						{
							sop.Dispose();
						}
						catch(Exception ex)
						{
							Platform.Log(LogLevel.Error, ex);
						}
					}

					_sops.Clear();

					OpenStudyException exception = new OpenStudyException(message, e);
					exception.TotalImages = total;
					exception.FailedImages = total;
					throw exception;
				}
			}

			public void AddSops()
			{
				if (_sops.Count == 0)
					return;

				List<Sop> sops = new List<Sop>(_sops);
				_sops.Clear();
				foreach (Sop sop in sops)
				{
					try
					{
						_viewer.StudyTree.AddSop(sop);
					}
					catch(SopValidationException e)
					{
						++_failed;
						Platform.Log(LogLevel.Error, e);
						sop.Dispose();
					}
					catch (Exception e)
					{
						++_failed;
						Platform.Log(LogLevel.Error, e);
					}

					++_total;
				}

				Study study = _viewer.StudyTree.GetStudy(_args.StudyInstanceUid);
				if (study != null)
					_viewer.EventBroker.OnStudyLoaded(new ItemEventArgs<Study>(study));

				VerifyLoad(Total, Failed);

				IPrefetchingStrategy prefetchingStrategy = _viewer.StudyLoaders[_args.StudyLoaderName].PrefetchingStrategy;
				if (prefetchingStrategy != null)
					prefetchingStrategy.Start(_viewer);
			}

			#region IDisposable Members

			public void Dispose()
			{
				try
				{
					foreach (Sop sop in _sops)
						sop.Dispose();

					_sops.Clear();

					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			#endregion
		}
	}
}