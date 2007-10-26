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

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Encapsulate the statistic update event 
    /// </summary>
    public class StatisticEventArg
    {
        /// <summary>
        /// The statistics object that has been updated
        /// </summary>
        public BaseStatistics Statistics;

        public StatisticEventArg(BaseStatistics stats)
        {
            Statistics = stats;
        }

    }

    public delegate void StatisticsUpdateEventDelegate(StatisticEventArg ev);

    /// <summary>
    /// Defines the interface for all statistics classes
    /// </summary>
    public interface IStatistics
    {
        /// <summary>
        /// Set/Get the description of the statistics
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
    /// Basic statistics which allows building a hiearchical statistics structure. 
    /// This structure can be used to store multi-level operation statistics if needed.
    /// 
    /// <para>It provides sub-classes with a mechnism to populate each level with any type of statistics data. 
    /// The values are kept in a generic collection accessible using indexer <seealso cref="this[key]"/>
    /// </para>
    /// 
    /// <para>The level is also accessible by indexer using <seealso cref="SubStats[key]"/></para>
    /// 
    /// <para></para>
    /// <para>Other features:</para>
    /// <para>It keeps track of the time when the recording is started/stoped when <seealso cref="Begin"/> or <seealso cref="End"/> are called.</para>
    /// <para>Listeners can register to statistics events using <seealso cref="OnStatisticsUpdated"/> and receive notification when statistics are updated</para>
    /// 
    /// <para>
    /// Known sub-classes:
    /// <seealso cref="TimeSpanStatistics"/>,<seealso cref="TransmissionStatistics"/>
    /// </para>
    /// </summary>
    /// 
    [TypeConverter(typeof(BasicStatisticConverter))]
    public abstract class BaseStatistics:IStatistics
    {
        /// <summary>
        ///  Class to use to convert the statistics to other types.
        /// </summary>
        public class BasicStatisticConverter : TypeConverter
        {

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return true;
                }
                else
                    return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    // spit out all properties as XML
                    BaseStatistics stats = value as BaseStatistics;
                    
                    StringBuilder sb = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = ("\t");

                    using (XmlWriter writer = XmlWriter.Create(sb, settings))
                    {
                        stats.WriteXml(writer);
                        writer.Flush();
                    }

                    return sb.ToString();
                }
                else
                    return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #region static methods

        static private string XMLElementName(string key)
        {
            if (key.StartsWith("@"))
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

        #region private members
        // the tick count when the statistics recording begins. In 100 nanoseconds
        protected long _beginTick;
        // the tick count when the statistics recording ends. In 100 nanoseconds
        protected long _endTick;
        // the description
        protected string _description;
        // store statistical/contextual values
        protected Dictionary<string, object> _statsValuesCollection = new Dictionary<string, object>();
        // list of sub-statistics
        protected Dictionary<object, BaseStatistics> _subStats = new Dictionary<object, BaseStatistics>();
        // event handler 
        protected EventHandlerList Events = new EventHandlerList();
        #endregion

        #region Constructors
        /// <summary>
        /// Create a statistics object
        /// </summary>
        /// <param name="desc">Description of the statistics</param>
        public BaseStatistics(string desc)
        {
            _description = desc;
            _beginTick = _endTick = 0;
            Begin();
        }

        
        #endregion

        #region private methods
        /// <summary>
        /// Notify event handlers of changes to the statistics
        /// Event handlers are registered through <seealso cref="OnStatisticsUpdated"/>
        /// </summary>
        protected void RaiseUpdateEvent()
        {
            StatisticsUpdateEventDelegate handler = Events["OnStatisticsUpdate"] as StatisticsUpdateEventDelegate;
            if (handler != null)
            {
                handler(new StatisticEventArg(this));
            }
        }



        /// <summary>
        /// Write the statistics into an XML stream.
        /// <para>
        /// All statistical data (name begins with "@") are stored as the <![CDATA[<Statistics> ]]> element attribute.
        /// Primary statistcal data are listed first, followed by secondary statistical ones.
        /// </para>
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Statistics");
            writer.WriteAttributeString("Type", this.GetType().Name);
            writer.WriteAttributeString("Description", Description);
            foreach (string key in Details.Keys)
            {
                if (key.StartsWith("@")) // Primary info
                    writer.WriteAttributeString(XMLElementName(key), Details[key].ToString());
            }

            foreach (string key in Details.Keys)
            {
                if (!key.StartsWith("@")) // Secondary info
                    writer.WriteAttributeString(XMLElementName(key), Details[key].ToString());
            }

            // write sub statistics as children
            foreach (object subOp in SubStats.Keys)
            {
                SubStats[subOp].WriteXml(writer);
            }

            writer.WriteEndElement();

        } //WriteXml
        #endregion

        #region Events

        public event StatisticsUpdateEventDelegate OnStatisticsUpdated
        {
            add
            {
                Events.AddHandler("OnStatisticsUpdate", value);
            }
            remove
            {
                Events.RemoveHandler("OnStatisticsUpdate", value);
            }

        }

        #endregion

        #region Virtual methods
        /// <summary>
        /// subclasses should override this method to specify additional functionality on Begin() is called.
        /// <para>The base statistics class keeps track of when Begin() is called</para>
        /// </summary>
        abstract protected void OnBegin();
        /// <summary>
        /// subclasses should override this method to specify additional functionality on Begin() is called.
        /// <para>The base statistics class keeps track of when End() is called</para>
        /// </summary>
        abstract protected void OnEnd();
        #endregion


        #region Public members
        /// <summary>
        /// Retrieve the sub-level statistics. Sub-statistics are indexable.
        /// 
        /// </summary>
        /// 
        /// <example>
        ///         TransmissionStatistics stats = ...   
        /// 
        ///         Association assoc = .....
        /// 
        ///         // create a sub stats for the association, using the association as the index key
        ///         stats.SubStats[assoc] = new AssociationStatistics(....)
        /// 
        ///         // You can access statistical property within the sub-statistics directly using indexer
        ///         stats.SubStats[assoc]["RemoteAE"] = "remoteAE";
        /// 
        /// 
        /// </example>
        ///
        public Dictionary<object, BaseStatistics> SubStats
        {
            get
            {
                return _subStats;
            }
        }

        /// <summary>
        /// Get/Set a statistic property
        /// Statistic property name must be a valid XML element name (eg, no spaces)
        /// Primary statistic properties should have the name beginning with "@". When converted into XML,
        /// primary properties will appear as the element attributes. Secondary statistics (eg contextual ones)
        /// will be child elements.
        /// 
        /// </summary>
        /// <param name="key">Name of the statistics</param>
        /// <returns></returns>
        /// 
        /// <example>
        ///         TransmissionStatistics stats  = ....
        ///         stats["@TransferRate"] = "100KB/s" // primary statistical info
        ///         stats["RemoteAE"] = remoteAE;   // secondary statistical info
        /// </example>
        /// 
        /// 
        public object this[string key]
        {
            // Use XmlConvert.EncodeName to translate the key into valid XML element name format
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
        /// Returns the statistical/contextual values set by the statistic recorder.
        /// If you know the name of the value, you can <seealso cref="this[key]"/> to retrieve it
        /// 
        /// </summary>
        /// 
        /// <example>
        ///      TransmissionStatistics stats  = ....
        ///      stats["@TransferRate"] = "100KB/s" // primary statistical info
        ///      stats["TimeStamp"] = DateTime.Now;   // secondary statistical info
        /// 
        /// is the same as
        /// 
        ///      stats.Details["@TransferRate"] = "100KB/s"
        ///      stats.Details["TimeStamp"] = DateTime.Now;
        /// 
        /// </example>
        /// 
        public IDictionary<string, object> Details
        {
            get
            {
                return _statsValuesCollection;
            }
        }

        public override string ToString()
        {
            return string.Format("Statistics:{0}", Description);
        }
        #endregion

        #region IStatisticsComponent interface

        public string Description
        {
            get { return _description; }
            set { _description = value; RaiseUpdateEvent(); }
        }


        virtual public void Begin()
        {
            //
            // this["StartTimeStamp"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.ffffff");  this is probably not important

            _beginTick = _endTick = DateTime.Now.Ticks;
            OnBegin();
            RaiseUpdateEvent();
        }

        virtual public void End()
        {
            _endTick = DateTime.Now.Ticks;
           
            //this["EndTimeStamp"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.ffffff");
            OnEnd();
            RaiseUpdateEvent();
        }

        

        
        #endregion


        #region public methods
        

        #endregion



    }


    /// <summary>
    /// Statistics to record the elapsed time between two "events"
    /// <para>
    /// Users call <seealso cref="Begin()"/> and <seealso cref="End()"/> to signal
    /// the beginning and at of the events. The elapsed time can be determine using <seealso cref="ElapsedTimeInMs"/>
    /// </para>
    /// </summary>
    public class TimeSpanStatistics : BaseStatistics
    {
        // elapsed time between begin and end of the recording. In 100 nanoseconds
        protected long _elapsedTime;
        public long ElapsedTimeInMs
        {
            get
            {
                return _elapsedTime;
            }
            set
            {
                _elapsedTime = value;
                _statsValuesCollection["@ElapsedTimeInMs"] = value;
            }
        }
        

        public TimeSpanStatistics(string desc)
            : base(desc)
        {
        }

        protected override void OnBegin()
        {
            // NOOP
        }

        protected override void OnEnd()
        {
            ElapsedTimeInMs = (_endTick - _beginTick) / 10000; // convert from 100 ns to ms
        }
    }

    /// <summary>
    /// Statistics to record the size of a object such as image, file, etc
    /// <para>
    /// To set the size, assign value to <seealso cref="SizeInBytes"/> between calling <seealso cref="Begin()"/> and <seealso cref="End()"/>
    /// </para>
    /// </summary>
    public class SizeStatistics : BaseStatistics
    {

        #region private members
        private ulong _size;
        #endregion

        #region public members

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

        #region constructors

        public SizeStatistics(string desc)
            : base(desc)
        {
        }

        #endregion

        #region override methods
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
    /// <summary>
    /// Class to record the statistics of a transmissions
    /// <para>
    /// Users call <seealso cref="Begin()"/> and <seealso cref="End()"/> to signal
    /// the beginning and at of the events. Before calling <seealso cref="End()"/>, 
    /// users should set <seealso cref="SizeInBytes"/>, <seealso cref="MessageCount"/>, <seealso cref="Status"/>.
    /// The following transmission statistics will be derived automatically:
    /// <para>
    ///     <seealso cref="AverageSpeedInBytesPerSec"/>
    ///     <seealso cref="AverageMessageRate"/>
    /// </para>
    /// </para>
    /// </summary>
    public class TransmissionStatistics : TimeSpanStatistics
    {

        #region private members
        private ulong _size;
        private double _speed; // byte/sec
        int _messageCount;
        double _messageRate;
        string _status;
        #endregion

        #region public members

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
        
        public double AverageSpeedInBytesPerSec
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                this["@AverageSpeedInKBPerSec"] = string.Format("{0:0.0}", value/1000);
            }
        }

        public int MessageCount
        {
            get
            {
                return _messageCount;
            }
            set
            {
                _messageCount = value;
                this["@MessageCount"] = string.Format("{0}", value);
            }
        }

        public double AverageMessageRate
        {
            get
            {
                return _messageRate;
            }
            set
            {
                _messageRate = value;
                this["@AverageMessageRatePerSec"] = string.Format("{0:0.00}", _messageRate);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                this["@Status"] = value;
            }
        }

        #endregion

        #region constructors

        public TransmissionStatistics(string desc)
            : base(desc)
        {
        }

        #endregion

        #region override methods
        protected override void OnBegin()
        {
            base.OnBegin();
            // Nothing else to do
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            // calculate the speed
            if (ElapsedTimeInMs <= 0)
            {
                AverageSpeedInBytesPerSec = 0;
                AverageMessageRate = 0;
            }
            else
            {
                double elapsedTimeInSec = ElapsedTimeInMs / 1000.0;

                AverageSpeedInBytesPerSec = SizeInBytes / elapsedTimeInSec;
                AverageMessageRate = MessageCount / elapsedTimeInSec;
            }
        }

        #endregion


    }


}
