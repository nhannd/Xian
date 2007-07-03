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

//Make the default constructor private, the factory methods in T_ASC_Network should be used instead.
%csmethodmodifiers T_ASC_Parameters::T_ASC_Parameters() "private";

%extend(canthrow=1) T_ASC_Parameters {

	T_ASC_Parameters() throw (dicom_runtime_error)
	{
		OFCondition cond = EC_Normal;
		throw new dicom_runtime_error(cond, "T_ASC_Parameters should be created via the T_ASC_Network factory method(s)");
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
}

