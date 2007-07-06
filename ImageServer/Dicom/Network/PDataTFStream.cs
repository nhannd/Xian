/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.ImageServer.Dicom.IO;

namespace ClearCanvas.ImageServer.Dicom.Network
{
    internal class PDataTFStream : Stream
    {
        public delegate void TickDelegate(TransferMonitor stats);

        #region Private Members
        private Stream _network;
        private bool _command;
        private int _max;
        private byte _pcid;
        private TransferMonitor _stats;
        private PDataTF _pdu;
        private byte[] _bytes;
        private int _total;
        private MemoryStream _buffer;
        #endregion

        #region Public Constructors
        public PDataTFStream(Stream network, byte pcid, int max, int total)
        {
            _network = network;
            _command = true;
            _pcid = pcid;
            _max = max;
            _stats = new TransferMonitor();
            _pdu = new PDataTF();
            _buffer = new MemoryStream(total + 1024);
            _total = total;
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
            _network.Flush();
        }
        #endregion

        #region Private Members
        private int CurrentPduSize()
        {
            return 6 + (int)_pdu.GetLengthOfPDVs();
        }

        private bool CreatePDV()
        {
            int len = Math.Min(GetBufferLength(), _max - CurrentPduSize() - 6);

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
                raw.WritePDU(_network);
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
