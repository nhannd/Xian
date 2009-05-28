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
using System.Threading;

namespace ClearCanvas.ImageServer.Common.Utilities
{
	/// <summary>
	/// File related utilities.
	/// </summary>
	public class FileUtils
	{
		private const int RETRY_MIN_DELAY = 100; // 100 ms
		private const long RETRY_MAX_DELAY = 10 * 1000; // 10 Seconds 
    
		/// <summary>
		/// Replacement for <see cref="File.Delete"/> that retries if the file is in use.
		/// </summary>
		/// <param name="path">The path to delete</param>
		static public void Delete(string path)
		{
			Delete(path, RETRY_MAX_DELAY, null, RETRY_MIN_DELAY);
		}

		/// <summary>
		/// Replacement for <see cref="File.Delete"/> that retries if the file is in use.
		/// </summary>
		/// <param name="path">The path to delete.</param>
		/// <param name="timeout">The timeout in milliseconds to attempt to retry to delete the file</param>
		/// <param name="stopSignal">An optional stopSignal to tell the delete operation to stop if retrying</param>
		/// <param name="retryMinDelay">The minimum number of milliseconds to delay when deleting.</param>
		static public void Delete(string path, long timeout, ManualResetEvent stopSignal, int retryMinDelay)
		{
            if (!File.Exists(path))
                return; //nothing to do

			Exception lastException = null;
			long begin = Environment.TickCount;
			bool cancelled = false;

			while (!cancelled)
			{
				try
				{
					File.Delete(path);
					return;
				}
				catch (IOException e)
				{
					// other IO exceptions should be treated as retry
					lastException = e;
					Random rand = new Random();
					Thread.Sleep(rand.Next(retryMinDelay, 2*retryMinDelay));
				}

				if (timeout > 0 && Environment.TickCount - begin > timeout)
				{
					if (lastException != null)
						throw lastException;
					else
						throw new TimeoutException();
				}

				if (stopSignal != null)
				{
					cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(retryMinDelay), false);
				}
			}

			throw lastException;
		}


        /// <summary>
        /// Replacement for <see cref="File.Copy"/> that retries if the file is in use.
        /// </summary>
        /// <param name="source">The path to copy from.</param>
        /// <param name="destination">The path to copy to.</param>
        static public void Copy(string source, string destination, bool overwrite)
        {
            Copy(source, destination, overwrite, RETRY_MAX_DELAY, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Replacement for <see cref="File.Copy"/> that retries if the file is in use.
        /// </summary>
        /// <param name="source">The path to copy from.</param>
        /// <param name="destination">The path to copy to.</param>
        /// <param name="overwrite">Boolean value to indicate whether to overwrite the destination if it exists</param>
        /// <param name="timeout">The timeout in milliseconds to attempt to retry to delete the file</param>
        /// <param name="stopSignal">An optional stopSignal to tell the delete operation to stop if retrying</param>
        /// <param name="retryMinDelay">The minimum number of milliseconds to delay when deleting.</param>
        static public void Copy(string source, string destination, bool overwrite,
                long timeout, ManualResetEvent stopSignal, int retryMinDelay)
        {
            if (!File.Exists(source))
                throw new FileNotFoundException(String.Format("Source file {0} does not exist", source), source);

            Exception lastException = null;
            long begin = Environment.TickCount;
            bool cancelled = false;

            while (!cancelled)
            {
                try
                {
                    File.Copy(source, destination, overwrite);
                    return;
                }
                catch (IOException e)
                {
                    // other IO exceptions should be treated as retry
                    lastException = e;
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(retryMinDelay, 2 * retryMinDelay));
                }

                if (timeout > 0 && Environment.TickCount - begin > timeout)
                {
                    if (lastException != null)
                        throw lastException;
                    else
                        throw new TimeoutException();
                }

                if (stopSignal != null)
                {
                    cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(retryMinDelay), false);
                }
            }

            throw lastException;
        }

        /// <summary>
        /// Creates copy of the specified file and returns the path to the backup file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The path to the backup file. Null if the file is not backed up.</returns>
        /// <param name="inSourceFolder">A boolean value indicating whether to save the backup in the same folder</param>
        /// <remarks>If the file is in use, retry will be attempted until it succeeds.</remarks>
        static public string Backup(string source, bool inSourceFolder)
        {
            FileInfo sourceInfo = new FileInfo(source);
            if (File.Exists(source))
            {
                int i = 0;
                bool filenameAbtained = false;
                while(!filenameAbtained)
                {
                    string backup;

                    // check if the file still exists every time in case it is moved/deleted 
                    // so that we are not stucked in the loop
                    if (!File.Exists(source))
                        throw new FileNotFoundException(String.Format("Source file {0} does not exist", source), source);

                    if (inSourceFolder)
                        backup = Path.Combine(sourceInfo.Directory.FullName, String.Format("{0}.bak({1})", sourceInfo.Name, i));
                    else 
                        backup =  (CommandProcessor.ExecutionContext.Current != null)
                                     ? Path.Combine(CommandProcessor.ExecutionContext.Current.BackupDirectory, String.Format("{0}.bak({1})", sourceInfo.Name, i))
                                     : Path.Combine(sourceInfo.Directory.FullName, String.Format("{0}.bak({1})", sourceInfo.Name, i));
                
                    try
                    {
                        FileStream stream = FileStreamOpener.OpenForSoleUpdate(backup, FileMode.CreateNew, RETRY_MIN_DELAY);
                        stream.Close();
                        filenameAbtained = true;
                    }
                    catch(Exception ex)
                    {
                        // try another file name
                    }

                    if (filenameAbtained)
                    {
                        FileUtils.Copy(source, backup, true);
                        return backup;
                    }
                }
            }

            return null;
                
        }

        /// <summary>
        /// Creates copy of the specified file and returns the path to the backup file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The path to the backup file. Null if the file is not backed up.</returns>
        static public string Backup(string source)
        {
            return Backup(source, false);
        }
	}
}
