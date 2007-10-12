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
using System.IO;

using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Network
{
    internal class PDataTFStream : Stream
    {
        public delegate void TickDelegate(TransferMonitor stats);

        #region Private Members
        private bool _command;
        private int _max;
        private byte _pcid;
        private TransferMonitor _stats;
        private PDataTF _pdu;
        private byte[] _bytes;
        private int _total;
        private MemoryStream _buffer;
        private NetworkBase _networkBase;
        #endregion

        #region Public Constructors
        public PDataTFStream(NetworkBase networkBase, byte pcid, int max, int total)
        {
            _command = true;
            _pcid = pcid;
            _max = max;
            _stats = new TransferMonitor();
            _pdu = new PDataTF();
            _buffer = new MemoryStream(total + 1024);
            _total = total;
            _networkBase = networkBase;
        }
        #endregion

        #region Public Properties
        public TickDelegate OnTick;

        public TransferMonitor Stats
        {
            get { return _stats; }
        }

        public bool IsCommand
        {
            get { return _command; }
            set
            {
                CreatePDV();
                _command = value;
                WritePDU(true);
            }
        }
        #endregion

        #region Public Members
        public void Flush(bool last)
        {
            WritePDU(last);
            //_network.Flush();
        }
        #endregion

        #region Private Members
        private int CurrentPduSize()
        {
            return (int)_pdu.GetLengthOfPDVs();
        }

        private bool CreatePDV()
        {
            int len = Math.Min(GetBufferLength(), _max - (CurrentPduSize() + 6));

            //DicomLogger.LogInfo("Created PDV of length: {0}",len);
            if (_bytes == null || _bytes.Length != len || _pdu.PDVs.Count > 0)
            {
                _bytes = new byte[len];
            }
            _buffer.Read(_bytes, 0, len);

            PDV pdv = new PDV(_pcid, _bytes, _command, false);
            _pdu.PDVs.Add(pdv);

            return pdv.IsLastFragment;
        }

        private void WritePDU(bool last)
        {
            if (_pdu.PDVs.Count == 0 || ((CurrentPduSize() + 6) < _max && GetBufferLength() > 0))
            {
                CreatePDV();
            }
            if (_pdu.PDVs.Count > 0)
            {
                if (last)
                {
                    _pdu.PDVs[_pdu.PDVs.Count - 1].IsLastFragment = true;
                }
                RawPDU raw = _pdu.Write();

                _networkBase.EnqueuePDU(raw);
                Stats.Tick((int)_pdu.GetLengthOfPDVs() - (6 * _pdu.PDVs.Count), _total);
                if (OnTick != null)
                    OnTick(Stats);
                _pdu = new PDataTF();
            }
        }

        private void AppendBuffer(byte[] buffer, int offset, int count)
        {
            long pos = _buffer.Position;
            _buffer.Seek(0, SeekOrigin.End);
            _buffer.Write(buffer, offset, count);
            _buffer.Position = pos;
        }

        private int GetBufferLength()
        {
            return (int)(_buffer.Length - _buffer.Position);
        }
        #endregion

        #region Stream Members
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            //_network.Flush();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            AppendBuffer(buffer, offset, count);
            while ((CurrentPduSize() + 6 + GetBufferLength()) > _max)
            {
                WritePDU(false);
            }
        }
        #endregion
    }
}
