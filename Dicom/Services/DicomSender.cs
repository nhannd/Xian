using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Services
{
    class DicomSender : IDicomSender
    {

        #region Handcoded and Private members


        private ApplicationEntity _sourceAE;
        private ApplicationEntity _destinationAE;
        #endregion

        #region IDicomSender Members

        public void SetSourceApplicationEntity(ApplicationEntity ae)
        {
            _sourceAE = ae;
        }

        public void SetDestinationApplicationEntity(ApplicationEntity ae)
        {
            _destinationAE = ae;
        }

        public void Send(IEnumerable<string> fileNames, IEnumerable<string> sopClasses, IEnumerable<string> transferSyntaxes)
        {
            DicomClient client = new DicomClient(_sourceAE);
            client.Store(_destinationAE, fileNames, sopClasses, transferSyntaxes);
        }

        #endregion
    }
}
