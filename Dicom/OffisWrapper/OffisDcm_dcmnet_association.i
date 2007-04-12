/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Association class
//
%typemap(csin) DcmDataset *cFindDataset "getCPtrAndAddReferenceFind($csinput)"
%typemap(csin) DcmDataset *cMoveDataset "getCPtrAndAddReferenceMove($csinput)"
%typemap(csin) T_ASC_Network *network "getCPtrAndAddReferenceNetwork($csinput)"

%typemap(cscode) T_ASC_Association %{
	// Ensure that the GC does not collect any 
	// DcmDataset set from C#
	// as the underlying C++ class stores a shallow copy
	private DcmDataset _findReference;
	private DcmDataset _moveReference;
	private T_ASC_Network _networkReference;
	private HandleRef getCPtrAndAddReferenceFind(DcmDataset cFindDataset) {
		_findReference = cFindDataset;
		return DcmDataset.getCPtr(cFindDataset);
	}
	private HandleRef getCPtrAndAddReferenceMove(DcmDataset cMoveDataset) {
		_moveReference = cMoveDataset;
		return DcmDataset.getCPtr(cMoveDataset);
	}
	private HandleRef getCPtrAndAddReferenceNetwork(T_ASC_Network network) {
		_networkReference = network;
		return T_ASC_Network.getCPtr(network);
	}
%}

/////////////////////////////////////////////////////////////////////////
// 
// SECTION: Customize the Dispose method to clean up references to the
// C# proxy objects that hold references to underlying unmanaged C++
// objects, created using the getCPtrAndAddXXX methods. The references
// were created in the first place, so that they are not GC when
// PInvoke occurs, but they have to be cleaned up afterwards
//
%typemap(csdestruct, methodname="Dispose") T_ASC_Association {
	if(swigCPtr.Handle != IntPtr.Zero && swigCMemOwn) 
	{
		swigCMemOwn = false;
		$imcall;
		_findReference = null;
		_moveReference = null;
		_networkReference = null;
	}
	swigCPtr = new HandleRef(null, IntPtr.Zero);

	GC.SuppressFinalize(this);
}

struct T_ASC_Association
{
    DUL_ASSOCIATIONKEY *DULassociation;
    T_ASC_Parameters *params;

    unsigned short nextMsgID;	        /* should be incremented by user */
    unsigned long sendPDVLength;	/* max length of PDV to send out */
    unsigned char *sendPDVBuffer;	/* buffer of size sendPDVLength */
};

%typemap(csin) T_ASC_Network *network;

