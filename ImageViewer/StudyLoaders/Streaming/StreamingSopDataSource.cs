#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public interface IStreamingSopDataSource : IDicomMessageSopDataSource
	{
		new IStreamingSopFrameData GetFrameData(FrameInfo frameInfo);
	}

	public partial class StreamingSopDataSource : DicomMessageSopDataSource, IStreamingSopDataSource
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly string _wadoUriPrefix;
		private readonly int _wadoServicePort;
		private volatile bool _fullHeaderRetrieved = false;
        private InstanceXml _instanceXml;

		public StreamingSopDataSource(InstanceXml instanceXml, string host, string aeTitle, string wadoUriPrefix, int wadoServicePort)
			: base(new DicomFile("", new DicomAttributeCollection(), instanceXml))
		{
		    _instanceXml = instanceXml;

			//These don't get set properly for instance xml.
			var sourceFile = (DicomFile)SourceMessage;
			sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
			sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;

		    sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0,
		                                                         instanceXml.SopClass == null
		                                                             ? instanceXml[DicomTags.SopClassUid].ToString()
		                                                             : instanceXml.SopClass.Uid);

		    _host = host;
			_aeTitle = aeTitle;
			_wadoUriPrefix = wadoUriPrefix;
			_wadoServicePort = wadoServicePort;
		}

		private InstanceXmlDicomAttributeCollection AttributeCollection
		{
			get { return (InstanceXmlDicomAttributeCollection)SourceMessage.DataSet; }
		}

		#region IStreamingSopDataSource Members

		public new IStreamingSopFrameData GetFrameData(FrameInfo info)
		{
			return (IStreamingSopFrameData) base.GetFrameData(info);
		}

        public override bool IsCacheable
        {
            get { return true; }
        }

		#endregion


        /// <summary>
        /// Gets the study instance UID.
        /// </summary>
        public override string StudyInstanceUid
        {
            get
            {
                return _instanceXml == null ? "" : _instanceXml.StudyInstanceUid;
            }
        }

        /// <summary>
        /// Gets the series instance UID.
        /// </summary>
        public override string SeriesInstanceUid
        {
            get
            {
                return _instanceXml == null ? "" : _instanceXml.SeriesInstanceUid;
            }
        }
        
        public override string TransferSyntaxUid
        {
            get
            {
                return _instanceXml == null ? "" : _instanceXml.TransferSyntax.UidString;
            }
        }
        
        public override string SopInstanceUid
        {
            get
            {
                return _instanceXml == null ? "" : _instanceXml.SopInstanceUid;
            }
        }
        public override string SopClassUid
        {
            get
            {
                var sourceFile = (DicomFile)SourceMessage;
                return sourceFile.MetaInfo[DicomTags.SopClassUid];
            }
        }
		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock(SyncLock)
				{
					if (NeedFullHeader(tag.TagValue))
						GetFullHeader();

					return base[tag];
				}
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				lock (SyncLock)
				{
					if (NeedFullHeader(tag))
						GetFullHeader();

					return base[tag];
				}
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock(SyncLock)
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


	    protected override StandardSopFrameData CreateFrameData(FrameInfo frameInfo)
		{
			return new StreamingSopFrameData(frameInfo, this);
		}

		private bool NeedFullHeader(uint tag)
		{
			if (_fullHeaderRetrieved)
				return false;

			if (AttributeCollection.IsTagExcluded(tag))
				return true;

			DicomAttribute attribute = base[tag];
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
				Exception retrieveException;
				DicomFile imageHeader = TryClientRetrieveImageHeader(out retrieveException);

				if (imageHeader != null)
				{
					base.SourceMessage = imageHeader;
					_fullHeaderRetrieved = true;
				}
				else
				{
					// if no result was returned, then the throw an exception with an appropriate, user-friendly message
					throw TranslateStreamingException(retrieveException);
				}
			}
		}

		private DicomFile TryClientRetrieveImageHeader(out Exception lastRetrieveException)
		{
			// retry parameters
			const int retryTimeout = 1500;
			int retryDelay = 50;
			int retryCounter = 0;

			Uri uri = new Uri(string.Format(StreamingSettings.Default.FormatWadoUriPrefix, _host, _wadoServicePort));
			StreamingClient client = new StreamingClient(uri);
			DicomFile result = null;
			lastRetrieveException = null;

			CodeClock timeoutClock = new CodeClock();
			timeoutClock.Start();

			while (true)
			{
				try
				{
					if (retryCounter > 0)
						Platform.Log(LogLevel.Info, "Retrying retrieve headers for Sop '{0}' (Attempt #{1})", this.SopInstanceUid, retryCounter);

					using (Stream imageHeaderStream = client.RetrieveImageHeader(_aeTitle, this.StudyInstanceUid, this.SeriesInstanceUid, this.SopInstanceUid))
					{
						DicomFile imageHeader = new DicomFile();
						imageHeader.Load(imageHeaderStream);
						result = imageHeader;
					}

					break;
				}
				catch (Exception ex)
				{
					lastRetrieveException = ex;

					timeoutClock.Stop();
					if (timeoutClock.Seconds*1000 >= retryTimeout)
					{
						// log an alert that we are aborting (exception trace at debug level only)
						int elapsed = (int) (1000*timeoutClock.Seconds);
						Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Aborting after {1} attempts in {2} ms", this.SopInstanceUid, retryCounter, elapsed);
						Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Abort] Sop: {0}, Retry Attempts: {1}, Elapsed: {2} ms", this.SopInstanceUid, retryCounter, elapsed);
						break;
					}
					timeoutClock.Start();

					retryCounter++;

					// log the retry (exception trace at debug level only)
					Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Retrying in {1} ms", this.SopInstanceUid, retryDelay);
					Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Retry] Sop: {0}, Retry in: {1} ms", this.SopInstanceUid, retryDelay);
					
					MemoryManager.Collect(retryDelay);
					retryDelay *= 2;
				}
			}

			return result;
		}

		/// <summary>
		/// Translates possible exceptions thrown by <see cref="StreamingClient"/> and related classes into standardized, user-friendly error messages.
		/// </summary>
		private static Exception TranslateStreamingException(Exception exception)
		{
			if (exception is StreamingClientException)
			{
				switch (((StreamingClientException) exception).Type)
				{
					case StreamingClientExceptionType.Access:
						return new InvalidOperationException(SR.MessageStreamingAccessException, exception);
					case StreamingClientExceptionType.Network:
						return new IOException(SR.MessageStreamingNetworkException, exception);
					case StreamingClientExceptionType.Protocol:
					case StreamingClientExceptionType.Server:
					case StreamingClientExceptionType.UnexpectedResponse:
					case StreamingClientExceptionType.Generic:
					default:
						return new Exception(SR.MessageStreamingGenericException, exception);
				}
			}
			return new Exception(SR.MessageStreamingGenericException, exception);
		}
	}
}
