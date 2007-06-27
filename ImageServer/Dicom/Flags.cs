using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom
{
    public static class Flags
    {
        public static bool IsSet(DicomDumpOptions options, DicomDumpOptions flag)
        {
            return (options & flag) == flag;
        }
        public static bool IsSet(DicomReadOptions options, DicomReadOptions flag)
        {
            return (options & flag) == flag;
        }
        public static bool IsSet(DicomWriteOptions options, DicomWriteOptions flag)
        {
            return (options & flag) == flag;
        }
    }

    [Flags]
    public enum DicomDumpOptions
    {
        None,
        ShortenLongValues,
        Restrict80CharactersPerLine,
        KeepGroupLengthElements,
        Default = DicomDumpOptions.ShortenLongValues | DicomDumpOptions.Restrict80CharactersPerLine
    }

    [Flags]
    public enum DicomReadOptions
    {
        None,
        KeepGroupLengths,
        UseDictionaryForExplicitUN,
        AllowSeekingForContext,
        Default = DicomReadOptions.UseDictionaryForExplicitUN | DicomReadOptions.AllowSeekingForContext
    }

    [Flags]
    public enum DicomWriteOptions
    {
        None,
        CalculateGroupLengths,
        ExplicitLengthSequence,
        ExplicitLengthSequenceItem,
        WriteFragmentOffsetTable,
        Default = DicomWriteOptions.CalculateGroupLengths | DicomWriteOptions.WriteFragmentOffsetTable
    }
}
