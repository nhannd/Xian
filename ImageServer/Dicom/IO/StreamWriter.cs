/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClearCanvas.ImageServer.Dicom.IO
{
    internal enum DicomWriteStatus
    {
        Success,
        UnknownError
    }

    internal class DicomStreamWriter
    {
        #region Private Members
        private const uint UndefinedLength = 0xFFFFFFFF;

        private Stream _stream = null;
        private BinaryWriter _writer = null;
        private TransferSyntax _syntax = null;
        private Endian _endian;

        private ushort _group = 0xffff;
        #endregion

        #region Public Constructors
        public DicomStreamWriter(Stream stream)
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
                if (_endian != _syntax.Endian || _writer == null)
                {
                    _endian = _syntax.Endian;
                    _writer = EndianBinaryWriter.Create(_stream, _endian);
                }
            }
        }

        #endregion

        public DicomWriteStatus Write(TransferSyntax syntax, DicomAttributeCollection dataset, DicomWriteOptions options)
        {
            TransferSyntax = syntax;

            foreach (DicomAttribute item in dataset)
            {
                if (item.Tag.Element == 0x0000)
                    continue;

                if (Flags.IsSet(options, DicomWriteOptions.CalculateGroupLengths)
                    && item.Tag.Group != _group && item.Tag.Group <= 0x7fe0)
                {
                    _group = item.Tag.Group;
                    _writer.Write((ushort)_group);
                    _writer.Write((ushort)0x0000);
                    if (_syntax.ExplicitVr)
                    {
                        _writer.Write((byte)'U');
                        _writer.Write((byte)'L');
                        _writer.Write((ushort)4);
                    }
                    else
                    {
                        _writer.Write((uint)4);
                    }
                    _writer.Write((uint)dataset.CalculateGroupWriteLength(_group, _syntax, options));
                }

                _writer.Write((ushort)item.Tag.Group);
                _writer.Write((ushort)item.Tag.Element);

                if (_syntax.ExplicitVr)
                {
                    _writer.Write((byte)item.Tag.VR.Name[0]);
                    _writer.Write((byte)item.Tag.VR.Name[1]);
                }

                if (item is DicomAttributeSQ)
                {
                    DicomAttributeSQ sq = item as DicomAttributeSQ;

                    if (_syntax.ExplicitVr)
                        _writer.Write((ushort)0x0000);

                    if (Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
                    {
                        int hl = _syntax.ExplicitVr ? 12 : 8;
                        _writer.Write((uint)sq.CalculateWriteLength(_syntax, options & ~DicomWriteOptions.CalculateGroupLengths) - (uint)hl);
                    }
                    else
                    {
                        _writer.Write((uint)UndefinedLength);
                    }

                    foreach (DicomSequenceItem ids in item.Values as DicomSequenceItem[])
                    {
                        _writer.Write((ushort)DicomTag.Item.Group);
                        _writer.Write((ushort)DicomTag.Item.Element);

                        if (Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
                        {
                            _writer.Write((uint)ids.CalculateWriteLength(_syntax, options & ~DicomWriteOptions.CalculateGroupLengths));
                        }
                        else
                        {
                            _writer.Write((uint)UndefinedLength);
                        }

                        Write(this.TransferSyntax, ids, options & ~DicomWriteOptions.CalculateGroupLengths);

                        if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
                        {
                            _writer.Write((ushort)DicomTag.ItemDelimitationItem.Group);
                            _writer.Write((ushort)DicomTag.ItemDelimitationItem.Element);
                            _writer.Write((uint)0x00000000);
                        }
                    }

                    if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
                    {
                        _writer.Write((ushort)DicomTag.SequenceDelimitationItem.Group);
                        _writer.Write((ushort)DicomTag.SequenceDelimitationItem.Element);
                        _writer.Write((uint)0x00000000);
                    }
                }

                else if (item is DicomAttributeOB && _syntax.Encapsulated)
                {
                    DicomAttributeOB fs = item as DicomAttributeOB;

                    if (_syntax.ExplicitVr)
                        _writer.Write((ushort)0x0000);
                    _writer.Write((uint)UndefinedLength);

                    _writer.Write((ushort)DicomTag.Item.Group);
                    _writer.Write((ushort)DicomTag.Item.Element);

                    if (Flags.IsSet(options, DicomWriteOptions.WriteFragmentOffsetTable) && fs.HasOffsetTable)
                    {
                        _writer.Write((uint)fs.OffsetTableBuffer.Length);
                        fs.OffsetTableBuffer.CopyTo(_writer.BaseStream);
                    }
                    else
                    {
                        _writer.Write((uint)0x00000000);
                    }

                    foreach (ByteBuffer bb in fs.Fragments)
                    {
                        _writer.Write((ushort)DicomTag.Item.Group);
                        _writer.Write((ushort)DicomTag.Item.Element);
                        _writer.Write((uint)bb.Length);
                        bb.CopyTo(_writer.BaseStream);
                    }

                    _writer.Write((ushort)DicomTag.SequenceDelimitationItem.Group);
                    _writer.Write((ushort)DicomTag.SequenceDelimitationItem.Element);
                    _writer.Write((uint)0x00000000);
                }

                else
                {
                    DicomAttribute de = item as DicomAttribute;

                    if (_syntax.ExplicitVr)
                    {
                        if (de.Tag.VR.Is16BitLengthField)
                        {
                            _writer.Write((ushort)de.StreamLength);
                        }
                        else
                        {
                            _writer.Write((ushort)0x0000);
                            _writer.Write((uint)de.StreamLength);
                        }
                    }
                    else
                    {
                        _writer.Write((uint)de.StreamLength);
                    }

                    de.GetByteBuffer(_syntax).CopyTo(_writer.BaseStream);
                }
            }

            return DicomWriteStatus.Success;
        }
    }
}
