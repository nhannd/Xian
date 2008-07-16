public class ImageServerConstants
{
    public const string High = "high";
    public const string Low = "low";
    public const string ImagePng = "image/png";
    public const string Pct = "pct";
    public const string Default = "Default";
    public const string First = "first";
    public const string Last = "last";
    public const string Next = "next";
    public const string Prev = "prev";
    public const string MMDDYYY = "MM/dd/yyyy";
    public const string DicomDate = "yyyyMMdd";
    public const string DicomDateTime = "YYYYMMDDHHMMSS.FFFFFF";
    public const string DicomSeparator = "^";
    
    public class DicomTags {
        public const string PatientsName = "00100010";
        public const string PatientID = "00100020";
        public const string PatientsBirthDate = "00100030";
        public const string PatientsSex = "00100040";
        public const string PatientsAge = "00101010";
        public const string ReferringPhysician = "00080090";
        public const string StudyDate = "00080020";
        public const string StudyTime = "00080030";
        public const string AccessionNumber = "00080050";
        public const string StudyDescription = "00081030";
        public const string StudyInstanceUID = "0020000D";
        public const string StudyID = "00200010";
    }
}
