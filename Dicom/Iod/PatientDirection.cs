using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Iod
{
    public class PatientDirection
    {
        public enum Component
        {
            Primary = 0,
            Secondary = 1,
            Tertiary = 2
        }

        public const char UnspecifiedCode = 'X';
        public const char LeftCode = 'L';
        public const char RightCode = 'R';
        public const char AnteriorCode = 'A';
        public const char PosteriorCode = 'P';
        public const char HeadCode = 'H';
        public const char FootCode = 'F';

        public static readonly PatientDirection Empty = new PatientDirection(String.Empty);
        public static readonly PatientDirection Unspecified = new PatientDirection(UnspecifiedCode);
        public static readonly PatientDirection Left = new PatientDirection(LeftCode);
        public static readonly PatientDirection Right = new PatientDirection(RightCode);
        public static readonly PatientDirection Posterior = new PatientDirection(PosteriorCode);
        public static readonly PatientDirection Anterior = new PatientDirection(AnteriorCode);
        public static readonly PatientDirection Head = new PatientDirection(HeadCode);
        public static readonly PatientDirection Foot = new PatientDirection(FootCode);

        public PatientDirection(char? code)
        {
            Code = String.Empty;
            if (code.HasValue)
                Code = code.Value.ToString();
        }

        public PatientDirection(string code)
        {
            Code = code ?? String.Empty;
        }

        public string Code { get; private set; }
        public int ComponentCount { get { return Code.Length; } }

        public bool IsEmpty { get { return Code.Length == 0; } }
        public bool IsUnspecified { get { return Code.Contains(UnspecifiedCode.ToString()); } }
        public bool IsValid
        {
            get
            {
                if (IsEmpty)
                    return false;

                //If contains 'X', it must be the only value.
                if (IsUnspecified && Code.Length == 1)
                    return true;

                if (Code.Length > 3)
                    return false;

                //Each value in the code must be valid.
                if (!CollectionUtils.TrueForAll(Code, IsValidCode))
                    return false;

                var normalized = Code.Replace(RightCode, LeftCode);
                normalized = normalized.Replace(AnteriorCode, PosteriorCode);
                normalized = normalized.Replace(FootCode, HeadCode);

                //Each value must be unique, and not along the same direction.
                return (CollectionUtils.Unique(normalized).Count == Code.Length);
            }
        }

        public PatientDirection this[int index]
        {
            get {return (Code.Length > index) ? new PatientDirection(Code[index]) : Empty; }
        }

        public PatientDirection this[Component component]
        {
            get { return this[(int) component]; }
        }

        public PatientDirection Primary
        {
            get { return this[0]; }
        }

        public PatientDirection Secondary
        {
            get { return this[1]; }
        }

        public PatientDirection Tertiary
        {
            get { return this[2]; }
        }

        public override bool Equals(object obj)
        {
            return obj is PatientDirection && ((PatientDirection) obj).Code == Code;
        }

        public PatientDirection OpposingDirection
        {
            get
            {
                string opposing = String.Empty;
                foreach (var component in Code)
                    opposing += GetOpposingDirection(component);
                return new PatientDirection(opposing);
            }
        }

        public override int GetHashCode()
        {
            return 0x76aF ^ Code.GetHashCode();
        }

        public override string ToString()
        {
            return Code;
        }

        public static PatientDirection operator +(PatientDirection d1, PatientDirection d2)
        {
            return new PatientDirection(d1.Code + d2.Code);
        }

        public static bool operator ==(PatientDirection d1, PatientDirection d2)
        {
            return d1.Code == d2.Code;
        }

        public static bool operator !=(PatientDirection d1, PatientDirection d2)
        {
            return d1.Code != d2.Code;
        }

        public static implicit operator String(PatientDirection direction)
        {
            return direction.ToString();
        }

        private static bool IsValidCode(char code)
        {
            foreach (var validCode in GetCodes())
            {
                if (code == validCode)
                    return true;
            }

            return false;
        }

        private static IEnumerable<char> GetCodes()
        {
            yield return LeftCode;
            yield return RightCode;
            yield return AnteriorCode;
            yield return PosteriorCode;
            yield return HeadCode;
            yield return FootCode;
        }

        private static char GetOpposingDirection(char code)
        {
            switch (code)
            {
                case LeftCode:
                    return RightCode;
                case RightCode:
                    return LeftCode;
                case AnteriorCode:
                    return PosteriorCode;
                case PosteriorCode:
                    return AnteriorCode;
                case HeadCode:
                    return FootCode;
                case FootCode:
                    return HeadCode;
            }

            return UnspecifiedCode;
        }

        public static implicit operator PatientDirection(ImageOrientationPatient.Directions direction)
        {
            switch (direction)
            {
                case ImageOrientationPatient.Directions.Left:
                    return Left;
                case ImageOrientationPatient.Directions.Right:
                    return Right;
                case ImageOrientationPatient.Directions.Anterior:
                    return Anterior;
                case ImageOrientationPatient.Directions.Posterior:
                    return Posterior;
                case ImageOrientationPatient.Directions.Head:
                    return Head;
                case ImageOrientationPatient.Directions.Foot:
                    return Foot;
            }

            return Empty;
        }
    }
}