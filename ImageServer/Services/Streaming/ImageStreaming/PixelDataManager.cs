using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{

    class PixelDataManager
    {
        delegate void PixelDataLoadedDelegate(string seriesInstanceUid, string sopInstanceUid);

        static private readonly Cache _cache = HttpRuntime.Cache;
        private readonly StudyStorageLocation _storage;
        private readonly PixelDataLoadedDelegate _onPixelDataLoaded;

        private readonly Dictionary<string, SeriesPrefetch> _prefetchers = new Dictionary<string, SeriesPrefetch>();
        private readonly bool _enablePrefetch;

        public static PixelDataManager GetInstance(StudyStorageLocation storage)
        {
            string key = storage.Key.ToString();
            lock (_cache)
            {
                PixelDataManager instance = _cache[key] as PixelDataManager;
                if (instance == null)
                {
                    instance = new PixelDataManager(storage, ImageStreamingServerSettings.Default.EnablePrefetch);
                    _cache.Add(key, instance, null, Cache.NoAbsoluteExpiration, ImageStreamingServerSettings.Default.CacheRetentionWindow, CacheItemPriority.Default, UnloadPixelDataManager);

                }
                return instance;
            }
        }

        public void Dispose()
        {
            foreach (SeriesPrefetch prefetcher in _prefetchers.Values)
            {
                prefetcher.Stop();
            }
        }

        private static void UnloadPixelDataManager(string key, object value, CacheItemRemovedReason reason)
        {
            PixelDataManager instance = value as PixelDataManager;
            instance.Dispose();
        }

        private PixelDataManager(StudyStorageLocation storage, bool enablePrefetch)
        {
            _storage = storage;
            _enablePrefetch = enablePrefetch;

            _onPixelDataLoaded += delegate(string seriesInstanceUid, string sopInstanceUid)
                                     {
                                         if (_enablePrefetch)
                                         {
                                             SeriesPrefetch prefetcher = GetSeriesPrefetcher(seriesInstanceUid);
                                             prefetcher.OnImageLoaded(sopInstanceUid);
                                         } 
                                     }; 

        }


        public DicomPixelData GetPixelData(string seriesInstanceUid, string sopInstanceUid)
        {
            DicomPixelData pd = null;

            for (int i = 0; i < 5; i++)
            {
                // look at the cache first
                pd = DicomPixelDataCache.Find(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid);
                if (pd != null)
                    break;

                else
                {
                    try
                    {
                        pd = DicomPixelData.CreateFrom(_storage.GetSopInstancePath(seriesInstanceUid, sopInstanceUid));
                        DicomPixelDataCache.Insert(_storage, _storage.StudyInstanceUid, seriesInstanceUid, sopInstanceUid, pd);
                        break;
                    }
                    catch (IOException)
                    {
                        Random rand = new Random();
                        Thread.Sleep(rand.Next(100, 500));
                    }
                }
                
            }
            OnPixelDataLoaded(seriesInstanceUid, sopInstanceUid);
            return pd;
        }

        private SeriesPrefetch GetSeriesPrefetcher(string seriesInstanceUid)
        {
            lock (_prefetchers)
            {
                if (_prefetchers.ContainsKey(seriesInstanceUid))
                    return _prefetchers[seriesInstanceUid];
                else
                {
                    SeriesPrefetch prefetcher = new SeriesPrefetch(_storage, seriesInstanceUid);
                    _prefetchers.Add(seriesInstanceUid, prefetcher);
                    return prefetcher;
                }
            }

        }
        private void OnPixelDataLoaded(string seriesInstanceUid, string sopInstanceUid)
        {
            if (_onPixelDataLoaded != null)
            {
                // Launch this asynchronously so that the pixel data can be returned to the client asap
                IAsyncResult result = _onPixelDataLoaded.BeginInvoke(seriesInstanceUid, sopInstanceUid, null, null);
                _onPixelDataLoaded.EndInvoke(result);
            }

        }
    }


    class PrefetchQueueItem
    {
        private string _seriesInstanceUid;
        private string _sopInstanceUid;
        private string _instanceNumner;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }

        public string InstanceNumner
        {
            get { return _instanceNumner; }
            set { _instanceNumner = value; }
        }
    }

    class SeriesPrefetch
    {
        private readonly StudyStorageLocation _storage;
        private bool _stop;
        private DateTime? _lastTransmission;
        private readonly string _seriesInstanceUid;
        private readonly SeriesXml _seriesXml;
        readonly AutoResetEvent _enqueueEvent = new AutoResetEvent(false);

        private int _currentClientIndex = 0;
        private int _prefetchStartIndex = 1;    
        
        private readonly List<InstanceXml> _instanceXmlList = new List<InstanceXml>();
        private readonly List<string> _sopUidList = new List<string>();
        
        public SeriesPrefetch(StudyStorageLocation storage, string seriesInstanceUid)
        {
            _storage = storage;
            _seriesInstanceUid = seriesInstanceUid;
            _seriesXml = storage.LoadStudyXml()[seriesInstanceUid];

            foreach (InstanceXml instanceXml in _seriesXml)
            {
                _instanceXmlList.Add(instanceXml);
            }

            lock (((ICollection)_instanceXmlList).SyncRoot)
            {
                _instanceXmlList.Sort(SortByInstanceNumber);
            }

            foreach (InstanceXml instanceXml in _instanceXmlList)
            {
                _sopUidList.Add(instanceXml.SopInstanceUid);
            }

            Thread prefetchThread = new Thread(Run);
            prefetchThread.Start();
        }

        private static int InitialPrefetchSize
        {
            get { return ImageStreamingServerSettings.Default.InitialPrefetchSize; }
        }

        private static int StackingPrefetchSize
        {
            get { return ImageStreamingServerSettings.Default.StackingPrefetchSize; }
        }

        private static int ClientPrefetchSize
        {
            get { return ImageStreamingServerSettings.Default.ClientPrefetchSize; }
        }

        private int PrefetchStartIndex
        {
            get
            {
                return _prefetchStartIndex;
            }
            set
            {
                _prefetchStartIndex = value;
            }
        }

        private bool IsStacking
        {
            get
            {
                return
                    _currentClientIndex > ClientPrefetchSize && (_lastTransmission != null && DateTime.Now - _lastTransmission < TimeSpan.FromSeconds(1));
            }
        }

        
        private static int SortByInstanceNumber(InstanceXml instance1, InstanceXml instance2)
          {
              int instance1Num;
              int instance2Num;
              if (instance1 != null && instance2 != null)
              {
                  DicomAttributeCollection c1 = instance1.Collection;
                  DicomAttributeCollection c2 = instance2.Collection;
                  if (c1 != null & c2 != null && c1.Contains(DicomTags.InstanceNumber) && c2.Contains(DicomTags.InstanceNumber))
                  {
                      if (
                          int.TryParse(instance1.Collection[DicomTags.InstanceNumber].ToString(), out instance1Num) &&
                          int.TryParse(instance2.Collection[DicomTags.InstanceNumber].ToString(), out instance2Num))
                      {
                          return instance1Num.CompareTo(instance2Num);
                      }
                  }
              }

              return 0;

          }

        private void Run()
        {
            Reset();

            while (!_stop)
            {
                if (!IsStacking)
                {
                    if (PrefetchStartIndex <= ClientPrefetchSize + InitialPrefetchSize)
                    {
                        if (PrefetchStartIndex >= _sopUidList.Count)
                            break;

                        PrefetchQueueItem item = new PrefetchQueueItem();
                        item.SeriesInstanceUid = _seriesInstanceUid;
                        item.SopInstanceUid = _sopUidList[PrefetchStartIndex];
                        item.InstanceNumner = _instanceXmlList[PrefetchStartIndex].Collection[DicomTags.InstanceNumber].ToString();
                        Prefetch(item);
                        PrefetchStartIndex++;
                    }
                }
                else
                {

                    for (int i = 0; i < StackingPrefetchSize; i++)
                    {
                        PrefetchStartIndex = Math.Max(PrefetchStartIndex, _currentClientIndex + ClientPrefetchSize);
                        if (PrefetchStartIndex >= _sopUidList.Count)
                            break;

                        PrefetchQueueItem item = new PrefetchQueueItem();
                        item.SeriesInstanceUid = _seriesInstanceUid;
                        item.SopInstanceUid = _sopUidList[PrefetchStartIndex];
                        item.InstanceNumner = _instanceXmlList[PrefetchStartIndex].Collection[DicomTags.InstanceNumber].ToString();
                        Prefetch(item);
                        PrefetchStartIndex++;
                    }
                }

                _enqueueEvent.WaitOne(1000);

            }

            Platform.Log(LogLevel.Debug, "Prefetch has stopped. Series {0}", _seriesInstanceUid);
        }

        public void Reset()
        {
            _currentClientIndex = 0;
            PrefetchStartIndex = ClientPrefetchSize;
        }

        public void Stop()
        {
            _stop = true;
        }

        private void Prefetch(PrefetchQueueItem item)
        {
            for (int i = 0; i < 5; i++)
            {
                DicomPixelData pd = DicomPixelDataCache.Find(_storage, _storage.StudyInstanceUid, item.SeriesInstanceUid, item.SopInstanceUid);
                if (pd!=null)
                    break;

                else
                {
                    try
                    {
                        pd = DicomPixelData.CreateFrom(_storage.GetSopInstancePath(item.SeriesInstanceUid, item.SopInstanceUid));
                        DicomPixelDataCache.Insert(_storage, _storage.StudyInstanceUid, item.SeriesInstanceUid,item.SopInstanceUid, pd);
                        Platform.Log(LogLevel.Info, "Prefetch Image #{0} : SOP {1}. Client's last known image (MAX): {2}",item.InstanceNumner, item.SopInstanceUid, _currentClientIndex);
                        break;
                    }
                    catch (IOException)
                    {
                        Random rand = new Random();
                        Thread.Sleep(rand.Next(100, 500));
                    }
                }
            }
        }

        public void OnImageLoaded(string sopInstanceUid)
        {
            _lastTransmission = DateTime.Now;
            if (_sopUidList.IndexOf(sopInstanceUid) == 0)
            {
                // client has re-opened this series?
                Reset();
            }
            else
            {
                _currentClientIndex = Math.Max(_currentClientIndex, _sopUidList.IndexOf(sopInstanceUid));
            }
            _enqueueEvent.Set();
        }
    }


}