/////////////////////////////////////////////////////////////////////////
//
// SECTION: Extension of the T_ASC_Parameters class
//
struct T_ASC_Parameters
{
    DIC_UI ourImplementationClassUID;
    DIC_SH ourImplementationVersionName;
    DIC_UI theirImplementationClassUID;
    DIC_SH theirImplementationVersionName;
    DUL_ModeCallback *modeCallback;

    DUL_ASSOCIATESERVICEPARAMETERS DULparams;
    /*
     * DICOM Upper Layer service parameters.  They should only be
     * accessed via functions defined below. 
     */

    long ourMaxPDUReceiveSize;		/* we say what we can receive */
    long theirMaxPDUReceiveSize;	/* they say what we can send */

};

typedef struct {
    char applicationContextName[DUL_LEN_NAME + 1];
    char callingAPTitle[DUL_LEN_TITLE + 1];
    char calledAPTitle[DUL_LEN_TITLE + 1];
    char respondingAPTitle[DUL_LEN_TITLE + 1];
    unsigned long maxPDU;
    unsigned short result;
    unsigned short resultSource;
    unsigned short diagnostic;
    char callingPresentationAddress[64];
    char calledPresentationAddress[64];
    LST_HEAD *requestedPresentationContext;
    LST_HEAD *acceptedPresentationContext;
    unsigned short maximumOperationsInvoked;
    unsigned short maximumOperationsPerformed;
    char callingImplementationClassUID[DICOM_UI_LENGTH + 1];
    char callingImplementationVersionName[16 + 1];
    char calledImplementationClassUID[DICOM_UI_LENGTH + 1];
    char calledImplementationVersionName[16 + 1];
    unsigned long peerMaxPDU;
    SOPClassExtendedNegotiationSubItemList *requestedExtNegList;
    SOPClassExtendedNegotiationSubItemList *acceptedExtNegList;
    OFBool useSecureLayer;
}   DUL_ASSOCIATESERVICEPARAMETERS;

%extend(canthrow=1) T_ASC_Parameters {

	T_ASC_Parameters(int maxReceivePduLength,
		const char* ourAETitle,
		const char* peerAETitle,
		const char* peerHostName,
		int peerPort) throw (dicom_runtime_error)
	{
		T_ASC_Parameters* pParameters = 0;
		OFCondition result = ASC_createAssociationParameters(&pParameters,
			maxReceivePduLength);

		if (result.bad())
		{
			string msg = string("ASC_createAssociationParameters: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

		result = ASC_setAPTitles(pParameters, ourAETitle, peerAETitle, NULL);

		if (result.bad())
		{
			string msg = string("ASC_setAPTitles: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
		
		// we will use an unsecured transport layer at this point (False)
		result = ASC_setTransportLayerType(pParameters, OFFalse);

		if (result.bad())
		{
			string msg = string("ASC_setTransportLayerType: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

		DIC_NODENAME localHost;
		DIC_NODENAME peerHost;

		gethostname(localHost, sizeof(localHost) - 1);
		sprintf(peerHost, "%s:%d", peerHostName, (int) peerPort);
		result = ASC_setPresentationAddresses(pParameters, localHost, peerHost);

		if (result.bad())
		{
			string msg = string("ASC_setPresentationAddresses: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

		return pParameters;
	}

	void ConfigureForVerification() throw (dicom_runtime_error)
	{
		static const char* transferSyntaxes[] = {
			UID_LittleEndianExplicitTransferSyntax,
			UID_BigEndianExplicitTransferSyntax,
			UID_LittleEndianImplicitTransferSyntax, 
		};

		OFCondition result = ASC_addPresentationContext(
			self, 
			1,
			UID_VerificationSOPClass, 
			transferSyntaxes, 
			DIM_OF(transferSyntaxes));

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}

	}


	void ConfigureForCStore(std::vector<string > interopFilenameList, std::vector<string > interopSopClassList, 
			std::vector<string > interopTransferSyntaxList) throw (dicom_runtime_error)
	{
		bool parseSopClass = (interopSopClassList.size() <= 0);
		bool parseTransferSyntax = (interopTransferSyntaxList.size() <= 0);

		if (parseSopClass)
		{
			char sopClassUID[128];
			char sopInstanceUID[128];

			for (std::vector<string >::iterator p = interopFilenameList.begin(); p != interopFilenameList.end(); ++p)
			{
				if (!DU_findSOPClassAndInstanceInFile((*p).c_str(), sopClassUID, sopInstanceUID))
				{
					OFCondition cond;

					string msg = string("SendCStore: Missing SOP class in file: ") + *p;
					throw dicom_runtime_error(cond, msg);
				}
				else if (!dcmIsaStorageSOPClassUID(sopClassUID))
				{
					OFCondition cond;

					string msg = string("SendCStore: Unknown storage SOP class in file: ") + *p;
					throw dicom_runtime_error(cond, msg);
				}
				else
				{
					interopSopClassList.push_back(sopClassUID);
				}
			}
		}

		OFCondition result = AddStoragePresentationContexts(self, interopSopClassList);
		if (result.bad())
		{
			string msg = string("AddStoragePresentationContexts: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
	}

	void ConfigureForCMoveStudyRootQuery() throw (dicom_runtime_error)
	{
		const char* transferSyntaxes[] = { NULL, NULL, NULL };
		int numTransferSyntaxes = 0;

        if (gLocalByteOrder == EBO_LittleEndian)  /* defined in dcxfer.h */
        {
            transferSyntaxes[0] = UID_LittleEndianExplicitTransferSyntax;
            transferSyntaxes[1] = UID_BigEndianExplicitTransferSyntax;
        } else {
            transferSyntaxes[0] = UID_BigEndianExplicitTransferSyntax;
            transferSyntaxes[1] = UID_LittleEndianExplicitTransferSyntax;
        }
        transferSyntaxes[2] = UID_LittleEndianImplicitTransferSyntax;
        numTransferSyntaxes = 3;

		OFCondition result = ASC_addPresentationContext(
			self, 
			1,
			UID_MOVEStudyRootQueryRetrieveInformationModel,
			transferSyntaxes, 
			DIM_OF(transferSyntaxes));

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
	}

	void ConfigureForStudyRootQuery() throw (dicom_runtime_error)
	{
		const char* transferSyntaxes[] = {
			UID_LittleEndianExplicitTransferSyntax,
			UID_BigEndianExplicitTransferSyntax,
			UID_LittleEndianImplicitTransferSyntax
		};

		OFCondition result = ASC_addPresentationContext(
			self, 
			1,
			UID_FINDStudyRootQueryRetrieveInformationModel,
			transferSyntaxes, 
			DIM_OF(transferSyntaxes));

		if (result.bad())
		{
			string msg = string("ASC_addPresentationContext: ") + result.text();
			throw dicom_runtime_error(result, msg);
		}
	}

	~T_ASC_Parameters() 
	{
		if (NULL != self)
			ASC_destroyAssociationParameters(&self);
	}
}

