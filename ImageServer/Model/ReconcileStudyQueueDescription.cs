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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Model
{
    class QueueSearchableDescriptionAttribute : Attribute
    {

    }

    public abstract class StudyIntegrityQueueDescription
    {
        private const String KEY_VALUE_SEPARATOR = "=";
        private readonly Dictionary<String, Object> _map = new Dictionary<string, object>();
        protected object this[string key]
        {
            get
            {
                if (!_map.ContainsKey(key))
                    return null;
                else
                    return _map[key];
            }
            set
            {
                if (_map.ContainsKey(key))
                    _map[key] = value;
                else
                    _map.Add(key, value);
            }
        }

        public void Parse(String text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            String[] lines = text.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String line in lines)
            {
                int index = line.IndexOf(KEY_VALUE_SEPARATOR);
                String key = line.Substring(0, index);
                String value = line.Substring(index + 1);
                this[key] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (AttributeUtils.HasAttribute<QueueSearchableDescriptionAttribute>(property))
                {
                    text.AppendFormat("{0}{1}{2}", property.Name, KEY_VALUE_SEPARATOR, property.GetValue(this, null));
                    text.Append(Environment.NewLine);
                }
            }

            return text.ToString();
        }

    }

   
    public class ReconcileStudyQueueDescription : StudyIntegrityQueueDescription
    {
        [QueueSearchableDescription]
        public String ExistingPatientName
        {
            get
            {
                return this["ExistingPatientName"] as String;
            }
            set
            {
                this["ExistingPatientName"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ExistingPatientId
        {
            get
            {
                return this["ExistingPatientId"] as String;
            }
            set
            {
                this["ExistingPatientId"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ConflictingPatientName
        {
            get
            {
                return this["ConflictingPatientName"] as String;
            }
            set
            {
                this["ConflictingPatientName"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ConflictingPatientId
        {
            get
            {
                return this["ConflictingPatientId"] as String;
            }
            set
            {
                this["ConflictingPatientId"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ExistingAccessionNumber
        {
            get
            {
                return this["ExistingAccessionNumber"] as String;
            }
            set
            {
                this["ExistingAccessionNumber"] = value;
            }
        }

        [QueueSearchableDescription]
        public String ConflictingAccessionNumber
        {
            get
            {
                return this["ConflictingAccessionNumber"] as String;
            }
            set
            {
                this["ConflictingAccessionNumber"] = value;
            }
        }

    }

}
