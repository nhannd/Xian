#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming.WebStation
{
    // TODO (CR Sep 2012): Remove

    [ExtensionOf(typeof(ServiceNodeServiceProviderExtensionPoint), Enabled = false)]
    internal class StudyLoaderServiceProvider : ServiceNodeServiceProvider
    {
        private bool IsStreamingServiceNode
        {
            get
            {
                var dicomServiceNode = Context.ServiceNode as IDicomServiceNode;
                return dicomServiceNode != null && dicomServiceNode.StreamingParameters != null;
            }
        }

        public override bool IsSupported(Type type)
        {
            return type == typeof(IStudyLoader) && IsStreamingServiceNode;
        }

        public override object GetService(Type type)
        {
            return IsSupported(type) ? new WebStationStreamingStudyLoader() : null;
        }
    }

    /// <summary>
    /// Special Streaming study loader to be used in a web server. Prefetch 
    /// should be less aggressive on the web server.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [ExtensionOf(typeof(StudyLoaderExtensionPoint), Enabled=false)]
    public class WebStationStreamingStudyLoader : StreamingStudyLoader
    {
        private WeightedWindowPrefetchingStrategy _strategy;
        private const string _loaderName = "CC_WEBSTATION_STREAMING";

        public WebStationStreamingStudyLoader()
            : base(_loaderName)
        {

        }

        protected override void InitStrategy()
        {
            var settings = new WebStationStreamingSettings();
            
            _strategy = new WeightedWindowPrefetchingStrategy(
                new StreamingCorePrefetchingStrategy(), _loaderName, SR.DescriptionWebStationPrefetchingStrategy)
                                      {
                                          Enabled = settings.RetrieveConcurrency > 0,
                                          RetrievalThreadConcurrency = Math.Max(settings.RetrieveConcurrency, 1),
                                          DecompressionThreadConcurrency = Math.Max(settings.DecompressConcurrency, 1),
                                          FrameLookAheadCount = settings.ImageWindow >= 0 ? (int?)settings.ImageWindow : null,
                                          SelectedImageBoxWeight = Math.Max(settings.SelectedWeighting, 1),
                                          UnselectedImageBoxWeight = Math.Max(settings.UnselectedWeighting, 0)
                                      };

            PrefetchingStrategy = _strategy;

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(String.Format("Streaming Prefetch Strategy: {0}", _strategy.Name));
                message.AppendLine(String.Format("\tEnabled: {0}", _strategy.Enabled ? "Yes" : "No"));
                message.AppendLine(String.Format("\tRetrieval Thread Concurrency: {0}", _strategy.RetrievalThreadConcurrency));
                message.AppendLine(String.Format("\tDecompression Thread Concurrency: {0}", _strategy.DecompressionThreadConcurrency));
                message.AppendLine(String.Format("\tFrame Lookahead Count: {0}", _strategy.FrameLookAheadCount));
                message.AppendLine(String.Format("\tSelected Imagebox Weight: {0}", _strategy.SelectedImageBoxWeight));
                message.AppendLine(String.Format("\tUnselected Imagebox Weight: {0}", _strategy.UnselectedImageBoxWeight));

                Platform.Log(LogLevel.Debug, message.ToString());                
            }
        }
    }
}