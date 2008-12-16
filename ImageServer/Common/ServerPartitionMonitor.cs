#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using Timer=System.Threading.Timer;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Event args for partition monitor
	/// </summary>
    public class ServerPartitionChangedEventArgs:EventArgs
    {
    	private readonly ServerPartitionMonitor _monitor;
		public ServerPartitionChangedEventArgs(ServerPartitionMonitor theMonitor)
		{
			_monitor = theMonitor;
		}

    	public ServerPartitionMonitor Monitor
    	{
    		get { return _monitor; }
    	}
    }

	/// <summary>
	/// Singleton class that monitors the currently loaded server partitions.
	/// </summary>
    public class ServerPartitionMonitor :  IEnumerable<ServerPartition>, IDisposable
	{
		#region Private Members
		private readonly object _partitionsLock = new Object();
        private Dictionary<string, ServerPartition> _partitions = new Dictionary<string,ServerPartition>();
        private EventHandler<ServerPartitionChangedEventArgs> _changedListener;
        private readonly Timer _timer;
        private static readonly ServerPartitionMonitor _instance = new ServerPartitionMonitor();
		#endregion

		#region Static Properties
		/// <summary>
		/// Singleton monitor class for the <see cref="ServerPartition"/> table.
		/// </summary>
		static public ServerPartitionMonitor Instance
        {
            get
            {
                return _instance;
            }
		}
		#endregion

		#region Private Constructors
		/// <summary>
        /// ***** internal use only ****
        /// </summary>
        private ServerPartitionMonitor()
        {
            LoadPartitions();

            _timer = new Timer(SynchDB, null, TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds), TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds));
		}
		#endregion

		#region Events
		/// <summary>
		/// Event for notification when partitions change.
		/// </summary>
		public event EventHandler<ServerPartitionChangedEventArgs> Changed
		{
			add { _changedListener += value; }
			remove { _changedListener -= value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get a partition based on an AE Title.
		/// </summary>
		/// <param name="serverAE"></param>
		/// <returns></returns>
		public ServerPartition GetPartition(string serverAE)
        {
            if (String.IsNullOrEmpty(serverAE))
                return null;

            lock(_partitionsLock)
            {
                if (_partitions.ContainsKey(serverAE))
                    return _partitions[serverAE];
                else
                    return null;
            }
		}

        public ServerPartition FindPartition(ServerEntityKey key)
        {
            return CollectionUtils.SelectFirst(
                       this,
                       delegate(ServerPartition partition)
                       {
                           return partition.GetKey().Equals(key);
                       });
        }
		#endregion

		#region Private Methods
		/// <summary>
		/// Internal method for loading partition information fromt he database.
		/// </summary>
		private void LoadPartitions()
        {
            bool changed = false;
            lock(_partitionsLock)
            {
                Dictionary<string, ServerPartition> templist = new Dictionary<string, ServerPartition>();
                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                using (IReadContext ctx = store.OpenReadContext())
                {
                    IServerPartitionEntityBroker broker = ctx.GetBroker<IServerPartitionEntityBroker>();
                    ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                    IList<ServerPartition> list = broker.Find(criteria);
                    foreach(ServerPartition partition in list)
                    {
                        if (IsChanged(partition))
                        {
                            changed = true;
                        }

                        templist.Add(partition.AeTitle, partition);
                    }
                }

                _partitions = templist;
            }

            if (changed && _changedListener!=null)
            {
                EventsHelper.Fire(_changedListener, this, new ServerPartitionChangedEventArgs(this));
            }
        }

		/// <summary>
		/// Timer method for synchronizing with the database.
		/// </summary>
		/// <param name="state"></param>
		private void SynchDB(object state)
		{
			try
			{
				LoadPartitions();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e,
				             "Unexpected exception when loading partitions, possible database error.  Operation will be reried later");
			}
		}

		private bool IsChanged(ServerPartition p2)
        {
            if (_partitions.ContainsKey(p2.AeTitle))
            {
                ServerPartition p1 = _partitions[p2.AeTitle];
                if (p1.AcceptAnyDevice != p2.AcceptAnyDevice)
                {
                    return true;
                }
                if (p1.AutoInsertDevice != p2.AutoInsertDevice)
                {
                    return true;
                }

                if (p1.DefaultRemotePort != p2.DefaultRemotePort)
                {
                    return true;
                }

                if (p1.Description != p2.Description)
                {
                    return true;
                }

                if (!p1.DuplicateSopPolicyEnum.Equals(p2.DuplicateSopPolicyEnum))
            		return true;

				if (p1.MatchAccessionNumber != p2.MatchAccessionNumber
					|| p1.MatchIssuerOfPatientId != p2.MatchIssuerOfPatientId
					|| p1.MatchPatientId != p2.MatchPatientId
					|| p1.MatchPatientsBirthDate != p2.MatchPatientsBirthDate
					|| p1.MatchPatientsName != p2.MatchPatientsName
					|| p1.MatchPatientsSex != p2.MatchPatientsSex)
					return true;

                if (p1.Enabled != p2.Enabled)
                {
                    return true;
                }

                if (p1.PartitionFolder != p2.PartitionFolder)
                {
                    return true;
                }

                if (p1.Port != p2.Port)
                {
                    return true;
                }

                else
                    return false;
            }
            else
            {
                return true;
            }
        }
		#endregion

        #region IEnumerable<ServerPartition> Members

        public IEnumerator<ServerPartition> GetEnumerator()
        {
            return _partitions.Values.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _partitions.Values.GetEnumerator();
        }

        #endregion

    	public void Dispose()
    	{
    		_timer.Dispose();
    	}
    }
}
