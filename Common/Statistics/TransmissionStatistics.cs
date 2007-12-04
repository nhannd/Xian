namespace ClearCanvas.Common.Statistics
{

    /// <summary>
    /// Used to store the statistics of a transmissions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The caller is repsonsible for populating the statistics. 
    /// <seealso cref="Begin()"/> and <seealso cref="End()"/> should be called to signal
    /// the beginning and the end of the transmission. Before calling <seealso cref="End()"/>, 
    /// callers should set <seealso cref="InboundBytes"/>, <seealso cref="OutboundBytes"/>, 
    /// <seealso cref="InboundMessageCount"/>, <seealso cref="OutboundMessageCount"/>.
    /// The following transmission statistics will be derived automatically:
    /// <list type="bullet">
    /// <item><seealso cref="TotalSizeInBytes"/></item>
    /// <item><seealso cref="AverageSpeedInBytesPerSec"/></item>
    /// <item><seealso cref="TotalMessageCount"/></item>
    /// <item><seealso cref="AverageMessageRate"/></item>
    /// </list>    
    /// </para>
    ///
    /// </remarks>
    /// 
    public class TransmissionStatistics : TimeSpanStatistics
    {

        #region Private Variables
        private ulong _inSize;  // inbound message size
        private ulong _outSize; // outbound message size
        private double _speed;  // byte/sec
        int _inMessageCount;    // number of inbound messages
        int _outMessageCount;   // number of outbound messages
        double _messageRate;    // average message rate
        string _status;         // status of the tranmission.
        #endregion

        #region public properties

        /// <summary>
        /// Sets/Gets number of bytes received in the transmission.
        /// </summary>
        public ulong InboundBytes
        {
            get { return _inSize; }
            set { _inSize = value; }
        }

        /// <summary>
        /// Sets/Gets number of bytes sent through the transmission.
        /// </summary>
        public ulong OutboundBytes
        {
            get { return _outSize; }
            set { _outSize = value; }
        }

        /// <summary>
        /// Sets/Gets total number of bytes sent and received through the transmission.
        /// </summary>
        public ulong TotalSizeInBytes
        {
            get
            {
                return InboundBytes + OutboundBytes;
            }
            set
            {
                this["@SizeInKB"] = string.Format("{0:0}", value / 1000);
            }
        }

        /// <summary>
        /// Sets/Gets average transmission speed.
        /// </summary>
        public double AverageSpeedInBytesPerSec
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                this["@AverageSpeedInKBPerSec"] = string.Format("{0:0.0}", value / 1000);
            }
        }

        /// <summary>
        /// Sets/Gets number of messages received through the transmission.
        /// </summary>
        public int InboundMessageCount
        {
            get
            {
                return _inMessageCount;
            }
            set
            {
                _inMessageCount = value;
                this["@InboundMessageCount"] = string.Format("{0}", value);
            }
        }

        /// <summary>
        /// Sets/Gets number of messages sent through the transmission.
        /// </summary>
        /// 
        public int OutboundMessageCount
        {
            get
            {
                return _outMessageCount;
            }
            set
            {
                _outMessageCount = value;
                this["@OutboundMessageCount"] = string.Format("{0}", value);
            }
        }

        /// <summary>
        /// Sets/Gets number of messages received/sent through the transmission.
        /// </summary>
        /// 
        public int TotalMessageCount
        {
            get
            {
                return InboundMessageCount + OutboundMessageCount;
            }
            set
            {
                this["@TotalMessageCount"] = string.Format("{0}", value);
            }
        }

        /// <summary>
        /// Sets/Gets average message rate for the transmission.
        /// </summary>
        /// 
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

        /// <summary>
        /// Sets/Gets status of the transmission.
        /// </summary>
        /// 
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
        /// <summary>
        /// Creates an instance of <seealso cref="TransmissionStatistics"/>.
        /// </summary>
        /// <param name="desc"></param>
        public TransmissionStatistics(string desc)
            : base(desc)
        {
        }

        #endregion

        #region overridden protected methods
        protected override void OnBegin()
        {
            base.OnBegin();

            TotalMessageCount = 0;
            InboundMessageCount = 0;
            OutboundMessageCount = 0;
            TotalSizeInBytes = 0;
            InboundBytes = 0;
            OutboundBytes = 0;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            TotalSizeInBytes = InboundBytes + OutboundBytes;
            TotalMessageCount = InboundMessageCount + OutboundMessageCount;

            // calculate the speed
            if (ElapsedTimeInMs <= 0)
            {
                AverageSpeedInBytesPerSec = 0;
                AverageMessageRate = 0;
            }
            else
            {
                double elapsedTimeInSec = ElapsedTimeInMs / 1000.0;

                AverageSpeedInBytesPerSec = TotalSizeInBytes / elapsedTimeInSec;
                AverageMessageRate = TotalMessageCount / elapsedTimeInSec;
            }
        }

        #endregion


    }
}