%extend(canthrow=1) T_ASC_Association {

	bool SendCEcho(int numberOfCEchoRepeats, int timeout) throw (dicom_runtime_error)
	{
		OFCondition cond = EC_Normal;
		unsigned long n = numberOfCEchoRepeats;

		// as long as no error occured and the counter does not equal 0 
		// send an C-ECHO-RQ and handle the response 
		while (cond == EC_Normal && n--) // compare with EC_Normal since DUL_PEERREQUESTEDRELEASE is also good()
		{
			DIC_US msgId = self->nextMsgID++;
			DIC_US status;
			DcmDataset *statusDetail = NULL;

			// send C-ECHO-RQ and handle response 
			cond = DIMSE_echoUser(self, msgId, DIMSE_BLOCKING, timeout,
				&status, &statusDetail);

			// check for status detail information, there should never be any 
			if (statusDetail != NULL) {
				delete statusDetail;
			}
		}

		if (cond != EC_Normal && cond.bad())
		{
			string msg = string("SendCEcho: ") + cond.text();

			if (cond != DUL_PEERABORTEDASSOCIATION)
				ASC_abortAssociation(self);
			
			throw dicom_runtime_error(cond, msg);
		}

		return true;
	}

	bool SendCStore(std::vector<string > fileNameList, unsigned long storeOperationIdentifier) throw (dicom_runtime_error)
	{
		OFCondition cond = EC_Normal;
		std::vector<string >::iterator iter = fileNameList.begin();
		std::vector<string >::iterator enditer = fileNameList.end();

		int currentCount = 1;
		int totalCount = fileNameList.size();
		while ((iter != enditer) && (cond == EC_Normal)) // compare with EC_Normal since DUL_PEERREQUESTEDRELEASE is also good()
		{
			cond = StoreScu(self, (*iter).c_str(), storeOperationIdentifier, currentCount++, totalCount);

			// don't increment the iterator if cond is not EC_Normal so that we can get the file name
			if (cond == EC_Normal)
				++iter;
		}

		/* tear down association, i.e. terminate network connection to SCP */
		if (cond == EC_Normal)
		{
			return true;
		}
		else if (cond == DUL_PEERREQUESTEDRELEASE)
		{
			ASC_abortAssociation(self);
			string msg = string("SendCStore: Protocol error, peer requested release (association aborted); ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}
		else if (cond == DUL_PEERABORTEDASSOCIATION)
		{
			// right now we don't do anything special
			// but in the future, this could be logged
			return false;
		}
		else // some other abnormal condition
		{
			string msg = string("SendCStore: Bad dicom file - ") + (*iter) + cond.text();
			ASC_abortAssociation(self);
			throw dicom_runtime_error(cond, msg);
		}
	}

	bool SendCMoveStudyRootQuery(DcmDataset* cMoveDataset, T_ASC_Network* network, int timeout, const char* saveDirectory, unsigned long queryRetrieveOperationIdentifier) 
		throw (dicom_runtime_error)
	{
		T_ASC_PresentationContextID presId;
		T_DIMSE_C_MoveRQ    req;
		T_DIMSE_C_MoveRSP   rsp;
		DIC_US              msgId = self->nextMsgID++;
		const char          *sopClass;
		DcmDataset          *rspIds = NULL;
		DcmDataset          *statusDetail = NULL;
		InteropQueryRetrieveCallbackInfo      callbackData;

		/* which presentation context should be used */
		sopClass = UID_MOVEStudyRootQueryRetrieveInformationModel;
		presId = ASC_findAcceptedPresentationContextID(self, sopClass);
		if (presId == 0) 
			throw dicom_runtime_error(DIMSE_NOVALIDPRESENTATIONCONTEXTID, "SendCMoveStudyRootQuery: No presentation context");

		callbackData.QueryRetrieveOperationIdentifier = queryRetrieveOperationIdentifier;
		
		req.MessageID = msgId;
		strcpy(req.AffectedSOPClassUID, sopClass);
		req.Priority = DIMSE_PRIORITY_MEDIUM;
		req.DataSetType = DIMSE_DATASET_PRESENT;
		ASC_getAPTitles(self->params, req.MoveDestination, NULL, NULL);

		OFCondition cond;
		cond = DIMSE_moveUser(self, presId, &req, cMoveDataset,
				MoveProgressCallback, &callbackData, DIMSE_BLOCKING, timeout,
				network, CStoreSubOpCallback, (void*) saveDirectory,
				&rsp, &statusDetail, &rspIds); 

		if (rspIds != NULL) delete rspIds;

		if (cond == EC_Normal)
		{
			return true;
		}
		else if (cond == DUL_PEERREQUESTEDRELEASE)
		{
			ASC_abortAssociation(self);

			string msg = string("SendCFindStudyRootQuery: Protocol error, peer requested release (association aborted); ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}
		else if (cond == DUL_PEERABORTEDASSOCIATION)
		{
			// right now we don't do anything special
			// but in the future, this could be logged
			return false;
		}
		else // some other abnormal condition
		{
			return false;
		}
	}

	bool SendCFindStudyRootQuery(DcmDataset* cFindDataset, unsigned long queryRetrieveOperationIdentifier) throw (dicom_runtime_error)
	{
		DIC_US msgId = self->nextMsgID++;
		T_ASC_PresentationContextID presId;
		T_DIMSE_C_FindRQ req;
		T_DIMSE_C_FindRSP rsp;
		DcmDataset *statusDetail = NULL;
		InteropQueryRetrieveCallbackInfo callbackData;

		presId = ASC_findAcceptedPresentationContextID(self, UID_FINDStudyRootQueryRetrieveInformationModel);

		if (presId == 0) 
			throw dicom_runtime_error(DIMSE_NOVALIDPRESENTATIONCONTEXTID, "SendCFindStudyRootQuery: No presentation context");

		// prepare the transmission of data 
		bzero((char*)&req, sizeof(req));
		req.MessageID = msgId;
		strcpy(req.AffectedSOPClassUID, UID_FINDStudyRootQueryRetrieveInformationModel);
		req.DataSetType = DIMSE_DATASET_PRESENT;
		req.Priority = DIMSE_PRIORITY_LOW;

		// prepare the callback 
		callbackData.QueryRetrieveOperationIdentifier = queryRetrieveOperationIdentifier;
		
		// finally conduct transmission of data
		OFCondition cond = DIMSE_findUser(self, presId, &req, cFindDataset,
						  CFindProgressCallback, &callbackData,
						  DIMSE_BLOCKING, 0,
						  &rsp, &statusDetail);

		if (cond == EC_Normal)
		{
			return true;
		}
		else if (cond == DUL_PEERREQUESTEDRELEASE)
		{
			ASC_abortAssociation(self);

			string msg = string("SendCFindStudyRootQuery: Protocol error, peer requested release (association aborted); ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}
		else if (cond == DUL_PEERABORTEDASSOCIATION)
		{
			// right now we don't do anything special
			// but in the future, this could be logged
			return false;
		}
		else // some other abnormal condition
		{
			return false;
		}
		
		/* This was in the original findscu.cxx code and may become
		 * helpful when we do more logging so I'm keeping it here for now

		if (cond == EC_Normal) {
			if (opt_verbose) {
				DIMSE_printCFindRSP(stdout, &rsp);
			} else {
				if (rsp.DimseStatus != STATUS_Success) {
					printf("Response: %s\n", DU_cfindStatusString(rsp.DimseStatus));
				}
			}
		} else {
			if (fname) {
				errmsg("Find Failed, file: %s:", fname);
			} else {
				errmsg("Find Failed, query keys:");
				dcmff.getDataset()->print(COUT);
			}
			DimseCondition::dump(cond);
		}

		if (statusDetail != NULL) {
			printf("  Status Detail:\n");
			statusDetail->print(COUT);
			delete statusDetail;
		}
		*/
	}

	OFCondition ProcessServerCommands(int operationTimeout, const char* saveDirectory)
	{
		OFCondition cond = EC_Normal;
		T_DIMSE_Message msg;
		T_ASC_PresentationContextID presID = 0;
		DcmDataset *statusDetail = NULL;

		// start a loop to be able to receive more than one DIMSE command
		while (cond == EC_Normal || cond == DIMSE_NODATAAVAILABLE)
		{
			// receive a DIMSE command over the network
			if (operationTimeout == -1)
		  		cond = DIMSE_receiveCommand(self, DIMSE_BLOCKING, 0, &presID, &msg, &statusDetail);
			else
			  	cond = DIMSE_receiveCommand(self, DIMSE_NONBLOCKING, operationTimeout, &presID, &msg, &statusDetail);

			// check what kind of error occurred. If no data was
			// received, check if certain other conditions are met
			if (cond == DIMSE_NODATAAVAILABLE)
			{
				// If in addition to the fact that no data was received also option --eostudy-timeout is set and
				// if at the same time there is still a study which is considered to be open (i.e. we were actually
				// expecting to receive more objects that belong to this study) (this is the case if lastStudyInstanceUID
				// does not equal NULL), we have to consider that all objects for the current study have been received.
				// In such an "end-of-study" case, we might have to execute certain optional functions which were specified
				// by the user through command line options passed to storescp.
				if( operationTimeout != -1)
				{
				}
			}

			// if the command which was received has extra status
			// detail information, dump this information
			if (statusDetail != NULL)
			{
				// printf("Extra Status Detail: \n");
				// statusDetail->print(COUT);
				delete statusDetail;
			}

			// check if peer did release or abort, or if we have a valid message
			if (cond == EC_Normal)
			{
				// in case we received a valid message, process this command
				// note that storescp can only process a C-ECHO-RQ and a C-STORE-RQ
				switch (msg.CommandField)
				{
					case DIMSE_C_ECHO_RQ:
						// process C-ECHO-Request
						cond = EchoScp(self, &msg, presID);
						break;
					case DIMSE_C_STORE_RQ:
						// process C-STORE-Request
						cond = StoreScp(self, &msg, presID, saveDirectory);
						break;
					case DIMSE_C_FIND_RQ:
						cond = FindScp(self, &msg.msg.CFindRQ, presID);
						break;
					case DIMSE_C_MOVE_RQ:
						cond = MoveScp(self, &msg.msg.CMoveRQ, presID);
						break;
					default:
						// we cannot handle this kind of message
						cond = DIMSE_BADCOMMANDTYPE;
						// fprintf(stderr, "storescp: Cannot handle command: 0x%x\n", OFstatic_cast(unsigned, msg.CommandField));
						string msg = string("Association::ProcessCommands: ") + cond.text();
						throw dicom_runtime_error(cond, msg);
				}
			}
		}
		return cond;
	}

	void DropAssociation()
	{
		OFCondition cond = ASC_dropSCPAssociation(self);
	}

	void RespondToReleaseRequest()
	{
		OFCondition cond = ASC_acknowledgeRelease(self);
        cond = ASC_dropSCPAssociation(self);
	}

	bool Release() throw (dicom_runtime_error)
	{
		OFCondition cond = EC_Normal;
		cond = ASC_releaseAssociation(self);

		if (cond != EC_Normal && cond.bad())
		{
			string msg = string("Release: ") + cond.text();
			throw dicom_runtime_error(cond, msg);
		}

		return true;
	}
}

