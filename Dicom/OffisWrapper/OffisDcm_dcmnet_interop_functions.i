
%{

//
// HACK:
// Forward declaration of something that currently exists in OffisDcm_dcmnet_utilities_functions.i
//
static void
AddRetrieveAETitle(DcmDataset *rspIds, DIC_AE ourAETitle);
//
//
// ------------------------------------------------------------------------------------
// DEPENDS: These functions are dependent on the typdefs defined above.
// ------------------------------------------------------------------------------------
//
//
// Hook back into the managed world realm to
// indicate progress for StoreScu operations.
//
static void
StoreScuProgressCallback(void * progressInfo,
    T_DIMSE_StoreProgress *progress,
    T_DIMSE_C_StoreRQ * req)
{
	// should fire off image received event
	if (NULL != CSharpStoreCallbackHelperCallback)
	{
		InteropStoreScuFileCountProgressInfo* pInfo = (InteropStoreScuFileCountProgressInfo*) progressInfo;
		InteropStoreScuCallbackInfo info;

		// prepare the transmission of data 
		bzero((char*)&info, sizeof(info));
		info.Progress = progress;
		info.Request = req;
		info.TotalCount = pInfo->TotalCount;
		info.CurrentCount = pInfo->CurrentCount;

		CSharpStoreScuCallbackHelperCallback(&info);
	}
}

static void 
MoveProgressCallback(void *callbackData, 
		T_DIMSE_C_MoveRQ *request,
    	int responseCount, 
		T_DIMSE_C_MoveRSP *response)
{
	// should fire off image received event
	if (NULL != CSharpRetrieveCallbackHelperCallback)
	{
		InteropRetrieveCallbackInfo info;

		// prepare the transmission of data 
		bzero((char*)&info, sizeof(info));
		info.CMoveResponse = response;

		CSharpRetrieveCallbackHelperCallback(&info);
	}
}

static void StoreScpCallback(
    /* in */
    void *callbackData,
    T_DIMSE_StoreProgress *progress,    /* progress state */
    T_DIMSE_C_StoreRQ *req,             /* original store request */
    char *imageFileName, DcmDataset **imageDataSet, /* being received into */
    /* out */
    T_DIMSE_C_StoreRSP *rsp,            /* final store response */
    DcmDataset **statusDetail)
{
    DIC_UI sopClass;
    DIC_UI sopInstance;

	if (progress->state == DIMSE_StoreEnd)
	{
		*statusDetail = NULL;    /* no status detail */

		/* could save the image somewhere else, put it in database, etc */
		/*
		* An appropriate status code is already set in the resp structure, it need not be success.
		* For example, if the caller has already detected an out of resources problem then the
		* status will reflect this.  The callback function is still called to allow cleanup.
		*/
		// rsp->DimseStatus = STATUS_Success;

		if ((imageDataSet)&&(*imageDataSet))
		{
			StoreCallbackData *cbdata = (StoreCallbackData*) callbackData;
			const char* fileName = cbdata->imageFileName;

			E_TransferSyntax xfer = (*imageDataSet)->getOriginalXfer();

			OFCondition cond = cbdata->dcmff->saveFile(fileName, xfer, EET_ExplicitLength, EGL_recalcGL,
				EPD_withoutPadding, (Uint32)0, (Uint32)0, false);

			if (cond.bad())
			{
				rsp->DimseStatus = STATUS_STORE_Refused_OutOfResources;
			}

			/* should really check the image to make sure it is consistent,
			* that its sopClass and sopInstance correspond with those in
			* the request.
			*/
			if ((rsp->DimseStatus == STATUS_Success))
			{
				/* which SOP class and SOP instance ? */
				if (! DU_findSOPClassAndInstanceInDataSet(*imageDataSet, sopClass, sopInstance, true))
				{
					rsp->DimseStatus = STATUS_STORE_Error_CannotUnderstand;
				}
				else if (strcmp(sopClass, req->AffectedSOPClassUID) != 0)
				{
					rsp->DimseStatus = STATUS_STORE_Error_DataSetDoesNotMatchSOPClass;
				}
				else if (strcmp(sopInstance, req->AffectedSOPInstanceUID) != 0)
				{
					rsp->DimseStatus = STATUS_STORE_Error_DataSetDoesNotMatchSOPClass;
				}
			}

			// should fire off image received event
			if (NULL != CSharpStoreCallbackHelperCallback)
			{
				InteropStoreCallbackInfo info;

				// prepare the transmission of data 
				bzero((char*)&info, sizeof(info));
				info.FileName = fileName;
				info.ImageDataset = (*imageDataSet);

				CSharpStoreCallbackHelperCallback(&info);
			}
		}
	}
    return;
}

static void MoveScpCallback(
	/* in */ 
	void *callbackData,  
	OFBool cancelled, T_DIMSE_C_MoveRQ *request, 
	DcmDataset *requestIdentifiers, int responseCount,
	/* out */
	T_DIMSE_C_MoveRSP *response,
	DcmDataset **responseIdentifiers,
	DcmDataset **statusDetail)
{
    OFCondition dbcond = EC_Normal;
    MoveCallbackData *context;

    context = (MoveCallbackData*)callbackData;	/* recover context */

    if (responseCount == 1) 
	{
        // start the database search 
		// dbcond = DB_startFindRequest(context->dbHandle, STATUS_FIND_Refused_OutOfResources
		//		request->AffectedSOPClassUID, requestIdentifiers, &dbStatus);

    }
    
    // cancel was requested, cancel the find
    if (cancelled) 
	{

    }

    if (DICOM_PENDING_STATUS(context->priorStatus)) 
	{
		// find the next matching response
		//dbcond = DB_nextFindResponse(context->dbHandle,
		//		responseIdentifiers, &dbStatus);
		//
		
		// should fire off image received event
		if (NULL != CSharpMoveCallbackHelperCallback)
		{
			InteropMoveCallbackInfo info;
			bzero((char*)&info, sizeof(info));

			// prepare the transmission of data 

			CSharpMoveCallbackHelperCallback(&info);
		}
		else
		{
			*responseIdentifiers = NULL;
			response->DimseStatus = STATUS_MOVE_Refused_OutOfResourcesNumberOfMatches;
			*statusDetail = new DcmDataset();
			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-MOVE missing.");
			return;
		}
    }

    if (*responseIdentifiers != NULL) 
	{
		AddRetrieveAETitle(*responseIdentifiers, context->ourAETitle);
    }

	// set the response status, i.e. whether there are more results 
	// and the status detail
    // response->DimseStatus = dbStatus.status;
    // *statusDetail = dbStatus.statusDetail;

}

%}
