%{
// ---------------------------------------------
// structs used for internal utilities function
// ---------------------------------------------
struct StoreCallbackData
{
  unsigned long StoreOperationIdentifier;
  char* imageFileName;
  DcmFileFormat* dcmff;
  T_ASC_Association* assoc;
};

struct FindCallbackData
{
	unsigned long QueryRetrieveOperationIdentifier;
	DIC_US priorStatus;
	DIC_AE ourAETitle;
	DIC_AE callingAETitle;
    char callingPresentationAddress[64];
};

struct MoveCallbackData
{
	unsigned long queryRetrieveOperationIdentifier;
	DIC_US priorStatus;
    DIC_AE ourAETitle;  	/* our current title */
    DIC_AE dstAETitle;		/* destination title for move */
	DIC_AE callingAETitle;
    char callingPresentationAddress[64];

    T_ASC_Association   *origAssoc;	/* association of requestor */
    T_ASC_Association   *subAssoc;	/* sub-association */

    OFBool assocStarted;	/* true if the association was started */
    
    DIC_US origMsgId;		/* message id of request */
    DIC_AE origAETitle;		/* title of requestor */
    DIC_NODENAME origHostName;	/* hostname of move requestor */

    T_DIMSE_Priority priority;	/* priority of move request */

    char *failedUIDs;		/* instance UIDs of failed store sub-ops */

    DIC_US nRemaining; 
    DIC_US nCompleted; 
    DIC_US nFailed; 
    DIC_US nWarning;
};

%}
