
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


static void CFindProgressCallback(
        void *callbackData,
        T_DIMSE_C_FindRQ *request,
        int responseCount,
        T_DIMSE_C_FindRSP *rsp,
        DcmDataset *responseIdentifiers
        )
{
	if (NULL != CSharpQueryCallbackHelperCallback)
	{
		CSharpQueryCallbackHelperCallback(callbackData,
											request,
											responseCount,
											rsp,
											responseIdentifiers);
	}
}

static void FindScpCallback(
	/* in */ 
	void *callbackData,  
	OFBool cancelled, T_DIMSE_C_FindRQ *request, 
	DcmDataset *requestIdentifiers, int responseCount,
	/* out */
	T_DIMSE_C_FindRSP *response,
	DcmDataset **responseIdentifiers,
	DcmDataset **statusDetail)
{
    OFCondition dbcond = EC_Normal;
    FindCallbackData *context = (FindCallbackData*)callbackData;

	// Build info to pass back to the Callback
	InteropFindScpCallbackInfo info;
	bzero((char*)&info, sizeof(info));

	if (*responseIdentifiers == NULL)
		*responseIdentifiers = new DcmDataset();

	if (*statusDetail == NULL)
		*statusDetail = new DcmDataset();
		
	// prepare the transmission of data
	info.CallingAETitle = context->callingAETitle;
	info.CallingPresentationAddress = context->callingPresentationAddress;
	info.Request = request;
	info.Response = response;
	info.RequestIdentifiers = requestIdentifiers; 
	info.ResponseIdentifiers = *responseIdentifiers;
	info.StatusDetail = *statusDetail;

	if (responseCount == 1) 
	{
        // start the database search 
		if (NULL != CSharpFindScpCallbackHelper_QueryDBCallback)
		{
			CSharpFindScpCallbackHelper_QueryDBCallback(&info);	
		}
		else
		{
			response->DimseStatus = STATUS_FIND_Refused_OutOfResources;
			
			if (*responseIdentifiers != NULL)
			{
				delete *responseIdentifiers;
				*responseIdentifiers = NULL;
			}

			if (*statusDetail == NULL)
				*statusDetail = new DcmDataset();

			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-FIND missing.");
		}
	}
    
    // cancel was requested, cancel the find
    if (cancelled) 
	{
		// Not implemented
    }

    if (DICOM_PENDING_STATUS(context->priorStatus)) 
	{
		if (NULL != CSharpFindScpCallbackHelper_GetNextFindResponseCallback)
		{
			// find the next matching response
			CSharpFindScpCallbackHelper_GetNextFindResponseCallback(&info);
			if (response->DimseStatus != STATUS_Pending)
			{
				// *responseIdentifiers MUST be NULL
				// Otherwise there will be problem releasing association
				if (*responseIdentifiers != NULL)
				{
					delete *responseIdentifiers;
					*responseIdentifiers = NULL;
				}
			}

			if (response->DimseStatus == STATUS_Pending ||
				response->DimseStatus == STATUS_Success)
			{
				delete *statusDetail;
				*statusDetail = NULL;
			}

			if (*responseIdentifiers != NULL) 
			{
				AddRetrieveAETitle(*responseIdentifiers, context->ourAETitle);
			}
		}
		else
		{
			response->DimseStatus = STATUS_FIND_Refused_OutOfResources;
			
			if (*responseIdentifiers != NULL)
			{
				delete *responseIdentifiers;
				*responseIdentifiers = NULL;
			}

			if (*statusDetail == NULL)
				*statusDetail = new DcmDataset();

			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-FIND missing.");
		}
    }
}

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
	if (NULL != CSharpStoreScuCallbackHelperCallback)
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

	StoreCallbackData *cbdata = (StoreCallbackData*) callbackData;
	char* fileName = cbdata->imageFileName;

	if (*statusDetail == NULL)
		*statusDetail = new DcmDataset();

	// prepare the transmission of data 
	InteropStoreScpCallbackInfo info;
	bzero((char*)&info, sizeof(info));
	info.FileName = fileName;
	info.ImageDataset = (*imageDataSet);
	info.Progress = progress;
	info.Request = req;

	if (progress->state == DIMSE_StoreBegin)
	{
		// should fire off image store begin event
		if (NULL != CSharpStoreScpCallbackHelper_StoreBeginCallback)
			CSharpStoreScpCallbackHelper_StoreBeginCallback(&info);
	}
	else if (progress->state == DIMSE_StoreProgressing)
	{
		// should fire off image store progressing event
		if (NULL != CSharpStoreScpCallbackHelper_StoreProgressCallback)
			CSharpStoreScpCallbackHelper_StoreProgressCallback(&info);	
	}
	else if (progress->state == DIMSE_StoreEnd)
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
			if (NULL != CSharpStoreScpCallbackHelper_StoreEndCallback)
				CSharpStoreScpCallbackHelper_StoreEndCallback(&info);
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

	// Build info to pass back to the Callback
	InteropMoveScpCallbackInfo info;
	bzero((char*)&info, sizeof(info));

	if (*responseIdentifiers == NULL)
		*responseIdentifiers = new DcmDataset();

	if (*statusDetail == NULL)
		*statusDetail = new DcmDataset();

	// prepare the transmission of data 
	info.CallingAETitle = context->callingAETitle;
	info.CallingPresentationAddress = context->callingPresentationAddress;
	info.Cancelled = cancelled;
	info.Request = request;
	info.Response = response;
	info.RequestIdentifiers = requestIdentifiers; 
	info.ResponseIdentifiers = *responseIdentifiers;
	info.StatusDetail = *statusDetail;

    if (responseCount == 1) 
	{
        // start the database search
		if (NULL != CSharpMoveScpCallbackHelper_MoveBeginCallback)
		{
			CSharpMoveScpCallbackHelper_MoveBeginCallback(&info);	
		}
		else
		{
			response->DimseStatus = STATUS_MOVE_Refused_OutOfResourcesNumberOfMatches;
			
			if (*responseIdentifiers != NULL)
			{
				delete *responseIdentifiers;
				*responseIdentifiers = NULL;
			}

			if (*statusDetail == NULL)
				*statusDetail = new DcmDataset();

			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-MOVE missing.");
		}
    }
    
    if (cancelled) 
	{
	    // cancel was requested, cancel the move

    }

    if (DICOM_PENDING_STATUS(context->priorStatus)) 
	{
		// find the next matching response
		
		// move the next matching response
		if (NULL != CSharpMoveScpCallbackHelper_MoveNextResponseCallback)
		{
			CSharpMoveScpCallbackHelper_MoveNextResponseCallback(&info);
			if (response->DimseStatus != STATUS_Pending)
			{
				// *responseIdentifiers MUST be NULL
				// Otherwise there will be problem releasing association
				if (*responseIdentifiers != NULL)
				{
					delete *responseIdentifiers;
					*responseIdentifiers = NULL;
				}
			}
			
			if (response->DimseStatus == STATUS_Pending ||
				response->DimseStatus == STATUS_Success)
			{
				if (*statusDetail != NULL)
					delete *statusDetail;
				*statusDetail = NULL;
			}
			
			if (*responseIdentifiers != NULL) 
			{
				AddRetrieveAETitle(*responseIdentifiers, context->ourAETitle);
			}

		}
		else
		{
			response->DimseStatus = STATUS_MOVE_Refused_OutOfResourcesNumberOfMatches;
			
			if (*responseIdentifiers != NULL)
			{
				delete *responseIdentifiers;
				*responseIdentifiers = NULL;
			}

			if (*statusDetail == NULL)
				*statusDetail = new DcmDataset();

			(*statusDetail)->putAndInsertString(DCM_ErrorComment, "User-defined callback function for C-FIND missing.");
		}
    }
}

%}
