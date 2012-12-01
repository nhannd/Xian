#region License

//MWL Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

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

namespace ClearCanvas.Ris.Shreds.Mwl
{
    public class MwlShredContext
    {
    }

    [ExtensionOf(typeof (ShredExtensionPoint))]
    public class MwlShred : Shred
    {
        public MwlShred()
        {
        }

        public override void Start()
        {
            _listener = new DicomScp<MwlShredContext>(new MwlShredContext(), null)
                            {
                                AeTitle = MwlSettings.Default.AeTitle,
                                ListenPort = MwlSettings.Default.Port
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
            return MwlSettings.Default.MwlShredName;
        }

        public override string GetDescription()
        {
            return MwlSettings.Default.MwlShredDescription;
        }

        private DicomScp<MwlShredContext> _listener;
    }
}