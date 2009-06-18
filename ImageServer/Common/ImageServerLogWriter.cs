#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using Ionic.Zip;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Class representing a log file being written.  The log file is ssaved in a MemoryStream in memory.
	/// </summary>
	/// <typeparam name="TLogClass">The class type being written to the log file.</typeparam>
	class ImageServerLogFile<TLogClass> : IDisposable
	{
		private MemoryStream _ms = null;
		private StreamWriter _sw = null;
		private readonly string _logType;

		public DateTime FirstTimestamp;
		public DateTime LastTimestamp;
		public string ZipFile;
		public string ZipDirectory;
		public DateTime Date;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="timestamp">The timestamp of the first log being written.</param>
		/// <param name="archivePath">The path to place the log file in.</param>
		/// <param name="logSize">The size of the log file.</param>
		/// <param name="logType">A string representation of the type of logs being written.  Used for logging and naming purposes.</param>
		public ImageServerLogFile(DateTime timestamp, string archivePath, int logSize, string logType)
		{
			_logType = logType;
			FirstTimestamp = timestamp;
			LastTimestamp = timestamp;
			Date = timestamp.Date;
			ZipDirectory = Path.Combine(archivePath, Date.ToString("yyyy-MM"));
			ZipFile = Path.Combine(ZipDirectory, String.Format("{0}Log_{1}.zip", logType, Date.ToString("yyyy-MM-dd")));
			_ms = new MemoryStream(logSize + 3 * 1024);
			_sw = new StreamWriter(_ms);
		}

		/// <summary>
		/// The name of the current log file.
		/// </summary>
		public string LogFileName
		{
			get
			{
				return String.Format("ImageServer{0}_{1}_to_{2}.log", _logType, FirstTimestamp.ToString("yyyy-MM-dd_HH-mm-ss"),
					LastTimestamp.ToString("HH-mm-ss"));
			}
		}
	
		/// <summary>
		/// The memory stream being written to.
		/// </summary>
		public MemoryStream Stream
		{
			get { return _ms; }
		}

		/// <summary>
		/// Write a log to the file.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="timestamp"></param>
		public void Write(TLogClass log, DateTime timestamp)
		{
			_sw.WriteLine(log.ToString());
			_sw.Flush();
			LastTimestamp = timestamp;
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			if (_sw != null)
			{
				_sw.Close();
				_sw.Dispose();
				_sw = null;
			}
			if (_ms != null)
			{
				_ms.Close();
				_ms.Dispose();
				_ms = null;
			}
		}
	}

	/// <summary>
	/// Simple writer class for archiving log data into zip files.
	/// </summary>
	/// <remarks>
	/// The class assumes that logs are supplied in increasing time order.  It will
	/// write a max of 10MB to a specific log file, then create a new log file with the
	/// name containing a time stamp.
	/// </remarks>
	public class ImageServerLogWriter<TLogClass> : IDisposable
	{
		private readonly string _logDirectory;
		private int _logSize = 10*1024*1024;
		private ImageServerLogFile<TLogClass> _archiveLog;
		private readonly string _logType;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logDirectory">The filesystem directory to place the log files into.</param>
		/// <param name="logType">A textual description of the type of log being archived.</param>
		public ImageServerLogWriter(string logDirectory, string logType)
		{
			_logDirectory = logDirectory;
			_logType = logType;
		}

		/// <summary>
		/// The size of the log files to be generated.
		/// </summary>
		public int LogFileSize
		{
			get { return _logSize; }
			set { _logSize = value; }
		}

		/// <summary>
		/// Write to the log.
		/// </summary>
		/// <param name="log">The class instance to log.</param>
		/// <param name="timestamp">The timestamp associated with the log.</param>
		/// <returns>true, if the logs have been flushed.</returns>
		public bool WriteLog(TLogClass log, DateTime timestamp)
		{
			DateTime logDate = timestamp.Date;
			if (_archiveLog == null)
			{
				_archiveLog = new ImageServerLogFile<TLogClass>(timestamp, _logDirectory, LogFileSize, _logType);
				Platform.Log(LogLevel.Info, "Starting archival of {0} logs for {1}",_logType, _archiveLog.FirstTimestamp.ToLongDateString());
			}

			if (logDate.Equals(_archiveLog.Date))
			{
				_archiveLog.Write(log, timestamp);
				if (_archiveLog.Stream.Length > LogFileSize)
				{
					FlushLog();
					return true;
				}
				return false;
			}

			// Flush the current log
			FlushLog();
		
			// Simple recursive call to rewrite, since the log has been flushed, will only go 1 deep
			// on the recursion because FlushLog set _archiveLog = null.
			WriteLog(log, timestamp);

			return true;
		}

		/// <summary>
		/// Routine for flushing the log file to the correct zip file.
		/// </summary>
		public void FlushLog()
		{
			if (_archiveLog == null) return;

			Platform.Log(LogLevel.Info, "Flushing log of {0} for {1}", _logType, _archiveLog.FirstTimestamp.ToLongDateString());

			if (!Directory.Exists(_logDirectory))
				Directory.CreateDirectory(_logDirectory);
			if (!Directory.Exists(_archiveLog.ZipDirectory))
				Directory.CreateDirectory(_archiveLog.ZipDirectory);

			using (ZipFile zip = File.Exists(_archiveLog.ZipFile) ? 
					ZipFile.Read(_archiveLog.ZipFile) :
					new ZipFile(_archiveLog.ZipFile))
			{
				zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
			
				ZipEntry e = zip.AddFileStream(_archiveLog.LogFileName, string.Empty, _archiveLog.Stream);
				e.Comment =
					String.Format("Log of {0} from {1} to {2}", _logType, _archiveLog.FirstTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
					              _archiveLog.LastTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

				zip.Save();
			}

			_archiveLog.Dispose();
			_archiveLog = null;
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			if (_archiveLog != null)
			{
				FlushLog();
			}
		}
	}
}
