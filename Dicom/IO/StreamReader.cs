using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClearCanvas.Dicom.IO
{
    internal enum DicomReadStatus
    {
        Success,
        UnknownError,
        NeedMoreData
    }

    internal class DicomStreamReader
    {
        #region Private Classes
        /// <summary>
        /// Class used to keep track of recursion within sequences
        /// </summary>
        private struct SequenceRecord
        {
            public long _pos;
            public long _len;
            public DicomAttributeCollection _parent;
            public uint _tag;
            public DicomAttributeCollection _current;

            public long _curpos;
            public long _curlen;
        
        }
        #endregion

        #region Private Members
        private const uint UndefinedLength = 0xFFFFFFFF;

        private Stream _stream = null;
        private BinaryReader _reader = null;
        private TransferSyntax _syntax = null;
        private Endian _endian;

        private DicomAttributeCollection _dataset;

        private uint _privateCreatorCard = 0xffffffff;
        private string _privateCreatorId = String.Empty;

        private DicomTag _tag = null;
        private DicomVr _vr = null;
        private uint _len = UndefinedLength;
        private long _pos = 0;

        private long _bytes = 0;
        private long _read = 0;
        private uint _need = 0;
        private long _remain = 0;

        private Stack<SequenceRecord> _sqrs = new Stack<SequenceRecord>();

        private DicomAttributeOB _fragment = null;
        #endregion

        #region Public Constructors
        public DicomStreamReader(Stream stream)
        {
            _stream = stream;
            TransferSyntax = TransferSyntax.ExplicitVRLittleEndian;
        }
        #endregion

        #region Public Properties
        public TransferSyntax TransferSyntax
        {
            get { return _syntax; }
            set
            {
                _syntax = value;
                _endian = _syntax.Endian;
                _reader = EndianBinaryReader.Create(_stream, _endian);
            }
        }

        public DicomAttributeCollection Dataset
        {
            get { return _dataset; }
            set
            {
                _dataset = value;
            }
        }

        public long BytesEstimated
        {
            get { return _bytes + _need; }
        }

        public long BytesRead
        {
            get { return _read; }
        }

        public uint BytesNeeded
        {
            get { return _need; }
        }
        #endregion

        private DicomReadStatus NeedMoreData(long count)
        {
            _need = (uint)count;
            return DicomReadStatus.NeedMoreData;
        }

        public DicomReadStatus Read(DicomTag stopAtTag, DicomReadOptions options)
        {
            if (stopAtTag == null)
                stopAtTag = new DicomTag(0xFFFFFFFF, "Bogus Tag", DicomVr.UNvr, false, 1, 1, false);

            // Counters:
            //  _remain - bytes remaining in stream
            //  _bytes - estimates bytes to end of dataset
            //  _read - number of bytes read from stream
            try
            {
                _need = 0;
                _remain = _stream.Length - _stream.Position;

                while (_remain > 0)
                {
                    uint tagValue = 0;
                    if (_tag == null)
                    {
                        if (_remain >= 4)
                        {
                            _pos = _stream.Position;
                            ushort g = _reader.ReadUInt16();
                            ushort e = _reader.ReadUInt16();
                            tagValue = DicomTag.GetTagValue(g, e);
                            if (DicomTag.IsPrivateGroup(g) && e > 0x00ff)
                            {
                                if ((tagValue & _privateCreatorCard) != _privateCreatorCard)
                                {
                                    _privateCreatorCard = tagValue & 0xffffff00;
                                    DicomTag pct = DicomTag.GetPrivateCreatorTag(g, e);
                                    DicomAttributeCollection ds = _dataset;
                                    if (_sqrs.Count > 0)
                                    {
                                        ds = _sqrs.Peek()._current;
                                        if (!ds.Contains(pct)) // not sure about this TODO
                                            ds = _dataset;
                                    }
                                    _privateCreatorId = ds[pct].ToString();
                                }
                                _tag = DicomTagDictionary.GetDicomTag(g, e);
                                if (_tag == null)
                                    _tag = new DicomTag((uint)g << 16 | (uint)e, "Private Tag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
                            }
                            else
                            {
                                if (e == 0x0000)
                                    _tag = new DicomTag((uint)g << 16 | (uint)e, "Group Length", DicomVr.ULvr, false, 1, 1, false);
                                else
                                    _tag = DicomTagDictionary.GetDicomTag(g, e);

                                if (_tag == null)
                                    _tag = new DicomTag((uint)g << 16 | (uint)e, "Private Tag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
                            }
                            _remain -= 4;
                            _bytes += 4;
                            _read += 4;
                        }
                        else
                        {
                            return NeedMoreData(4);
                        }
                    }
                    else
                        tagValue = _tag.TagValue;

                    if ((tagValue >= stopAtTag.TagValue) && (_sqrs.Count == 0)) // only exit in root message when after stop tag
                        return DicomReadStatus.Success;

                    if (_vr == null)
                    {
                        if (_syntax.ExplicitVr)
                        {
                            if (_tag == DicomTag.Item ||
                                _tag == DicomTag.ItemDelimitationItem ||
                                _tag == DicomTag.SequenceDelimitationItem)
                            {
                                _vr = DicomVr.NONE;
                            }
                            else
                            {
                                if (_remain >= 2)
                                {
                                    _vr = DicomVr.GetVR(new String(_reader.ReadChars(2)));
                                    _remain -= 2;
                                    _bytes += 2;
                                    _read += 2;
                                    if (_tag.VR.Equals(DicomVr.UNvr))
                                        _tag = new DicomTag(_tag.TagValue, "Private Tag", _vr, false, 1, uint.MaxValue, false);
                                    else if (!_tag.VR.Equals(_vr))
                                    {
                                        DicomTag tag = new DicomTag(_tag.TagValue,_tag.Name,_vr,_tag.MultiVR,_tag.VMLow,_tag.VMHigh,_tag.Retired);
                                        _tag = tag;
                                        ; // TODO, log something
                                    }
                                }
                                else
                                {
                                    return NeedMoreData(2);
                                }
                            }
                        }
                        else
                        {
                            _vr = _tag.VR;
                        }

                        if (_vr == DicomVr.UNvr)
                        {
                            if (_tag.IsPrivate)
                            {
                                if (_tag.Element <= 0x00ff)
                                {
                                    // private creator id
                                    _vr = DicomVr.LOvr;
                                }
                                else if (_stream.CanSeek && Flags.IsSet(options, DicomReadOptions.AllowSeekingForContext))
                                {
                                    // attempt to identify private sequence
                                    long pos = _stream.Position;
                                    if (_syntax.ExplicitVr)
                                    {
                                        if (_remain >= 2)
                                            _reader.ReadUInt16();
                                        else
                                        {
                                            _vr = null;
                                            _stream.Position = pos;
                                            return NeedMoreData(2);
                                        }
                                    }

                                    uint l = 0;
                                    if (_remain >= 4)
                                    {
                                        l = _reader.ReadUInt32();
                                        if (l == UndefinedLength)
                                            _vr = DicomVr.SQvr;
                                    }
                                    else
                                    {
                                        _vr = null;
                                        _stream.Position = pos;
                                        return NeedMoreData(4);
                                    }

                                    if (l != 0 && _vr == DicomVr.UNvr)
                                    {
                                        if (_remain >= 4)
                                        {
                                            ushort g = _reader.ReadUInt16();
                                            ushort e = _reader.ReadUInt16();
                                            uint tempTagValue = DicomTag.GetTagValue(g, e);

                                            if (tempTagValue == DicomTag.Item.TagValue || tempTagValue == DicomTag.SequenceDelimitationItem.TagValue)
                                                _vr = DicomVr.SQvr;
                                        }
                                        else
                                        {
                                            _vr = null;
                                            _stream.Position = pos;
                                            return NeedMoreData(4);
                                        }
                                    }

                                    _stream.Position = pos;
                                }
                            }
                            else if (!_syntax.ExplicitVr || Flags.IsSet(options, DicomReadOptions.UseDictionaryForExplicitUN))
                                _vr = _tag.VR;
                        }
                    }

                    if (_len == UndefinedLength)
                    {
                        if (_syntax.ExplicitVr)
                        {
                            if (_tag == DicomTag.Item ||
                                _tag == DicomTag.ItemDelimitationItem ||
                                _tag == DicomTag.SequenceDelimitationItem)
                            {
                                if (_remain >= 4)
                                {
                                    _len = _reader.ReadUInt32();
                                    _remain -= 4;
                                    _bytes += 4;
                                    _read += 4;
                                }
                                else
                                {
                                    return NeedMoreData(4);
                                }
                            }
                            else
                            {
                                if (_vr.Is16BitLengthField)
                                {
                                    if (_remain >= 2)
                                    {
                                        _len = (uint)_reader.ReadUInt16();
                                        _remain -= 2;
                                        _bytes += 2;
                                        _read += 2;
                                    }
                                    else
                                    {
                                        return NeedMoreData(2);
                                    }
                                }
                                else
                                {
                                    if (_remain >= 6)
                                    {
                                        _reader.ReadByte();
                                        _reader.ReadByte();
                                        _len = _reader.ReadUInt32();
                                        _remain -= 6;
                                        _bytes += 6;
                                        _read += 6;
                                    }
                                    else
                                    {
                                        return NeedMoreData(6);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_remain >= 4)
                            {
                                _len = _reader.ReadUInt32();
                                _remain -= 4;
                                _bytes += 4;
                                _read += 4;
                            }
                            else
                            {
                                return NeedMoreData(4);
                            }
                        }

                        if (_len != UndefinedLength)
                        {
                            if (_vr != DicomVr.SQvr && !(_tag.Equals(DicomTag.Item) && _fragment == null))
                                _bytes += _len;
                        }
                    }

                    if (_fragment != null)
                    {
                        // In the middle of parsing pixels
                        if (_tag == DicomTag.Item)
                        {
                            if (_remain >= _len)
                            {
                                ByteBuffer data = new ByteBuffer(_endian);
                                data.CopyFrom(_stream, (int)_len);
                                _remain -= _len;
                                _read += _len;

                                if (!_fragment.HasOffsetTable)
                                    _fragment.SetOffsetTable(data);
                                else
                                    _fragment.AddFragment(data);

                            }
                            else
                            {
                                return NeedMoreData(_remain - _len);
                            }
                        }
                        else if (_tag == DicomTag.SequenceDelimitationItem)
                        {
                            _dataset[_fragment.Tag] = _fragment;
                            _fragment = null;
                        }
                        else
                        {
                            DicomLogger.LogError("Encountered unexpected tag in stream: " + _tag.ToString());
                            // unexpected tag
                            return DicomReadStatus.UnknownError;
                        }

                    }
                    else if (_sqrs.Count > 0 &&
                                (_tag == DicomTag.Item ||
                                _tag == DicomTag.ItemDelimitationItem ||
                                _tag == DicomTag.SequenceDelimitationItem))
                    {
                        SequenceRecord rec = _sqrs.Peek();

                        if (_tag.Equals(DicomTag.Item))
                        {
                            if (_len != UndefinedLength)
                            {
                                if (_len > _remain)
                                    return NeedMoreData(_remain - _len);
                            }

                            DicomSequenceItem ds = new DicomSequenceItem();

                            rec._current = ds;

                            // Do a lookup to see if the parent SQ attribute doesn't exist in the
                            //dictionary, this prevents an exception being thrown by the _parent
                            // indexer, and allows us to add the tag
                            DicomTag dicomTag = DicomTagDictionary.GetDicomTag(rec._tag);
                            if (dicomTag == null)
                                dicomTag = new DicomTag(rec._tag, "Unknown SQ Attribute", DicomVr.SQvr, false, 0, uint.MaxValue, false);

                            rec._parent[dicomTag].AddSequenceItem(ds);

                            // Specific character set is inherited, save it.  It will be overwritten
                            // if a new value of the tag is encountered in the sequence.
                            rec._current.SpecificCharacterSet = rec._parent.SpecificCharacterSet;

                            // save the sequence length
                            rec._curpos = _pos + 8;
                            rec._curlen = _len;

                            _sqrs.Pop();
                            _sqrs.Push(rec);

                            if (_len != UndefinedLength)
                            {
                                ByteBuffer data = new ByteBuffer(_endian);
                                data.CopyFrom(_stream, (int)_len);
                                _remain -= _len;
                                _read += _len;

                                DicomStreamReader idsr = new DicomStreamReader(data.Stream);
                                idsr.Dataset = ds;
                                idsr._syntax = this._syntax;
                                DicomReadStatus stat = idsr.Read(null, options);
                                if (stat == DicomReadStatus.UnknownError)
                                {
                                    DicomLogger.LogError("Unexpected parsing error when reading sequence attribute: {0}.",dicomTag.ToString());
                                    return stat;
                                }

                            }
                            else
                            {
                                
                            }

                        }
                        else if (_tag == DicomTag.ItemDelimitationItem)
                        {
                        }
                        else if (_tag == DicomTag.SequenceDelimitationItem)
                        {
                            _sqrs.Pop();
                        }

                        if (rec._len != UndefinedLength)
                        {
                            long end = rec._pos + 8 + rec._len;
                            if (_syntax.ExplicitVr)
                                end += 2 + 2;
                            if (_stream.Position >= end)
                            {
                                _sqrs.Pop();
                            }
                        }

                    }
                    else
                    {
                        if (_len == UndefinedLength)
                        {
                            if (_vr == DicomVr.SQvr)
                            {
                                SequenceRecord rec = new SequenceRecord();
                                if (_sqrs.Count > 0)
                                    rec._parent = _sqrs.Peek()._current;
                                else
                                    rec._parent = _dataset;
                                rec._current = null;
                                rec._tag = _tag.TagValue;
                                rec._len = UndefinedLength;
                                
                                _sqrs.Push(rec);
                            }
                            else
                            {
                                _fragment = new DicomAttributeOB(_tag);
                            }
                        }
                        else
                        {
                            if (_vr == DicomVr.SQvr)
                            {
                                // Zero length sequences should not be saved, they're just ignored.
                                if (_len == 0)
                                {
                                    DicomAttributeCollection ds;
                                    if (_sqrs.Count > 0)
                                    {
                                        SequenceRecord rec = _sqrs.Peek();
                                        ds = rec._current;
                                    }
                                    else
                                        ds = _dataset;

                                    DicomAttribute elem = _tag.CreateDicomAttribute();

                                    elem.Values = new DicomSequenceItem[0];

                                    ds[_tag] = elem;
                                }
                                else
                                {
                                    SequenceRecord rec = new SequenceRecord();
                                    rec._len = _len;
                                    rec._pos = _pos;
                                    rec._tag = _tag.TagValue;
                                    if (_sqrs.Count > 0)
                                        rec._parent = _sqrs.Peek()._current;
                                    else
                                        rec._parent = _dataset;

                                    _sqrs.Push(rec);
                                }
                            }
                            else
                            {
                                if (_remain >= _len)
                                {
                                    ByteBuffer bb = new ByteBuffer();
                                    // If the tag is impacted by specific character set, 
                                    // set the encoding properly.
                                    if (_tag.VR.SpecificCharacterSet)
                                    {
                                        if (_sqrs.Count > 0)
                                        {
                                            SequenceRecord rec = _sqrs.Peek();
                                            bb.SpecificCharacterSet = rec._current.SpecificCharacterSet;
                                        }
                                        else
                                        {
                                            bb.SpecificCharacterSet = _dataset.SpecificCharacterSet;
                                        }
                                    }

                                    bb.Endian = _endian;
                                    bb.CopyFrom(_stream, (int)_len);

                                    DicomAttribute elem = _tag.CreateDicomAttribute(bb);


                                    _remain -= _len;
                                    _read += _len;

                                    if (_sqrs.Count > 0)
                                    {
                                        SequenceRecord rec = _sqrs.Peek();
                                        DicomAttributeCollection ds = rec._current;

                                        if (elem.Tag.TagValue == DicomTags.SpecificCharacterSet)
                                        {
                                            ds.SpecificCharacterSet = elem.ToString();
                                        }

                                        if (_tag.Element == 0x0000)
                                        {
                                            if (Flags.IsSet(options, DicomReadOptions.KeepGroupLengths))
                                                ds[_tag] = elem;
                                        }
                                        else
                                            ds[_tag] = elem;

                                        if (rec._curlen != UndefinedLength)
                                        {
                                            long end = rec._curpos + rec._curlen;
                                            if (_stream.Position >= end)
                                            {
                                                rec._current = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (elem.Tag.TagValue == DicomTags.SpecificCharacterSet)
                                        {
                                            _dataset.SpecificCharacterSet = elem.ToString();
                                        }

                                        if (_tag.Element == 0x0000)
                                        {
                                            if (Flags.IsSet(options, DicomReadOptions.KeepGroupLengths))
                                                _dataset[_tag] = elem;
                                        }
                                        else
                                            _dataset[_tag] = elem;
                                    }
                                }
                                else
                                {
                                    return NeedMoreData(_len - _remain);
                                }
                            }
                        }
                    }

                    _tag = null;
                    _vr = null;
                    _len = UndefinedLength;
                }
                return DicomReadStatus.Success;
            }
            catch (EndOfStreamException e)
            {
                // should never happen
                DicomLogger.LogError("Unexpected exception when reading file: " + e.ToString());
                return DicomReadStatus.UnknownError;
            }
        }
    }
}
