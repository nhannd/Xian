﻿#region License

//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using ClearCanvas.Common;
using DataCache;

namespace ClearCanvas.Dicom.Utilities.Xml
{
    internal class CacheLogger : ICacheLogger
    {
        #region IDiskCacheLogger Members

        public void Log(CacheLogLevel level, string message)
        {
            var logLevel = LogLevel.Debug;
            switch (level)
            {
                case CacheLogLevel.Error:
                    logLevel = LogLevel.Error;
                    break;
                case CacheLogLevel.Info:
                    logLevel = LogLevel.Info;
                    break;
                case CacheLogLevel.Debug:
                    logLevel = LogLevel.Debug;
                    break;
                case CacheLogLevel.Warn:
                    logLevel = LogLevel.Warn;
                    break;
            }
            Platform.Log(logLevel, message);
        }

        #endregion
    }
}