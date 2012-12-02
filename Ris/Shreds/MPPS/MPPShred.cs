#region License

//MPPS Support for Clear Canvas RIS
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

using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Dicom.Network.Scp;

namespace ClearCanvas.Ris.Shreds.MPPS
{
    public class MPPSShredContext
    {
    }

    [ExtensionOf(typeof (ShredExtensionPoint))]
    public class MPPSShred : Shred
    {
        public MPPSShred()
        {
        }

        public override void Start()
        {
            _listener = new DicomScp<MPPSShredContext>(new MPPSShredContext(), null)
                            {
                                AeTitle = MPPSSettings.Default.AeTitle,
                                ListenPort = MPPSSettings.Default.Port
                            };


            if (!_listener.Start(IPAddress.Any))
            {
                Platform.Log(LogLevel.Error, "Unable to start SCP listener");
                _listener = null;
            }
        }

        public override void Stop()
        {
            if (_listener != null)
                _listener.Stop();
        }

        public override string GetDisplayName()
        {
            return MPPSSettings.Default.MPPSShredName;
        }

        public override string GetDescription()
        {
            return MPPSSettings.Default.MPPSShredDescription;
        }

        private DicomScp<MPPSShredContext> _listener;
    }
}