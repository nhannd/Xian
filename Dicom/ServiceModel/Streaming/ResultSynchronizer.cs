﻿﻿#region License

//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    public class ResultSynchronizer
    {
        public class ResultWaiter : IDisposable 
        {
            private readonly ResultSynchronizer _synchronizer;
            private readonly string _cacheId;
            public ResultWaiter(string cacheId, ResultSynchronizer synchronizer)
            {
                _cacheId = cacheId;
                _synchronizer = synchronizer;

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="timeoutInMilliseconds"></param>
            /// <returns>true if result is cached, false otherwise</returns>
            public bool Wait(int timeoutInMilliseconds)
            {
                var result = _synchronizer.Acquire(_cacheId, AcquireType.CreateOrGet);
                if (result.ResultStatus != null)
                {
                    // if result is already in, return true
                    if (result.ResultStatus.ReceivedResult)
                    {
                        return true;
                    }

                    //otherwise, wait for it (if we didn't create it!!!)
                    if (!result.Created && result.ResultStatus.SynchEvent.WaitOne(timeoutInMilliseconds))
                    {
                        return true;
                    }
                }
                return false;

            }

            public void Dispose()
            {
                if (_synchronizer != null)
                    _synchronizer.Release(_cacheId);
            }
        }

        public class ResultStatus
        {
            public ResultStatus()
            {
                ReferenceCount = 1;
            }

            public bool ReceivedResult { get; set; }
            public ManualResetEvent SynchEvent { get; set; }
            public int ReferenceCount { get; set; }
        }

        public class AcquireResult
        {
            public AcquireResult()
            {
                Created = false;
            }

            public ResultStatus ResultStatus { get; set; }
            public bool Created { get; set; }
        }

        public enum AcquireType
        {
            CreateOnly,
            GetOnly,
            CreateOrGet
        }

        private const int CreateMask = 1;
        private const int GetMask = 2;


        private readonly IDictionary<string, ResultStatus> _results = new Dictionary<string, ResultStatus>();
        private readonly object _lock = new object();


        public ResultWaiter GetResultWaiter(string cacheId)
        {
            return new ResultWaiter(cacheId, this);
        }

        /// <summary>
        /// Get or Create SynchronizedResult.  The actual result is null, but the ManualResetEvent is created, so that other threads
        /// can wait on this event until the result is available.
        /// </summary>
        /// <param name="cacheId">cache id</param>
        /// <param name="acquireType"></param>
        /// <returns></returns>
        public AcquireResult Acquire(string cacheId, AcquireType acquireType)
        {
            switch (acquireType)
            {
                case AcquireType.CreateOnly:
                    return Acquire(cacheId, CreateMask);
                case AcquireType.GetOnly:
                    return Acquire(cacheId, GetMask);
                case AcquireType.CreateOrGet:
                    return Acquire(cacheId, CreateMask | GetMask);
            }
            return new AcquireResult();;
        }


        private AcquireResult Acquire(string cacheId, int mask)
        {
            if (string.IsNullOrEmpty(cacheId))
            {
                Platform.Log(LogLevel.Warn, "[ResultSynchronizer.Get] Empty cache id");
                return new AcquireResult();;
            }
            lock (_lock)
            {
                ResultStatus resultStatus;
                _results.TryGetValue(cacheId, out resultStatus);
                var create = (mask & CreateMask) != 0;
                var get = (mask & GetMask) != 0;
                if (create)
                {
                    if (resultStatus == null)
                    {
                        var result = new AcquireResult
                        {
                            ResultStatus =
                                new ResultStatus { SynchEvent = new ManualResetEvent(false) }
                        };
                        _results.Add(cacheId, result.ResultStatus);
                        result.Created = true;
                        return result;
                    }
                    if (!get)
                    {
                        Platform.Log(LogLevel.Warn,
                                     "[ResultSynchronizer.Get] Attempt to create a result that already exists");
                        return new AcquireResult();
                    }
                }

                if (get)
                {
                    if (resultStatus == null)
                    {
                        Platform.Log(LogLevel.Warn,
                                     "[ResultSynchronizer.Get] Attempt to get a result that does not exist");
                        return new AcquireResult();
                    }
                    resultStatus.ReferenceCount++;
                    var result = new AcquireResult { ResultStatus = resultStatus };
                    _results[cacheId] = result.ResultStatus;
                    return result;
                }
            }
            return new AcquireResult();
        }

        /// <summary>
        /// Set the actual result, and then set the event
        /// </summary>
        /// <param name="cacheId"></param>
        public void Set(string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId))
            {
                Platform.Log(LogLevel.Warn, "[ResultSynchronizer.Set] Empty cache id");
                return;
            }
            lock (_lock)
            {
                ResultStatus resultStatus;
                _results.TryGetValue(cacheId, out resultStatus);
                if (resultStatus == null)
                {
                    Platform.Log(LogLevel.Warn, "[ResultSynchronizer.Set] No result for cache id {0}", cacheId);
                    return;
                }
                resultStatus.ReceivedResult = true;
                resultStatus.SynchEvent.Set();
            }
        }

        /// <summary>
        /// Release the SynchronizedResult. If ref count drops to zero, then remove from result list.
        /// </summary>
        /// <param name="cacheId"></param>
        public void Release(string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId))
            {
                Platform.Log(LogLevel.Warn, "[ResultSynchronizer.Release] Empty cache id");
                return;
            }
            lock (_lock)
            {
                ResultStatus resultStatus;
                _results.TryGetValue(cacheId, out resultStatus);
                if (resultStatus == null)
                {
                    Platform.Log(LogLevel.Warn, "[ResultSynchronizer.Release] No result for cache id {0}", cacheId);
                    return;
                }
                resultStatus.ReferenceCount--;
                if (resultStatus.ReferenceCount == 0)
                {
                    _results.Remove(cacheId);
                }
            }
        }
    }
}