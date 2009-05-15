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

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Provides convenient blocking methods for file opening.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public static class FileStreamOpener
    {
        #region Private Members
        private const int RETRY_MIN_DELAY = 100; 
        private const int FILE_MISSING_OVERRIDE_TIMEOUT = 2; // # of seconds to abort if the file is missing.
        #endregion

        /// <summary>
        /// Opens a file for update, using specified mode
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">Opening mode</param>
        /// <returns>A <see cref="FileStream"/> with sole write access.</returns>
        /// <remarks>
        /// This methods will block indefinitely until the file is opened or exceptions are thrown because file cannot be open 
        /// using the specified mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// <para>
        /// <para>
        /// Once the file is opened, subsequent attempt to open the file for writing will fail until the returned stream is closed. However, other processes are 
        /// allowed to open the files for reading.
        /// </para>
        /// </remarks>
        static public FileStream OpenForSoleUpdate(string path, FileMode mode)
        {
            return OpenForSoleUpdate(path, mode, -1, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Opens a file for update, using specified opening mode and timeout period.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">File opening mode</param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <returns>The stream object for the specified file</returns>
        /// <exception cref="TimeoutException">Thrown when the file cannot be opened after the specified timeout</exception>
        /// <remarks>
        /// This methods will block until the specified file is opened or timeout has been reached.
        /// If the file cannot be open using the specified mode because it doesn't exist, exceptions may be be thrown 
        /// depending on the file opening mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// <para>
        /// Subsequent attempt to open the file for writing will fail until the returned stream is closed. However, other processes are 
        /// allowed to open the files for reading.
        /// </para>
        /// </remarks>
        static public FileStream OpenForSoleUpdate(string path, FileMode mode, int timeout)
        {
            return OpenForSoleUpdate(path, mode, timeout, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Opens a file for update, using specified opening mode and waits until timeout expires or a cancelling signal is set.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">File opening mode</param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <param name="stopSignal">Cancelling signal</param>
        /// <param name="retryMinDelay">Minimum delay between retries</param>
        /// <returns>The stream object for the specified file</returns>
        /// <exception cref="TimeoutException">Thrown when the file cannot be opened after the specified timeout</exception>
        /// <remarks>
        /// <para>
        /// This methods will block until the specified file is opened or when timeout has been reached or the cancelling signal 
        /// is set. If the file cannot be open using the specified mode because it doesn't exist, exceptions may be be thrown 
        /// depending on the file opening mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// <para>
        /// The returned stream will have opened with <see cref="FileAccess.Write"/> and <see cref="FileShare.Read"/>  permissions.
        /// Subsequent attempt to open the file for writing will fail until the stream is closed. However, other processes are 
        /// allowed to open the files for reading.
        /// </para>
        /// If cancel signal is set and the file hasn't been opened, <b>null</b> will be returned.
        /// </para>
        /// </remarks>
        static public FileStream OpenForSoleUpdate(string path, FileMode mode, int timeout, ManualResetEvent stopSignal, int retryMinDelay)
        {
            FileStream stream = null;

            // wait until we can lock the compressed header file for update
            long begin = Environment.TickCount;
            while (true)
            {
                try
                {
                	stream =
                		new FileStream(path, mode, FileAccess.Write, FileShare.Read
                		               /* don't block others from reading this file */, Settings.Default.WriteBufferSize,
                		               Settings.Default.WriteThroughMode ? FileOptions.WriteThrough : FileOptions.None);
                    break;
                }
                catch(FileNotFoundException)
                {
                    // The caller should've used FileMode.CreateNew or FileMode.OpenOrCreate
                    // Nothing can be done if it doesn't.
                    throw;
                }
                catch(DirectoryNotFoundException)
                {
                    // The path is invalid
                    throw;
                }
                catch (PathTooLongException)
                {
                    // The path is too long
                    throw;
                }
                catch (IOException)
                {
                    // other types of exception should be treated as retry
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(retryMinDelay, 2 * retryMinDelay));
                }

                if (stream == null)
                {
                    if (timeout > 0 && Environment.TickCount - begin > timeout)
                    {
                        throw new TimeoutException();
                    }

                    if (stopSignal != null)
                        stopSignal.WaitOne(TimeSpan.FromMilliseconds(100), false);

                }                
            }

            return stream;
        }

        /// <summary>
        /// Opens a file for reading, using specified opening mode.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns>a <see cref="FileStream"/> with read permission.</returns>
        /// <remarks>
        /// <para>
        /// </para>This method will be blocked indefinitely until the file is opened using the specified mode or exceptions
        /// are thrown because it doesn't exist. If access permission exceptions occur, the method will try to open the file again.
        /// <para>
        /// The file will be opened using <see cref="FileAccess.Read"/> and <see cref="FileShare.ReadWrite"/> permissions.
        /// </para>
        /// </remarks>
        static public FileStream OpenForRead(string path, FileMode mode)
        {
            return OpenForRead(path, mode, -1, null, RETRY_MIN_DELAY);
        }

        /// <summary>
        /// Opens a file for reading, using specified opening mode and timeout period
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <returns></returns>
        /// <remarks>
        /// The file will be opened using <see cref="FileAccess.Read"/> and <see cref="FileShare.ReadWrite"/> permissions.
        /// Once the file has been opened, other processes still can open the files for reading.
        /// </remarks>
        static public FileStream OpenForRead(string path, FileMode mode, int timeout)
        {
            return OpenForRead(path, mode, timeout, null, RETRY_MIN_DELAY);
        }


        /// <summary>
        /// Opens a file for reading, using specified opening mode and waits until timeout expires or a cancelling signal is set.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="mode">File opening mode</param>
        /// <param name="timeout">timeout (in ms)</param>
        /// <param name="stopSignal">Cancelling signal</param>
        /// <param name="retryMinDelay">Minimum delay between retries</param>
        /// <returns>The stream object for the specified file</returns>
        /// <exception cref="TimeoutException">Thrown when the file cannot be opened after the specified timeout</exception>
        /// <remarks>
        /// <para>
        /// This methods will block until the specified file is opened or when timeout has been reached or the cancelling signal 
        /// is set. If the file cannot be open using the specified mode because it doesn't exist, exceptions may be be thrown 
        /// depending on the file opening mode. If it cannot be opened due to access permission (eg, it is being locked
        /// for update by another process), the method will try again.
        /// </para>
        /// 
        /// <para>
        /// If cancel signal is set and the file hasn't been opened, <b>null</b> will be returned.
        /// </para>
        /// 
        /// <para>
        /// The returned stream will have opened with <see cref="FileAccess.Read"/> and <see cref="FileShare.ReadWrite"/> permissions.
        /// </para>
        /// 
        /// </remarks>
        static public FileStream OpenForRead(string path, FileMode mode, long timeout, ManualResetEvent stopSignal, int retryMinDelay)
        {
            FileStream stream = null;
            Exception lastException = null;
            long begin = Environment.TickCount;
            bool cancelled = false;

            while (!cancelled)
            {
                try
                {
                    stream = new FileStream(path, mode, FileAccess.Read, FileShare.ReadWrite /* allow others to update this file */);
                    break;
                }
                catch (FileNotFoundException e)
                {
                    // Maybe it is being swapped?
                    lastException = e;
                    
                    // regardless of what the caller wants, if we can't find the file 
                    // after FILE_MISSING_OVERRIDE_TIMEOUT seconds, we should abort so that we don't block 
                    // the application for too long.
                    TimeSpan elapse = TimeSpan.FromMilliseconds(Environment.TickCount - begin);
                    if (elapse > TimeSpan.FromSeconds(FILE_MISSING_OVERRIDE_TIMEOUT))
                        throw;
                }
                catch (DirectoryNotFoundException)
                {
                    // The path is invalid
                    throw;
                }
                catch (PathTooLongException)
                {
                    // The path is too long
                    throw;
                }
                catch (IOException e)
                {
                    // other IO exceptions should be treated as retry
                    lastException = e; 
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(retryMinDelay, 2 * retryMinDelay));
                }

                if (stream == null)
                {
                    if (timeout > 0 && Environment.TickCount - begin > timeout)
                    {
                        if (lastException != null)
                            throw lastException;
                        else
                            throw new TimeoutException();
                    }

                    if (stopSignal != null)
                    {
                        cancelled = stopSignal.WaitOne(TimeSpan.FromMilliseconds(100), false);
                    }
                }
            }

            return stream;
        }
    }
}
