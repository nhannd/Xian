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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;
using System.IO;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal partial class StreamingSopDataSource : DicomMessageSopDataSource
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly string _wadoUriPrefix;
		private readonly int _wadoServicePort;
		private volatile bool _fullHeaderRetrieved = false;

		public StreamingSopDataSource(InstanceXml instanceXml, string host, string aeTitle, string wadoUriPrefix, int wadoServicePort)
			: base(new DicomFile("", new DicomAttributeCollection(), instanceXml.Collection))
		{
			//These don't get set properly for instance xml.
			DicomFile sourceFile = (DicomFile)SourceMessage;
			sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
			sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;
			sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0, instanceXml.SopClass.Uid);

			_host = host;
			_aeTitle = aeTitle;
			_wadoUriPrefix = wadoUriPrefix;
			_wadoServicePort = wadoServicePort;
		}

		private InstanceXmlDicomAttributeCollection AttributeCollection
		{
			get { return (InstanceXmlDicomAttributeCollection)SourceMessage.DataSet; }
		}

		public override DicomAttribute GetDicomAttribute(DicomTag tag)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag.TagValue))
					GetFullHeader();

				return base.GetDicomAttribute(tag);
			}
		}

		public override DicomAttribute GetDicomAttribute(uint tag)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.GetDicomAttribute(tag);
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag.TagValue))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new StreamingSopFrameData(frameNumber, this);
		}

		private bool NeedFullHeader(uint tag)
		{
			if (_fullHeaderRetrieved)
				return false;

			if (CollectionUtils.Contains(AttributeCollection.ExcludedTags, 
				delegate(DicomTag dicomTag) { return dicomTag.TagValue == tag; }))
			{
				return true;
			}

			DicomAttribute attribute = base.GetDicomAttribute(tag);
			if (attribute is DicomAttributeSQ)
			{
				DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem) item).HasExcludedTags(true))
								return true;
						}
					}
				}
			}

			return false;
		}

		private void GetFullHeader()
		{
			if (!_fullHeaderRetrieved)
			{
				try
				{
					Uri uri = new Uri(String.Format(StreamingSettings.Default.FormatWadoUriPrefix, _host, _wadoServicePort));
					StreamingClient client = new StreamingClient(uri);

					DicomFile imageHeader = new DicomFile();
					using (Stream imageHeaderStream = client.RetrieveImageHeader(_aeTitle, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid))
					{
						imageHeader.Load(imageHeaderStream);
						base.SourceMessage = imageHeader;
						_fullHeaderRetrieved = true;
					}
				}
				catch(StreamingClientException ex)
				{
					switch(ex.Type)
					{
						case StreamingClientExceptionType.Access:
							throw new Exception(SR.MessageStreamingAccessException, ex);
						case StreamingClientExceptionType.Network:
							throw new Exception(SR.MessageStreamingNetworkException, ex);
						case StreamingClientExceptionType.Protocol:
						case StreamingClientExceptionType.Server:
						case StreamingClientExceptionType.UnexpectedResponse:
						case StreamingClientExceptionType.Generic:
						default:
							throw new Exception(SR.MessageStreamingGenericException, ex);
					}
				}
				catch(FormatException ex)
				{
					// this exception happens if the FormatWadoUriPrefix setting is invalid.
					throw new Exception(SR.MessageStreamingClientConfigurationException, ex);
				}
				catch(Exception ex)
				{
					throw new Exception(SR.MessageStreamingGenericException, ex);
				}
			}
		}
	}
}
