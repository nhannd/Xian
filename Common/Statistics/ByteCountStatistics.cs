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

using System;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Statistics to store the number of bytes
    /// </summary>
    /// <remarks>
    /// <see cref="IStatistics.FormattedValue"/> of the <see cref="ByteCountStatistics"/> has unit of "GB", 'MB" or "KB"
    /// depending on the number of bytes being set.
    /// </remarks>
    public class ByteCountStatistics : Statistics<ulong>
    {
        private const double KILOBYTES = 1024;
        private const double MEGABYTES = 1024*KILOBYTES;
        private const double GIGABYTES = 1024*MEGABYTES;

        public ByteCountStatistics(string name)
            : this(name, 0)
        {
            
        }

        public ByteCountStatistics(string name, ulong value)
            : base(name, value)
        {
            ValueFormatter = delegate(ulong bytes)
                                 {
                                     if (bytes > GIGABYTES)
                                         return String.Format("{0:0.00} GB", bytes / GIGABYTES);
                                     if (bytes > MEGABYTES)
                                         return String.Format("{0:0.00} MB", bytes / MEGABYTES);
                                     if (bytes > KILOBYTES)
                                         return String.Format("{0:0.00} KB", bytes / KILOBYTES);

                                     return String.Format("{0} B", bytes);
                                 };
        }
    }
}
