#region License

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

namespace ClearCanvas.Common
{
    public class DynamicBuffer
    {
        public byte[] Buffer { get; private set; }

        public void Resize(int incomingDataSize, bool isCompressed)
        {
            if (incomingDataSize < 0)
                return;

            if (!isCompressed)
            {
                if (Buffer == null || Buffer.Length != incomingDataSize)
                    Buffer = new byte[incomingDataSize];
            }
            else
            {
                var currentSize = (Buffer != null) ? Buffer.Length : 0;
                if (currentSize <= incomingDataSize || currentSize > 2 * incomingDataSize)
                {
                    var newSize = (int)(incomingDataSize * 1.5);
                    //make size even
                    if ((newSize & 01) == 1)
                        newSize++;

                    Buffer = new byte[newSize];
                }
            }

        }
    }
}
