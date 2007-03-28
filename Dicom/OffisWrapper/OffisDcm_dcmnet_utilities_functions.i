%{

static unsigned long nextOperationIdentifier = 0;
static OFMutex nextOperationMutex;

unsigned long 
NextOperationIdentifier()
{
	unsigned long next;
	nextOperationMutex.lock();
	next = ++nextOperationIdentifier;
	nextOperationMutex.unlock();
	return next;
}

static void
AddRetrieveAETitle(DcmDataset *rspIds, DIC_AE ourAETitle)
{
    /*
     * Since images are stored only by us (for RSNA'93 demo),
     * we must add in our AE Title to the response identifiers.
     * The DB cannot do this since it does not know our AE Title.
     */
    OFBool ok;

    ok = DU_putStringDOElement(rspIds, DCM_RetrieveAETitle, ourAETitle);
}

// ---------------------------------------------------------------
// Helper function to add presentation contexts to an association
// parameter
// ---------------------------------------------------------------
static OFCondition
AddPresentationContext(T_ASC_Parameters *params,
    int presentationContextId, const OFString& abstractSyntax,
    const OFString& transferSyntax,
    T_ASC_SC_ROLE proposedRole = ASC_SC_ROLE_DEFAULT)
{
    const char* c_p = transferSyntax.c_str();
    OFCondition cond = ASC_addPresentationContext(params, presentationContextId,
        abstractSyntax.c_str(), &c_p, 1, proposedRole);
    return cond;
}

// -------------------------------------------------------------
// Helper function to add an array of transfer syntaxes to an 
// association parameter
// -------------------------------------------------------------
static OFCondition
AddPresentationContext(T_ASC_Parameters *params,
    int presentationContextId, const OFString& abstractSyntax,
    const std::vector<string >& transferSyntaxList,
    T_ASC_SC_ROLE proposedRole = ASC_SC_ROLE_DEFAULT)
{
    // create an array of supported/possible transfer syntaxes
    const char** transferSyntaxes = new const char*[transferSyntaxList.size()];
    int transferSyntaxCount = 0;
	std::vector<string >::const_iterator s_cur = transferSyntaxList.begin();
	std::vector<string >::const_iterator s_end = transferSyntaxList.end();
    while (s_cur != s_end) 
	{
        transferSyntaxes[transferSyntaxCount++] = (*s_cur).c_str();
        ++s_cur;
    }

    OFCondition cond = ASC_addPresentationContext(params, presentationContextId,
        abstractSyntax.c_str(), transferSyntaxes, transferSyntaxCount, proposedRole);

    delete[] transferSyntaxes;
    return cond;
}

// -------------------------------------------------------------------
// Helper function to determine whether a particular string is
// in a string array or not
// -------------------------------------------------------------------
static OFBool
IsaListMember(std::vector<string >& lst, OFString& s)
{
	std::vector<string >::iterator cur = lst.begin();
    std::vector<string >::iterator end = lst.end();

    OFBool found = OFFalse;

    while (cur != end && !found) {

        found = (s == *cur);

        ++cur;
    }

    return found;
}

// ----------------------------------------------------------------------
// Helper function to add presentation contexts that pertain to doing
// DICOM C-STORE
// ----------------------------------------------------------------------
static OFCondition
AddStoragePresentationContexts(T_ASC_Parameters *params, std::vector<string >& sopClasses)
{
    /*
     * Each SOP Class will be proposed in two presentation contexts (unless
     * the opt_combineProposedTransferSyntaxes global variable is true).
     * The command line specified a preferred transfer syntax to use.
     * This prefered transfer syntax will be proposed in one
     * presentation context and a set of alternative (fallback) transfer
     * syntaxes will be proposed in a different presentation context.
     *
     * Generally, we prefer to use Explicitly encoded transfer syntaxes
     * and if running on a Little Endian machine we prefer
     * LittleEndianExplicitTransferSyntax to BigEndianTransferSyntax.
     * Some SCP implementations will just select the first transfer
     * syntax they support (this is not part of the standard) so
     * organise the proposed transfer syntaxes to take advantage
     * of such behaviour.
     */

    OFString preferredTransferSyntax;

	/* gLocalByteOrder is defined in dcxfer.h */
	if (gLocalByteOrder == EBO_LittleEndian) 
	{
		/* we are on a little endian machine */
		preferredTransferSyntax = UID_LittleEndianExplicitTransferSyntax;
	} 
	else 
	{
		/* we are on a big endian machine */
		preferredTransferSyntax = UID_BigEndianExplicitTransferSyntax;
	}

	std::vector<string > fallbackSyntaxes;
	fallbackSyntaxes.push_back(UID_LittleEndianImplicitTransferSyntax);

	std::vector<string >::iterator s_cur;
    std::vector<string >::iterator s_end;

    // If little endian implicit is preferred then we do not need any fallback syntaxes
    // because it is the default transfer syntax and all applications must support it.
    if (false) 
	{
		// if preferred syntax was EXS_LittleEndianImplicit
        fallbackSyntaxes.clear();
    }

    // created a list of transfer syntaxes combined from the preferred and fallback syntaxes
	std::vector<string > combinedSyntaxes;
    s_cur = fallbackSyntaxes.begin();
    s_end = fallbackSyntaxes.end();
    combinedSyntaxes.push_back(preferredTransferSyntax);
    while (s_cur != s_end)
    {
        if (!IsaListMember(combinedSyntaxes, *s_cur)) combinedSyntaxes.push_back(*s_cur);
        ++s_cur;
    }

    if (false) 
	{
        // add all the known storage sop classes to the list
        // the array of Storage SOP Class UIDs comes from dcuid.h
        for (int i=0; i<numberOfAllDcmStorageSOPClassUIDs; i++) 
		{
            sopClasses.push_back(dcmAllStorageSOPClassUIDs[i]);
        }
    }

    // thin out the sop classes to remove any duplicates.
	std::vector<string > sops;
    s_cur = sopClasses.begin();
    s_end = sopClasses.end();
    while (s_cur != s_end) 
	{
        if (!IsaListMember(sops, *s_cur)) 
		{
            sops.push_back(*s_cur);
        }
        ++s_cur;
    }

    // add a presentations context for each sop class / transfer syntax pair
    OFCondition cond = EC_Normal;
    int pid = 1; // presentation context id
    s_cur = sops.begin();
    s_end = sops.end();
    while (s_cur != s_end && cond.good()) 
	{

        if (pid > 255) 
		{
            // errmsg("Too many presentation contexts");
            return ASC_BADPRESENTATIONCONTEXTID;
        }

        if (false) 
		{
			// combine transfer syntaxes
            cond = AddPresentationContext(params, pid, *s_cur, combinedSyntaxes);
            pid += 2;   /* only odd presentation context id's */
        } 
		else 
		{

            // sop class with preferred transfer syntax
            cond = AddPresentationContext(params, pid, *s_cur, preferredTransferSyntax);
            pid += 2;   /* only odd presentation context id's */

            if (fallbackSyntaxes.size() > 0) 
			{
                if (pid > 255) 
				{
                    // errmsg("Too many presentation contexts");
                    return ASC_BADPRESENTATIONCONTEXTID;
                }

                // sop class with fallback transfer syntax
                cond = AddPresentationContext(params, pid, *s_cur, fallbackSyntaxes);
                pid += 2;       /* only odd presentation context id's */
            }
        }
        ++s_cur;
    }

    return cond;
}

// -------------------------------------------
// Helper function to send a CECHO
// -------------------------------------------
static OFCondition EchoScp(
	T_ASC_Association * assoc,
	T_DIMSE_Message * msg,
	T_ASC_PresentationContextID presID)
{
	/* the echo succeeded !! */
	OFCondition cond = DIMSE_sendEchoResponse(assoc, presID, &msg->msg.CEchoRQ, STATUS_Success, NULL);
	return cond;
}

// ----------------------------------------
// Helper function to send store
// ----------------------------------------
static OFCondition
StoreScu(T_ASC_Association * assoc, const char *fname, unsigned long storeOperationIdentifier, int currentCount, int totalCount)
    /*
     * This function will read all the information from the given file,
     * figure out a corresponding presentation context which will be used
     * to transmit the information over the network to the SCP, and it
     * will finally initiate the transmission of all data to the SCP.
     *
     * Parameters:
     *   assoc - [in] The association (network connection to another DICOM application).
     *   fname - [in] Name of the file which shall be processed.
     */
{
    DIC_US msgId = assoc->nextMsgID++;
    T_ASC_PresentationContextID presId;
    T_DIMSE_C_StoreRQ req;
    T_DIMSE_C_StoreRSP rsp;
    DIC_UI sopClass;
    DIC_UI sopInstance;
    DcmDataset *statusDetail = NULL;

    /* read information from file. After the call to DcmFileFormat::loadFile(...) the information */
    /* which is encapsulated in the file will be available through the DcmFileFormat object. */
    /* In detail, it will be available through calls to DcmFileFormat::getMetaInfo() (for */
    /* meta header information) and DcmFileFormat::getDataset() (for data set information). */
    DcmFileFormat dcmff;
    OFCondition cond = dcmff.loadFile(fname);

    /* figure out if an error occured while the file was read*/
    if (cond.bad()) {
        return cond;
    }

    /* figure out which SOP class and SOP instance is encapsulated in the file */
    if (!DU_findSOPClassAndInstanceInDataSet(dcmff.getDataset(),
        sopClass, sopInstance, true)) 
	{
        return DIMSE_BADDATA;
    }

    /* figure out which of the accepted presentation contexts should be used */
    DcmXfer filexfer(dcmff.getDataset()->getOriginalXfer());

    if (filexfer.getXfer() != EXS_Unknown) presId = ASC_findAcceptedPresentationContextID(assoc, sopClass, filexfer.getXferID());
    else presId = ASC_findAcceptedPresentationContextID(assoc, sopClass);
    if (presId == 0) 
	{
        const char *modalityName = dcmSOPClassUIDToModality(sopClass);
        if (!modalityName) modalityName = dcmFindNameOfUID(sopClass);
        if (!modalityName) modalityName = "unknown SOP class";
        return DIMSE_NOVALIDPRESENTATIONCONTEXTID;
    }

    /* prepare the transmission of data */
    bzero((char*)&req, sizeof(req));
    req.MessageID = msgId;
    strcpy(req.AffectedSOPClassUID, sopClass);
    strcpy(req.AffectedSOPInstanceUID, sopInstance);
    req.DataSetType = DIMSE_DATASET_PRESENT;
    req.Priority = DIMSE_PRIORITY_LOW;

	InteropStoreScuProgressInfo progressInfo;
	progressInfo.Association = assoc;
	progressInfo.StoreOperationIdentifier = storeOperationIdentifier;
	progressInfo.CurrentFile = fname;
	progressInfo.TotalCount = totalCount;
	progressInfo.CurrentCount = currentCount;
	 
    /* finally conduct transmission of data */
    cond = DIMSE_storeUser(assoc, presId, &req,
        NULL, dcmff.getDataset(), StoreScuProgressCallback, &progressInfo,
        DIMSE_BLOCKING, 0,
        &rsp, &statusDetail, NULL, DU_fileSize(fname));

    /*
     * If store command completed normally, with a status
     * of success or some warning then the image was accepted.
     */
    if (cond == EC_Normal && (rsp.DimseStatus == STATUS_Success || DICOM_WARNING_STATUS(rsp.DimseStatus))) 
	{
        
    }

    /* remember the response's status for later transmissions of data */
    // lastStatusCode = rsp.DimseStatus;

    /* dump some more general information */
    if (cond == EC_Normal)
    {

    }
    else
    {

    }

    /* dump status detail information if there is some */
    if (statusDetail != NULL) 
	{
        delete statusDetail;
    }

    /* return */
    return cond;
}

static OFCondition AcceptSubAssociation(T_ASC_Network * aNet, T_ASC_Association ** assoc)
{
    const char* knownAbstractSyntaxes[] = {
        UID_VerificationSOPClass
    };
    const char* transferSyntaxes[] = { NULL, NULL, NULL, NULL };
    int numTransferSyntaxes;

    OFCondition cond = ASC_receiveAssociation(aNet, assoc, ASC_DEFAULTMAXPDU);
    if (cond.good())
    {
		if (gLocalByteOrder == EBO_LittleEndian)  // defined in dcxfer.h 
		{
			transferSyntaxes[0] = UID_LittleEndianExplicitTransferSyntax;
			transferSyntaxes[1] = UID_BigEndianExplicitTransferSyntax;
		} 
		else 
		{
			transferSyntaxes[0] = UID_BigEndianExplicitTransferSyntax;
			transferSyntaxes[1] = UID_LittleEndianExplicitTransferSyntax;
		}

		transferSyntaxes[2] = UID_LittleEndianImplicitTransferSyntax;
		numTransferSyntaxes = 3;

        /* accept the Verification SOP Class if presented */
        cond = ASC_acceptContextsWithPreferredTransferSyntaxes(
            (*assoc)->params,
            knownAbstractSyntaxes, DIM_OF(knownAbstractSyntaxes),
            transferSyntaxes, numTransferSyntaxes);

        if (cond.good())
        {
            /* the array of Storage SOP Class UIDs comes from dcuid.h */
            cond = ASC_acceptContextsWithPreferredTransferSyntaxes(
                (*assoc)->params,
                dcmAllStorageSOPClassUIDs, numberOfAllDcmStorageSOPClassUIDs,
                transferSyntaxes, numTransferSyntaxes);
        }
    }
	
    if (cond.good()) cond = ASC_acknowledgeAssociation(*assoc);
    if (cond.bad()) 
	{
        ASC_dropAssociation(*assoc);
        ASC_destroyAssociation(assoc);
    }
    return cond;
}

static OFCondition StoreScp(
	T_ASC_Association *assoc,
	T_DIMSE_Message *msg,
	T_ASC_PresentationContextID presID,
	const char* saveDirectoryPath)
{
	OFCondition cond = EC_Normal;
	T_DIMSE_C_StoreRQ *req;
	char imageFileName[2048];

	req = &msg->msg.CStoreRQ;
	
	// Use "GetTempFileName" instead of SOPInstanceUID to generate a temporary filename
	// this will avoid collision when multiple server are storing the same SOPInstanceUID
	// onto the server
	if (GetTempFileName(saveDirectoryPath, "DCM", 0, imageFileName) == 0)
	{
		sprintf(imageFileName, "%s%s.%s.dcm",
		saveDirectoryPath,
		dcmSOPClassUIDToModality(req->AffectedSOPClassUID),
		req->AffectedSOPInstanceUID);
	}

	StoreCallbackData callbackData;
	callbackData.assoc = assoc;
	callbackData.imageFileName = imageFileName;
	DcmFileFormat dcmff;
	callbackData.dcmff = &dcmff;

	DcmDataset *dset = dcmff.getDataset();

	cond = DIMSE_storeProvider(assoc, presID, req, (char *)NULL, true,
		&dset, StoreScpCallback, (void*)&callbackData, DIMSE_BLOCKING, 0);

	if (cond.bad())
		if (strcmp(imageFileName, NULL_DEVICE_NAME) != 0) unlink(imageFileName);

    return cond;
}

static OFCondition SubOpScp(T_ASC_Association **subAssoc, const char* saveDirectoryPath)
{
    T_DIMSE_Message     msg;
    T_ASC_PresentationContextID presID;

    if (!ASC_dataWaiting(*subAssoc, 0)) /* just in case */
        return DIMSE_NODATAAVAILABLE;

    OFCondition cond = DIMSE_receiveCommand(*subAssoc, DIMSE_BLOCKING, 0, &presID,
            &msg, NULL);

    if (cond == EC_Normal) {
        switch (msg.CommandField) {
        case DIMSE_C_STORE_RQ:
            cond = StoreScp(*subAssoc, &msg, presID, saveDirectoryPath);
            break;
        case DIMSE_C_ECHO_RQ:
            cond = EchoScp(*subAssoc, &msg, presID);
            break;
        default:
            cond = DIMSE_BADCOMMANDTYPE;
            break;
        }
    }
    /* clean up on association termination */
    if (cond == DUL_PEERREQUESTEDRELEASE)
    {
        cond = ASC_acknowledgeRelease(*subAssoc);
        ASC_dropSCPAssociation(*subAssoc);
        ASC_destroyAssociation(subAssoc);
        return cond;
    }
    else if (cond == DUL_PEERABORTEDASSOCIATION)
    {
    }
    else if (cond != EC_Normal)
    {
        /* some kind of error so abort the association */
        cond = ASC_abortAssociation(*subAssoc);
    }

    if (cond != EC_Normal)
    {
        ASC_dropAssociation(*subAssoc);
        ASC_destroyAssociation(subAssoc);
    }
    return cond;
}

static void
CStoreSubOpCallback(void * subOpCallbackData,
        T_ASC_Network *aNet, T_ASC_Association **subAssoc)
{

    if (aNet == NULL) return;   /* help no net ! */

	const char* saveDirectoryPath = (const char*) subOpCallbackData;

    if (*subAssoc == NULL) {
        /* negotiate association */
        AcceptSubAssociation(aNet, subAssoc);
    } else {
        /* be a service class provider */
        SubOpScp(subAssoc, saveDirectoryPath);
    }
}

static OFCondition FindScp(T_ASC_Association * assoc, T_DIMSE_C_FindRQ * request,
	T_ASC_PresentationContextID presID)

{
	OFCondition cond = EC_Normal;
	FindCallbackData context;
	
	context.QueryRetrieveOperationIdentifier = NextOperationIdentifier();
	
	context.priorStatus = STATUS_Pending;	
	ASC_getAPTitles(assoc->params, context.callingAETitle, context.ourAETitle, NULL);
	ASC_getPresentationAddresses(assoc->params, context.callingPresentationAddress, NULL);

	cond = DIMSE_findProvider(assoc, presID, request, 
		FindScpCallback, &context, DIMSE_BLOCKING, 0);

	return cond; 
}

static OFCondition MoveScp(T_ASC_Association * assoc, T_DIMSE_C_MoveRQ * request,
	T_ASC_PresentationContextID presID)

{
	OFCondition cond = EC_Normal;
	MoveCallbackData context;
	context.queryRetrieveOperationIdentifier = NextOperationIdentifier();
	
	context.priorStatus = STATUS_Pending;
	ASC_getAPTitles(assoc->params, context.callingAETitle, context.ourAETitle, NULL);
	ASC_getPresentationAddresses(assoc->params, context.callingPresentationAddress, NULL);

	cond = DIMSE_moveProvider(assoc, presID, request, 
		MoveScpCallback, &context, DIMSE_BLOCKING, 0);

	return cond; 
}


%}
