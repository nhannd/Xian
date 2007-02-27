using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client.Common
{
    public static class TextFieldMasks
    {
        public static string TelephoneNumberLocalMask
        {
            get { return FormatSettings.Default.TelephoneNumberLocalMask; }
        }

        public static string TelphoneNumberFullMask
        {
            get { return FormatSettings.Default.TelephoneNumberFullMask; }
        }

        public static string HealthcardNumberMask
        {
            get { return FormatSettings.Default.HealthcardNumberMask; }
        }

        public static string HealthcardVersionCodeMask
        {
            get { return FormatSettings.Default.HealthcardVersionCodeMask; }
        }
    }
}
