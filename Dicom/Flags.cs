/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
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
        None = 0,
        ShortenLongValues = 1,
        Restrict80CharactersPerLine = 2,
        KeepGroupLengthElements = 4,
        Default = DicomDumpOptions.ShortenLongValues | DicomDumpOptions.Restrict80CharactersPerLine
    }

    [Flags]
    public enum DicomReadOptions
    {
        None = 0,
        KeepGroupLengths = 1,
        UseDictionaryForExplicitUN = 2,
        AllowSeekingForContext = 4,
        Default = DicomReadOptions.UseDictionaryForExplicitUN | DicomReadOptions.AllowSeekingForContext
    }

    [Flags]
    public enum DicomWriteOptions
    {
        None = 0,
        CalculateGroupLengths = 1,
        ExplicitLengthSequence = 2,
        ExplicitLengthSequenceItem = 4,
        WriteFragmentOffsetTable = 8,
        Default = DicomWriteOptions.CalculateGroupLengths | DicomWriteOptions.WriteFragmentOffsetTable
    }
}
