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
			private readonly List<Sop> _sops;
			private readonly LoadStudyArgs _args;
			private readonly StudyItem _studyItem;

			public SingleStudyLoader(ImageViewerComponent viewer, LoadStudyArgs args)
			{
				_viewer = viewer;
				_args = args;
				_sops = new List<Sop>();
			}

			public SingleStudyLoader(ImageViewerComponent viewer, StudyItem studyItem)
			{
				_viewer = viewer;
				_studyItem = studyItem;
				_sops = new List<Sop>();
			}

			public StudyItem StudyItem
			{
				get { return _studyItem; }	
			}

			public string StudyLoaderName
			{
				get
				{
					if (_args != null)
						return _args.StudyLoaderName;
					else
						return _studyItem.StudyLoaderName;
				}	
			}

			public string StudyInstanceUid
			{
				get
				{
					if (_args != null)
						return _args.StudyInstanceUid;
					else
						return _studyItem.StudyInstanceUID;
				}
			}

			public object Server
			{
				get
				{
					if (_args != null)
						return _args.Server;
					else
						return _studyItem.Server;
				}
			}

			public void LoadStudy()
			{
				LoadSops();
				AddSops();
			}

			public void LoadSops()
			{
				//Use a new loader in case this call is asynchronous, we don't want to use the viewer's instance.
				IStudyLoader studyLoader = new StudyLoaderMap()[StudyLoaderName];

				StudyLoaderArgs args = new StudyLoaderArgs(StudyInstanceUid, Server);
				int total;

				try
				{
					total = studyLoader.Start(args);
					if (total <= 0)
						throw new NotFoundLoadStudyException(args.StudyInstanceUid);
				}
				catch (LoadStudyException)
				{
					throw;
				}
				catch (Exception e)
				{
					throw new LoadStudyException(args.StudyInstanceUid, e);
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

					throw new LoadStudyException(args.StudyInstanceUid, total, total, e);
				}
			}

			public void AddSops()
			{
				if (_sops.Count == 0)
					return;

				int total = 0;
				int failed = 0;

				//don't try to load something that's already there.
				Study study = _viewer.StudyTree.GetStudy(StudyInstanceUid);
				if (study != null)
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
						++failed;
						Platform.Log(LogLevel.Error, e);
						sop.Dispose();
					}
					catch (Exception e)
					{
						++failed;
						Platform.Log(LogLevel.Error, e);
					}

					++total;
				}

				study = _viewer.StudyTree.GetStudy(StudyInstanceUid);
				if (study != null)
					_viewer.EventBroker.OnStudyLoaded(new ItemEventArgs<Study>(study));

				if (failed > 0)
					throw new LoadStudyException(StudyInstanceUid, total, failed);

				IPrefetchingStrategy prefetchingStrategy = _viewer.StudyLoaders[StudyLoaderName].PrefetchingStrategy;
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