#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Basic Film Box Presentation and Relationship Module as per Part 3, C.13-3 (pg 862) and C.13.4 (pg 869)
    /// </summary>
    public class BasicFilmBoxModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FilmBoxModuleIod"/> class.
        /// </summary>
        public BasicFilmBoxModuleIod()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilmBoxModuleIod"/> class.
        /// </summary>
        public BasicFilmBoxModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Type of image display format. Enumerated Values:
        /// <para>
        /// STANDARD\C,R : film contains equal size rectangular image boxes with R rows of image boxes and C columns of image boxes; C and R are integers.
        /// </para>
        /// <para>
        /// ROW\R1,R2,R3, etc. : film contains rows with equal size rectangular image boxes with R1 image boxes in the first row, R2 image boxes in second row, 
        /// R3 image boxes in third row, etc.; R1, R2, R3, etc. are integers.
        /// </para>
        /// <para>
        /// COL\C1,C2,C3, etc.: film contains columns with equal size rectangular image boxes with C1 image boxes in the first column, C2 image boxes in second
        ///  column, C3 image boxes in third column, etc.; C1, C2, C3, etc. are integers.
        /// </para>
        /// <para>
        /// SLIDE : film contains 35mm slides; the number of slides for a particular film size is configuration dependent.
        /// </para>
        /// <para>
        /// SUPERSLIDE : film contains 40mm slides; the number of slides for a particular film size is configuration dependent.
        /// </para>
        /// <para>
        /// CUSTOM\i : film contains a customized ordering of rectangular image boxes; i identifies the image display format; the definition of the image display
        /// formats is defined in the Conformance Statement; i is an integer.
        /// </para>
        /// </summary>
        /// <value></value>
        public ImageDisplayFormat ImageDisplayFormat
        {
            get { return new ImageDisplayFormat(base.DicomAttributeProvider[DicomTags.ImageDisplayFormat].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.ImageDisplayFormat].SetStringValue(value.DicomString); }
        }

        /// <summary>
        /// Identification of annotation display format. The definition of the annotation display formats and the
        /// annotation box position sequence are defined in the Conformance Statement.
        /// </summary>
        /// <value>The annotation display format id.</value>
        public string AnnotationDisplayFormatId
        {
            get { return base.DicomAttributeProvider[DicomTags.AnnotationDisplayFormatId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.AnnotationDisplayFormatId].SetString(0, value); }
        }


        /// <summary>
        /// Gets or sets the film orientation.
        /// </summary>
        /// <value>The film orientation.</value>
        public FilmOrientation FilmOrientation
        {
            get { return IodBase.ParseEnum<FilmOrientation>(base.DicomAttributeProvider[DicomTags.FilmOrientation].GetString(0, String.Empty), FilmOrientation.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.FilmOrientation], value, false); }
        }

        /// <summary>
        /// Gets or sets the film size id.
        /// </summary>
        /// <value>The film size id.</value>
        public FilmSize FilmSizeId
        {
            get { return new FilmSize(base.DicomAttributeProvider[DicomTags.FilmSizeId].GetString(0, String.Empty)); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.FilmSizeId], value.DicomString, false); }
        }

        /// <summary>
        /// Gets or sets the type of the magnification.Interpolation type by which the printer magnifies or decimates the image in order to fit the image in the
        /// image box on film.
        /// </summary>
        /// <value>The type of the magnification.</value>
        public MagnificationType MagnificationType
        {
            get { return IodBase.ParseEnum<MagnificationType>(base.DicomAttributeProvider[DicomTags.MagnificationType].GetString(0, String.Empty), MagnificationType.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MagnificationType], value, false); }
        }

        /// <summary>
        /// Gets or sets the type of the smoothing.  Further specifies the type of the interpolation function. Values are defined in Conformance Statement.
        /// </summary>
        /// <value>The type of the smoothing.</value>
        public SmoothingType SmoothingType
        {
            get { return IodBase.ParseEnum<SmoothingType>(base.DicomAttributeProvider[DicomTags.SmoothingType].GetString(0, String.Empty), SmoothingType.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.SmoothingType], value, false); }
        }

        /// <summary>
        /// Gets or sets the border density.  Density of the film areas surrounding and between images on the film. Defined Terms: 
        /// <para>BLACK 
        /// </para>
        /// <para>
        /// WHITE 
        /// </para>
        /// <para>
        /// i where i represents the desired density in hundreds of OD
        /// </para>
        /// </summary>
        /// <value>The border density.</value>
        public string BorderDensity
        {
            get { return base.DicomAttributeProvider[DicomTags.BorderDensity].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.BorderDensity].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the empty image density.  Density of the image box area on the film that contains no image. Defined Terms: 
        /// <para>BLACK 
        /// </para>
        /// <para>
        /// WHITE 
        /// </para>
        /// <para>
        /// i where i represents the desired density in hundreds of OD
        /// </para>
        /// </summary>
        /// <value>The empty image density.</value>
        public string EmptyImageDensity
        {
            get { return base.DicomAttributeProvider[DicomTags.EmptyImageDensity].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.EmptyImageDensity].SetString(0, value); }
        }


        /// <summary>
        /// Gets or sets the min density.  Minimum density of the images on the film, expressed in hundredths of OD. If Min Density is lower than minimum printer density than Min Density 
        /// is set to minimum printer density.
        /// </summary>
        /// <value>The min density.</value>
        public ushort MinDensity
        {
            get { return base.DicomAttributeProvider[DicomTags.MinDensity].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.MinDensity].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the min density.  Maximum density of the images on the film, expressed in hundredths of OD. If Max Density higher than maximum printer density than Max 
        /// Density is set to maximum printer density.
        /// </summary>
        /// <value>The min density.</value>
        public ushort MaxDensity
        {
            get { return base.DicomAttributeProvider[DicomTags.MaxDensity].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.MaxDensity].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the trim, YES OR NO.
        /// </summary>
        /// <value>The trim.</value>
        public DicomBoolean Trim
        {
            get { return IodBase.ParseEnum<DicomBoolean>(base.DicomAttributeProvider[DicomTags.Trim].GetString(0, String.Empty), DicomBoolean.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.Trim], value, false); }
        }

        /// <summary>
        /// Gets or sets the configuration information.
        /// </summary>
        /// <value>The configuration information.</value>
        public string ConfigurationInformation
        {
            get { return base.DicomAttributeProvider[DicomTags.ConfigurationInformation].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ConfigurationInformation].SetStringValue(value); }
        }

        /// <summary>
        /// Gets or sets the illumination.  Luminance of lightbox illuminating a piece of transmissive film, or for the case of reflective media, luminance obtainable from diffuse reflection of the illumination present. Expressed as L0, in candelas per square meter (cd/m2).
        /// </summary>
        /// <value>The illumination.</value>
        public ushort Illumination
        {
            get { return base.DicomAttributeProvider[DicomTags.Illumination].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.Illumination].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the reflected ambient light.  For transmissive film, luminance contribution due to reflected ambient light. Expressed as La, in candelas per square meter (cd/m2).
        /// </summary>
        /// <value>The reflected ambient light.</value>
        public ushort ReflectedAmbientLight
        {
            get { return base.DicomAttributeProvider[DicomTags.ReflectedAmbientLight].GetUInt16(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.ReflectedAmbientLight].SetUInt16(0, value); }
        }

        /// <summary>
        /// Gets or sets the requested resolution id.  Specifies the resolution at which images in this Film Box are to be printed.
        /// </summary>
        /// <value>The requested resolution id.</value>
        public RequestedResolution RequestedResolutionId
        {
            get { return IodBase.ParseEnum<RequestedResolution>(base.DicomAttributeProvider[DicomTags.RequestedResolutionId].GetString(0, String.Empty), RequestedResolution.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.RequestedResolutionId], value, false); }
        }

        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedFilmSessionSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedFilmSessionSequence] as DicomAttributeSQ);
            }
        }

        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedImageBoxSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedImageBoxSequence] as DicomAttributeSQ);
            }
        }

        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedBasicAnnotationBoxSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedBasicAnnotationBoxSequence] as DicomAttributeSQ);
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the commonly used tags in the base dicom attribute collection.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(base.DicomAttributeProvider);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the commonly used tags in the specified dicom attribute collection.
        /// </summary>
        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
            if (dicomAttributeProvider == null)
                throw new ArgumentNullException("dicomAttributeProvider");

            //dicomAttributeProvider[DicomTags.NumberOfCopies].SetNullValue();
            //dicomAttributeProvider[DicomTags.PrintPriority].SetNullValue();
            //dicomAttributeProvider[DicomTags.MediumType].SetNullValue();
            //dicomAttributeProvider[DicomTags.FilmDestination].SetNullValue();
            //dicomAttributeProvider[DicomTags.FilmSessionLabel].SetNullValue();
            //dicomAttributeProvider[DicomTags.MemoryAllocation].SetNullValue();
            //dicomAttributeProvider[DicomTags.OwnerId].SetNullValue();
        }
        #endregion
    }

    #region FilmOrientation Enum
    /// <summary>
    /// enumeration for the Film Orientation
    /// </summary>
    [TypeConverter(typeof(BasicPrintEnumConverter<FilmOrientation>))]
    public enum FilmOrientation
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// vertical film position
        /// </summary>
        Portrait,
        /// <summary>
        /// horizontal film position
        /// </summary>
        Landscape
    }
    #endregion

    #region MagnificationType Enum
    /// <summary>
    /// Magnification type enum.  Interpolation type by which the printer magnifies or decimates the image in order to fit the image in the
    /// image box on film.
    /// </summary>
    [TypeConverter(typeof(BasicPrintEnumConverter<MagnificationType>))]
    public enum MagnificationType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Replicate,
        /// <summary>
        /// 
        /// </summary>
        Bilinear,
        /// <summary>
        /// 
        /// </summary>
        Cubic
    }
    #endregion

    #region SmoothingType Enum
    /// <summary>
    /// Further specifies the type of the interpolation function. Values are defined in Conformance Statement.
    /// </summary>
    [TypeConverter(typeof(BasicPrintEnumConverter<SmoothingType>))]
    public enum SmoothingType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// Only valid for Magnification Type
        /// </summary>
        Cubic
    }
    #endregion

    #region DicomBoolean Enum
    public enum DicomBoolean
    {
        None,
        Yes,
        No
    }
    #endregion

    #region RequestedResolution Enum
    /// <summary>
    /// Specifies the resolution at which images in this Film Box are to be printed.
    /// </summary>
    [TypeConverter(typeof(BasicPrintEnumConverter<RequestedResolution>))]
    public enum RequestedResolution
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// approximately 4k x 5k printable pixels on a 14 x 17 inch film
        /// </summary>
        Standard,
        /// <summary>
        /// Approximately twice the resolution of STANDARD.
        /// </summary>
        High
    }
    #endregion

    #region ImageDisplayFormat class
    [TypeConverter(typeof(ImageDisplayFormat.DisplayValueConverter))]
    public class ImageDisplayFormat
    {
        public class DisplayValueConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value is ImageDisplayFormat && destinationType == typeof(string))
                    return GetDisplayString(value as ImageDisplayFormat);

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public string GetDisplayString(ImageDisplayFormat idf)
            {
                var baseTypeProperCase = GetProperCasing(idf.BaseType.ToString());
                switch (idf.BaseType)
                {
                    case FormatType.STANDARD:
                        return string.Format("{0} {1}x{2}",
                            baseTypeProperCase,
                            idf.TypeModifiers[0],
                            idf.TypeModifiers[1]);

                    case FormatType.ROW:
                        return string.Format("{0} {1}",
                            baseTypeProperCase,
                            StringUtilities.Combine(idf.TypeModifiers, ", "));

                    case FormatType.COL:
                        return string.Format("{0} {1}",
                            "Column",
                            StringUtilities.Combine(idf.TypeModifiers, ", "));

                    case FormatType.SLIDE:
                    case FormatType.SUPERSLIDE:
                    case FormatType.CUSTOM:
                    default:
                        return baseTypeProperCase;
                }
            }

            private static string GetProperCasing(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                if (input.Length == 1)
                    return input.ToUpper();

                return string.Format("{0}{1}",
                    input[0].ToString().ToUpper(),
                    input.Substring(1).ToLower());
            }
        }

        /// <summary>
        /// Type of image display format. Enumerated Values:
        /// <para>
        /// STANDARD\C,R : film contains equal size rectangular image boxes with R rows of image boxes and C columns of image boxes; C and R are integers.
        /// </para>
        /// <para>
        /// ROW\R1,R2,R3, etc. : film contains rows with equal size rectangular image boxes with R1 image boxes in the first row, R2 image boxes in second row, 
        /// R3 image boxes in third row, etc.; R1, R2, R3, etc. are integers.
        /// </para>
        /// <para>
        /// COL\C1,C2,C3, etc.: film contains columns with equal size rectangular image boxes with C1 image boxes in the first column, C2 image boxes in second
        ///  column, C3 image boxes in third column, etc.; C1, C2, C3, etc. are integers.
        /// </para>
        /// <para>
        /// SLIDE : film contains 35mm slides; the number of slides for a particular film size is configuration dependent.
        /// </para>
        /// <para>
        /// SUPERSLIDE : film contains 40mm slides; the number of slides for a particular film size is configuration dependent.
        /// </para>
        /// <para>
        /// CUSTOM\i : film contains a customized ordering of rectangular image boxes; i identifies the image display format; the definition of the image display
        /// formats is defined in the Conformance Statement; i is an integer.
        /// </para>
        /// </summary>
        /// <value></value>
        public enum FormatType
        {
            STANDARD,
            ROW,
            COL,
            SLIDE,
            SUPERSLIDE,
            CUSTOM
        }

        public static List<ImageDisplayFormat> Choices = new List<ImageDisplayFormat>
            {
                new ImageDisplayFormat(FormatType.STANDARD,1,1),
                new ImageDisplayFormat(FormatType.STANDARD,1,2),
                new ImageDisplayFormat(FormatType.STANDARD,2,1),
                new ImageDisplayFormat(FormatType.STANDARD,2,2),
                new ImageDisplayFormat(FormatType.STANDARD,4,1),
                new ImageDisplayFormat(FormatType.STANDARD,4,2),
                new ImageDisplayFormat(FormatType.ROW,1,2),
                new ImageDisplayFormat(FormatType.COL,1,2)
            };

        private string _dicomString;

        /// <summary>
        /// Constructor for serialization.
        /// </summary>
        public ImageDisplayFormat()
        {
        }

        public ImageDisplayFormat(string dicomString)
        {
            this.DicomString = dicomString;
        }

        private ImageDisplayFormat(FormatType baseType, params int[] typeModifiers)
        {
            this.BaseType = baseType;
            this.TypeModifiers = new List<int>();
            this.TypeModifiers.AddRange(typeModifiers);

            _dicomString = this.TypeModifiers.Count == 0
                ? this.BaseType.ToString()
                : string.Format(@"{0}\{1}", this.BaseType, StringUtilities.Combine(this.TypeModifiers, ","));
        }

        public string DicomString
        {
            get { return _dicomString; }
            set
            {
                var itemInChoices = CollectionUtils.SelectFirst(Choices, idf => Equals(value, idf._dicomString));
                _dicomString = itemInChoices._dicomString;
                this.BaseType = itemInChoices.BaseType;
                this.TypeModifiers = itemInChoices.TypeModifiers;
            }
        }

        [XmlIgnore]
        public FormatType BaseType { get; private set; }

        [XmlIgnore]
        public List<int> TypeModifiers { get; private set; }

        [XmlIgnore]
        public int MaximumImageBoxes
        {
            get
            {
                if (this.TypeModifiers.Count == 0)
                    return 1;

                switch (this.BaseType)
                {
                    case FormatType.STANDARD:
                        return this.TypeModifiers[0] * this.TypeModifiers[1];
                    case FormatType.ROW:
                    case FormatType.COL:
                        return CollectionUtils.Reduce<int, int>(this.TypeModifiers, 0, (m, sum) => sum + m);

                    case ImageDisplayFormat.FormatType.SLIDE:
                    case ImageDisplayFormat.FormatType.SUPERSLIDE:
                    case ImageDisplayFormat.FormatType.CUSTOM:
                    default:
                        throw new NotSupportedException(string.Format("{0} image display format is not supported", this.BaseType));
                }
            }
        }
    }
    #endregion

    #region FilmSize

    /// <summary>
    /// Film size identification.
    /// </summary>
    [TypeConverter(typeof(FilmSize.DisplayValueConverter))]
    public class FilmSize
    {
        public class DisplayValueConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value is FilmSize && destinationType == typeof(string))
                    return GetDisplayString(value as FilmSize);

                return base.ConvertTo(context, culture, value, destinationType);
            }

            private static string GetDisplayString(FilmSize filmSize)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0} {2} x {1} {2}",
                    filmSize._width,
                    filmSize._height,
                    filmSize._unitType == UnitType.Inch ? "in" : "mm");

                return filmSize.DicomString == "A3" || filmSize.DicomString == "A4"
                    ? string.Format("{0} ({1})", filmSize.DicomString, builder)
                    : builder.ToString();
            }
        }

        public static List<FilmSize> Choices = new List<FilmSize>
            {
                new FilmSize("8INX10IN", 8, 10, UnitType.Inch),
                new FilmSize("8_5INX11IN", 8.5f, 11, UnitType.Inch),
                new FilmSize("10INX12IN", 10, 12, UnitType.Inch),
                new FilmSize("10INX14IN", 10, 14, UnitType.Inch), //  corresponds with 25.7CMX36.4CM
                new FilmSize("11INX14IN", 11, 14, UnitType.Inch),
                new FilmSize("11INX17IN", 11, 17, UnitType.Inch),
                new FilmSize("14INX14IN", 14, 14, UnitType.Inch),
                new FilmSize("14INX17IN", 14, 17, UnitType.Inch),
                new FilmSize("24CMX24CM", 240, 240, UnitType.Millimeter),
                new FilmSize("24CMX30CM", 240, 300, UnitType.Millimeter),
                new FilmSize("A4", 210, 297, UnitType.Millimeter),
                new FilmSize("A3", 297, 420, UnitType.Millimeter),
            };

        public enum UnitType
        {
            Inch,
            Millimeter
        }

        private string _dicomString;
        private UnitType _unitType;
        private float _width;
        private float _height;

        // Empty constructor for serialization
        public FilmSize()
        {
        }

        public FilmSize(string dicomString)
        {
            this.DicomString = dicomString;
        }

        private FilmSize(string dicomString, float width, float height, UnitType unitType)
        {
            _dicomString = dicomString;
            _width = width;
            _height = height;
            _unitType = unitType;
        }

        // The only public property, for serialization
        public string DicomString
        {
            get { return _dicomString; }
            set
            {
                var filmSizeInChoices = CollectionUtils.SelectFirst(Choices, fs => Equals(value, fs.DicomString));
                _dicomString = filmSizeInChoices._dicomString;
                _unitType = filmSizeInChoices._unitType;
                _width = filmSizeInChoices._width;
                _height = filmSizeInChoices._height;
            }
        }

        public float GetHeight(UnitType desiredType)
        {
            if (_unitType == desiredType)
                return _height;

            return desiredType == UnitType.Inch
                ? ConvertToInches(_height)
                : ConvertToMillimeters(_height);
        }

        public float GetWidth(UnitType desiredType)
        {
            if (_unitType == desiredType)
                return _width;

            return desiredType == UnitType.Inch
                ? ConvertToInches(_width)
                : ConvertToMillimeters(_width);
        }

        private static float ConvertToMillimeters(float inches)
        {
            return inches * 25.4f;
        }

        private static float ConvertToInches(float mm)
        {
            return mm / 25.4f;
        }
    }

    public class BasicPrintEnumConverter<TEnumType> : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is TEnumType && destinationType == typeof(string))
            {
                if (value.ToString() == "None")
                    return "Default";

                if (value is MediumType)
                    return ConvertMediumType((MediumType)value);

                if (value is FilmDestination)
                    return ConvertFilmDestination((FilmDestination) value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static string ConvertMediumType(MediumType mediumType)
        {
            switch (mediumType)
            {
                case MediumType.Paper:
                    return "Paper";
                case MediumType.ClearFilm:
                    return "Clear Film";
                case MediumType.BlueFilm:
                    return "Blue Film";
                case MediumType.MammoClearFilm:
                    return "Mammo Clear Film";
                case MediumType.MammoBlueFilm:
                    return "Mammo Blue Film";
                default:
                    return mediumType.ToString();
            }
        }

        private static string ConvertFilmDestination(FilmDestination filmDestination)
        {
            var destinationString = filmDestination.ToString();
            return destinationString.StartsWith("Bin_")
                ? destinationString.Replace('_', ' ')
                : destinationString;
        }
    }

    #endregion

}
