#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public static class TextFieldMasks
    {
        public static string TelephoneNumberLocalMask
        {
            get { return FormatSettings.Default.TelephoneNumberLocalMask; }
        }

        public static string TelephoneNumberFullMask
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

    	public static string AccessionNumberMask
    	{
			get { return FormatSettings.Default.AccessionNumberMask; }
    	}
    }
}
