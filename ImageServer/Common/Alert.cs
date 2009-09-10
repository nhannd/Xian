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
using System.Runtime.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// List of predefined alert type codes
    /// </summary>
    public class AlertTypeCodes
    {
        public const int Starting = 0;
        public const int Started = 1;
        public const int UnableToStart = 2;
        public const int Stopping = 10;
        public const int Stopped = 11;
        public const int UnableToStop = 12;

        public const int NoPermission = 13;

        public const int InvalidConfiguration = 20;

        public const int UnableToProcess = 30;

        public const int NoResources = 40;
        public const int LowResources = 41;


    }

    /// <summary>
    /// Represents the category of an <see cref="Alert"/>
    /// </summary>
    public enum AlertCategory
    {
        /// <summary>
        /// System alert
        /// </summary>
        System,

        /// <summary>
        /// Application alert
        /// </summary>
        Application,

        /// <summary>
        /// Security alert
        /// </summary>
        Security,

        /// <summary>
        /// User alert
        /// </summary>
        User,

        Unknown
    }

    /// <summary>
    /// Represents the level of an <see cref="Alert"/>
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// Alerts carrying information
        /// </summary>
        Informational,

        /// <summary>
        /// Alerts carrying warning message
        /// </summary>
        Warning,

        /// <summary>
        /// Alerts carrying error message
        /// </summary>
        Error,

        /// <summary>
        /// Alerts carrying critical information message
        /// </summary>
        Critical
    }

    /// <summary>
    /// Represents the source of an <see cref="Alert"/>
    /// </summary>
    [DataContract]
    public class AlertSource : IEquatable<AlertSource>
    {
        #region Private Members
        private string _name;
        private string _host;
        #endregion

        #region Constructors

        public AlertSource()
        {
        }


        /// <summary>
        /// Creates an instance of <see cref="Alert"/> for the specified source.
        /// </summary>
        /// <param name="name">Name of the source associated with alerts</param>
        public AlertSource(string name)
        {
            _name = name;
            _host = ServerPlatform.ServerInstanceId;
        }

        #endregion

        [DataMember]
        /// <summary>
        /// Name of the source.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets a string that represents the host machine of the source
        /// </summary>
        [DataMember]
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        #region IEquatable<AlertSource> Members

        public bool Equals(AlertSource other)
        {
            return Name == other.Name && Host == other.Host;
        }

        #endregion
    }

    /// <summary>
    /// Represents an alert.
    /// </summary>
    [DataContract]
    public class Alert : IEquatable<Alert>
    {
        #region Public Static Properties
        /// <summary>
        /// 'Null' alert.
        /// </summary>
        #endregion

        #region Private Members
        private AlertSource _source;
        private AlertCategory _category;
        private AlertLevel _level;
        private DateTime _timeStamp;
        private DateTime _expirationTime;
        private int _code;
        private String _message;
        private object _data;

        #endregion

        #region Constructors

        public Alert()
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="Alert"/> of specified category and level for the specified source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="category"></param>
        /// <param name="level"></param>
        public Alert(AlertSource source, AlertCategory category, AlertLevel level)
        {
            _source = source;
            _timeStamp = Platform.Time;
            _expirationTime = Platform.Time; 
            _category = category;
            _level = level;
            _code = 0;
            _data = null;
            _message = null;
        }

        #endregion

        /// <summary>
        /// Sets or gets the source of the alert.
        /// </summary>
        [DataMember]
        public AlertSource Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Gets the timestamp when the alert was created
        /// </summary>
        [DataMember]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }



        /// <summary>
        /// Gets or sets the data associated with the alert
        /// </summary>
        [DataMember]
        public object ContextData
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Gets or sets the alert category
        /// </summary>
        [DataMember]
        public AlertCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// Gets or sets the alert level
        /// </summary>
        [DataMember]
        public AlertLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        /// Gets or sets the alert code
        /// </summary>
        /// <remarks>
        /// <seealso cref="AlertTypeCodes"/> for predefined codes
        /// </remarks>
        [DataMember]
        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Gets or sets the expiration time of the alert.
        /// </summary>
        [DataMember]
        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { _expirationTime = value; }
        }

        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #region IEquatable<Alert> Members

        public bool Equals(Alert other)
        {
            if (! (Source.Equals(other.Source) &&
                   Category.Equals(other.Category) &&
                   Level.Equals(other.Level) &&
                   Code.Equals(other.Code)))
            return false;

            if (ContextData == null)
                return other.ContextData == null;
            
            return ContextData.Equals(other.ContextData);
        }

        #endregion
    }
}