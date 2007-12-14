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


namespace ClearCanvas.Common.Performance.Network
{

    
    public class TransmissionStatistics : StatisticsSet
    {
        private TimeSpanStatistics _timeSpan = new TimeSpanStatistics("TimeSpan");

        public TransmissionStatistics(string name)
            : base(name)
        {
            AddField(new ByteCountStatistics("IncomingBytes"));
            AddField(new MessageCountStatistics("IncomingMessages"));
            AddField(new ByteCountStatistics("OutgoingBytes"));
            AddField(new MessageCountStatistics("OutgoingMessages"));

            AddField(new RateStatistics("Speed", RateType.BYTES));
            AddField(new RateStatistics("MessageRate", RateType.MESSAGES));
            
        }

        public RateStatistics Speed
        {
            get { return this["Speed"] as RateStatistics; }
            
        }

        public RateStatistics MessageRate
        {
            get { return this["MessageRate"] as RateStatistics; }
        }

        public long IncomingBytes
        {
            get
            {
                return (long)(this["IncomingBytes"] as ByteCountStatistics).Value;
            }
            set
            {
                this["IncomingBytes"].SetValue(value);
            }
        }

        public long IncomingMessages
        {
            get
            {
                return (long)(this["IncomingMessages"] as MessageCountStatistics).Value;
            }
            set
            {
                this["IncomingMessages"].SetValue(value);
            }
        }

        public long OutgoingBytes
        {
            get
            {
                return (long)(this["OutgoingBytes"] as ByteCountStatistics).Value;
            }
            set
            {
                this["OutgoingBytes"].SetValue(value);
            }
        }
        public long OutgoingMessages
        {
            get
            {
                return (long)(this["OutgoingMessages"] as MessageCountStatistics).Value;
            }
            set
            {
                this["OutgoingMessages"].SetValue(value);
            }
        }


        public void Begin()
        {
            MessageRate.Start();
            Speed.Start();
        }

        public void End()
        {
            Speed.SetData(IncomingBytes);
            Speed.End();

            MessageRate.SetData(IncomingMessages);
            MessageRate.End();
        }
    }
}
