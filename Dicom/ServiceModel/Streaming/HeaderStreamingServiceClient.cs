using System;
using System.IO.Compression;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Dicom.Utilities.Xml.Nodes;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    public class HeaderStreamingServiceClient : ClientBase<IHeaderStreamingService>, IHeaderStreamingService
    {
        /// <summary>
        /// Constructor - uses the default endpoint and binding configuration.
        /// </summary>
        public HeaderStreamingServiceClient()
		{
		}

        /// <summary>
        /// Constructor - uses the default binding configuration and the given endpoint address.
        /// </summary>
        public HeaderStreamingServiceClient(EndpointAddress remoteAddress)
        {
            Endpoint.Address = remoteAddress;
        }

        /// <summary>
        /// Constructor - uses the default binding configuration and the given endpoint uri.
        /// </summary>
        public HeaderStreamingServiceClient(Uri uri)
        {
            Endpoint.Address = new EndpointAddress(uri);
        }

		/// <summary>
		/// Constructor - uses input configuration name to configure endpoint and bindings.
		/// </summary>
		public HeaderStreamingServiceClient(string endpointConfigurationName)
			: base(endpointConfigurationName)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint and binding.
		/// </summary>
		public HeaderStreamingServiceClient(Binding binding, EndpointAddress remoteAddress)
			: base(binding, remoteAddress)
		{
		}

		/// <summary>
		/// Constructor - uses input endpoint, loads bindings from given configuration name.
		/// </summary>
        public HeaderStreamingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
		}
        
        #region IHeaderStreamingService Members

        /// <summary>
        /// Gets the study header (xml) as a gzipped stream.
        /// </summary>
        /// <param name="callingAETitle">AE title of the local application.</param>
        /// <param name="parameters">Input parameters.</param>
        /// <returns></returns>
        public System.IO.Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters)
        {
            return base.Channel.GetStudyHeader(callingAETitle, parameters);
        }

        #endregion

        /// <summary>
        /// Gets the study header (via <see cref="GetStudyHeader"/>), unzips the gzipped stream, and returns it as <see cref="StudyXml"/>.
        /// </summary>
        /// <param name="callingAETitle">AE title of the local application.</param>
        /// <param name="parameters">Input parameters.</param>
        /// <returns></returns>
        public StudyXml GetStudyXml(string callingAETitle, HeaderStreamingParameters parameters, bool isPrior)
        {
            var studyXml = new StudyXml();
            using (var stream = GetStudyHeader(callingAETitle, parameters))
            {
                using (var gzStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var reader = new ReaderWrapper(gzStream);
                    studyXml.SetMemento(new ReaderNode(reader), isPrior);
                    gzStream.Close();
                }
            }
            return studyXml;
        }
    }
}
