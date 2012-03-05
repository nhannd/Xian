namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// 
    /// </summary>
    public enum CFindRspCharacterSet
    {
        /// <summary>
        /// Use whatever in the source (header/database)
        /// </summary>
        Source,    

        /// <summary>
        /// Use  ISO_IR 126 (UTF-8)
        /// </summary>
        ISO_IR_126  
    }
}