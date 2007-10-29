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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.EnumBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public class FilesystemTierEnum : ServerEnum
    {
        private static Dictionary<short, FilesystemTierEnum> _dict = new Dictionary<short, FilesystemTierEnum>();

        #region Constructors
        /// <summary>
        /// One-time load from the database of type enumerated value.
        /// </summary>
        static FilesystemTierEnum()
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IEnumBroker<FilesystemTierEnum> broker = read.GetBroker<IFilesystemTierEnum>();
                IList<FilesystemTierEnum> list = broker.Execute();
                foreach (FilesystemTierEnum type in list)
                {
                    _dict.Add(type.Enum, type);
                }
            }
        }

        public FilesystemTierEnum()
            : base("FilesystemTierEnum")
        {
        }
        #endregion

        public override void SetEnum(short val)
        {
            FilesystemTierEnum tierEnum;
            if (false == _dict.TryGetValue(val, out tierEnum))
                throw new PersistenceException("Unknown FilesystemTierEnum value: " + val, null);

            Enum = tierEnum.Enum;
            Lookup = tierEnum.Lookup;
            Description = tierEnum.Description;
            LongDescription = tierEnum.LongDescription;
        }

        public static FilesystemTierEnum GetEnum(string lookup)
        {
            foreach (FilesystemTierEnum type in _dict.Values)
            {
                if (type.Lookup.Equals(lookup))
                    return type;
            }
            throw new PersistenceException("Unknown FilesystemTierEnum: " + lookup, null);
        }
    }
}
