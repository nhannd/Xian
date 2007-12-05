#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

#pragma warning disable 1591

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Encapsulate the statistic update event.
    /// </summary>
    public class StatisticEventArg
    {
        #region Private Variables
        private readonly BaseStatistics _statistics =null;
        #endregion

        #region Public Properties
        /// <summary>
        /// The statistics object that has been updated
        /// </summary>
        public BaseStatistics Statistics
        {
            get { return _statistics; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stats"></param>
        public StatisticEventArg(BaseStatistics stats)
        {
            _statistics = stats;
        }
        #endregion
    }

    /// <summary>
    /// Defines method to handle a statistics update.
    /// </summary>
    /// <param name="theEvent"></param>
    public delegate void StatisticsUpdateEventHandler(StatisticEventArg theEvent);

    /// <summary>
    /// Defines the interface for all statistics classes.
    /// </summary>
    public interface IStatistics
    {
        /// <summary>
        /// Sets/Gets the description of the statistics
        /// </summary>
        string Description
        {
            get;
            set;
        }
        
        /// <summary>
        /// Called by the owner/statistics generator to signal the beginning of the recording
        /// </summary>
        void Begin();

        /// <summary>
        /// Called by the owner/statistics generator to signal the end of the recording
        /// </summary>
        void End();
    }

    
    /// <summary>
    /// The basic class which allows building a hiearchical statistics structure.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="BaseStatistics"/> can be used to store multi-level operation statistics if needed. 
    /// It provides sub-classes with a mechnism to populate each level with any type of statistics data. 
    /// The statistical data is kept in a generic collection accessible using indexer.
    /// </para>
    /// <para>
    /// Sub-level statistics is accessible using <seealso cref="SubStats"/>. The index <i>key</i> can be anything
    /// that uniquely identifies the statistics (if there are muliple statistics in the same level)</para>
    /// <para>
    /// <see cref="BasicStatisticsConverter"/> can be used to generate statistical report in XML format.
    /// </para>
    /// <para>Other features:</para>
    /// <para>It keeps track of the time when the recording is started/stoped when <seealso cref="Begin"/> or <seealso cref="End"/> are called.</para>
    /// <para>Listeners can register to statistics events using <seealso cref="StatisticsUpdated"/> to receive notification when statistics are updated</para>
    /// </remarks>
    [TypeConverter(typeof(BasicStatisticsConverter))]
    public abstract class BaseStatistics:IStatistics
    {
        #region Static Methods
        /// <summary>
        /// Returns name for a key to be used in an XML message.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static private string XmlElementName(string key)
        {
            if (key.StartsWith("@")) //Keys starting with "@" are considered as primary and should be listed first in the xml
            {
                // remove the "@", then make sure the name is encoded correctly for XML
                return XmlConvert.EncodeName(key.Substring(1).Trim());
            }
            else
            {
                return XmlConvert.EncodeName(key.Trim());
            }
        }
        #endregion

        #region Protected Members
        // the tick count when the statistics recording begins. In 100 nanoseconds
        protected long _beginTick;
        // the tick count when the statistics recording ends. In 100 nanoseconds
        protected long _endTick;
        // the description
        protected string _description;
        // store statistical/contextual data
        protected Dictionary<string, object> _statsValuesCollection = new Dictionary<string, object>();
        // list of sub-statistics
        protected Dictionary<object, BaseStatistics> _subStats = new Dictionary<object, BaseStatistics>();
        // event handler 
        protected EventHandlerList Events = new EventHandlerList();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="BaseStatistics"/>.
        /// </summary>
        /// <param name="desc">Description of the statistics</param>
        public BaseStatistics(string desc)
        {
            _description = desc;
            _beginTick = _endTick = 0;
            Begin(); // in case the caller forgets to do this
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the statistics is updated.
        /// </summary>
        public event StatisticsUpdateEventHandler StatisticsUpdated;
        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the statistics into an XML stream.
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>
        /// <para>
        /// All statistical data (name begins with "@") are stored as the <![CDATA[<Statistics> ]]> element attribute.
        /// Primary statistcal data are listed first, followed by secondary statistical ones.
        /// </para>
        /// </remarks>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Statistics");
            writer.WriteAttributeString("Type", this.GetType().Name);
            writer.WriteAttributeString("Description", Description);

            // list primary statistical data first
            foreach (string key in Details.Keys)
            {
                if (key.StartsWith("@")) // Primary info
                    writer.WriteAttributeString(XmlElementName(key), Details[key].ToString());
            }

            foreach (string key in Details.Keys)
            {
                if (!key.StartsWith("@")) // Secondary info
                    writer.WriteAttributeString(XmlElementName(key), Details[key].ToString());
            }

            // write sub-level statistics as child elements
            foreach (object subOp in SubStats.Keys)
            {
                SubStats[subOp].WriteXml(writer);
            }

            writer.WriteEndElement();

        } //WriteXml
        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the sub-level statistics.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Individual sub-level statistics can be accessed directly by index.</para>
        /// </remarks>
        /// <example>
        /// <code>
        ///         TransmissionStatistics stats = ...   
        /// 
        ///         Association assoc = .....
        /// 
        ///         // create a sub stats for the association, using the association as the index key
        ///         stats.SubStats[assoc] = new AssociationStatistics(....)
        /// 
        ///         // You can access statistical property within the sub-statistics directly using indexer
        ///         stats.SubStats[assoc]["RemoteAE"] = "remoteAE";
        /// </code>
        /// </example>
        public Dictionary<object, BaseStatistics> SubStats
        {
            get
            {
                return _subStats;
            }
        }

        /// <summary>
        /// Gets/Sets a statistical data
        /// </summary>
        /// <param name="key">Name of the statistical data</param>
        /// <returns>The statistical data object</returns>
        /// <remarks>
        /// <para>
        /// Statistic property name must be a valid XML element name (eg, no spaces).
        /// Primary statistical data must have the name beginning with "@". When converted into XML,
        /// primary statistical data will appear before secondary statistical data in the attribute list.
        /// </para>
        /// <para>
        /// When the data is set, an <see cref="StatisticsUpdated"/> event will be fired.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        ///         TransmissionStatistics stats  = ....
        ///         stats["@TransferRate"] = "100KB/s" // primary statistical data
        ///         stats["RemoteAE"] = remoteAE;   // secondary statistical data
        /// </code>
        /// </example>
        public object this[string key]
        {
            get
            {
                return _statsValuesCollection[key];
            }
            set
            {
                _statsValuesCollection[key] = value;
                RaiseUpdateEvent();
            }
        }

        /// <summary>
        /// Returns the list of all statistical data in the "root" level statistics.
        /// </summary>
        /// <remarks>
        /// Individual statistical data can be retrieved using the indexer.
        /// </remarks>
        /// <example>
        /// <code>
        ///      TransmissionStatistics stats  = ....
        ///      stats["@TransferRate"] = "100KB/s" // primary statistical info
        ///      stats["TimeStamp"] = Platform.Time;   // secondary statistical info
        /// </code>
        /// is the same as
        /// <code>
        ///      stats.Details["@TransferRate"] = "100KB/s"
        ///      stats.Details["TimeStamp"] = Platform.Time;
        /// </code>
        /// </example>
        public IDictionary<string, object> Details
        {
            get
            {
                return _statsValuesCollection;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Notifies event handlers when statistics is updated.
        /// </summary>
        /// <remarks>
        /// Event handlers are registered through <seealso cref="StatisticsUpdated"/>
        /// </remarks>
        protected void RaiseUpdateEvent()
        {
            if (StatisticsUpdated!=null)
               StatisticsUpdated(new StatisticEventArg(this));
        }
        #endregion

        #region IStatistics interface
        public string Description
        {
            get { return _description; }
            set 
            { 
                _description = value; 
                RaiseUpdateEvent(); // should we fire update event?
            }
        }

        public void Begin()
        {
            _beginTick = _endTick = Platform.Time.Ticks;
            OnBegin(); // sub-class can do additional stuff
            RaiseUpdateEvent();
        }

        public void End()
        {
            _endTick = Platform.Time.Ticks;

            OnEnd(); // sub-class can do additional stuff

            RaiseUpdateEvent();
        }       
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Called by <seealso cref="BaseStatistics"/> when Begin() is called, prior to firing StatisticsUpdated event.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The base statistics class keeps track of when Begin() is called. 
        /// Sub-classes should override this method to specify additional functionality when Begin() is called.
        /// </para>
        /// </remarks>
        abstract protected void OnBegin();

        /// <summary>
        /// Called by <seealso cref="BaseStatistics"/> when End() is called, prior to firing StatisticsUpdated event.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The base statistics class keeps track of when End() is called. 
        /// Sub-classes should override this method to specify additional functionality when End() is called.
        /// </para>
        /// </remarks>
        abstract protected void OnEnd();
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("Statistics: {0}", Description);
        }
        #endregion
    }

    /// <summary>
    /// Statistics to record the size of a object such as image, file, etc
    /// </summary>
    /// <remarks>
    /// <para>
    /// To set the size, assign value to <seealso cref="SizeInBytes"/> between calling <seealso cref="OnBegin()"/> and <seealso cref="OnEnd()"/>
    /// </para>
    /// </remarks>
    public class SizeStatistics : BaseStatistics
    {
        #region Private Members
        private ulong _size;
        #endregion

        #region Public Members
        ///<summary>
        /// Size in bytes.
        ///</summary>
        public ulong SizeInBytes
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                this["@SizeInKB"] = string.Format("{0:0}", value / 1000);
            }
        }
        #endregion

        #region Constructors
        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="desc">A description of the statistic</param>
        public SizeStatistics(string desc)
            : base(desc)
        {
        }
        #endregion

        #region Override Methods
        protected override void OnBegin()
        {
            // NOOP
        }

        protected override void OnEnd()
        {
            // NOOP
        }
        #endregion
    }



}
