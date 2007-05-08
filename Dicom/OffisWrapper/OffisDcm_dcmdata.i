
%{
#include "dcmtk/config/osconfig.h"
#include "dcmtk/ofstd/ofstring.h"
#include "dcmtk/dcmdata/dcobject.h"
#include "dcmtk/dcmdata/dcitem.h"
#include "dcmtk/dcmdata/dcelem.h"
#include "dcmtk/dcmdata/dcsequen.h"
#include "dcmtk/dcmdata/dctagkey.h"
#include "dcmtk/dcmdata/dcdatset.h"
#include "dcmtk/dcmdata/dcmetinf.h"
#include "dcmtk/dcmdata/dcfilefo.h"
#include "dcmtk/dcmdata/dcxfer.h"
#include "dcmtk/dcmdata/dctypes.h"
#include "dcmtk/dcmdata/dctag.h"
#include "dcmtk/dcmdata/dcdeftag.h"
#include "dcmtk/dcmdata/dcdict.h"
#include "dcmtk/dcmdata/dcdicent.h"
%}

%include "OffisDcm_typemaps.i"
%include "dcmtk/ofstd/ofstring.h"
%include "dcmtk/dcmdata/dcobject.h"
%include "dcmtk/dcmdata/dcitem.h"
%include "dcmtk/dcmdata/dcelem.h"
%include "dcmtk/dcmdata/dcsequen.h"
%include "dcmtk/dcmdata/dctagkey.h"
%include "dcmtk/dcmdata/dcdatset.h"
%include "dcmtk/dcmdata/dcmetinf.h"
%include "dcmtk/dcmdata/dcfilefo.h"
%include "dcmtk/dcmdata/dcxfer.h"
%include "dcmtk/dcmdata/dctypes.h"
%include "dcmtk/dcmdata/dctag.h"
%include "dcmtk/dcmdata/dcdict.h"
%include "dcmtk/dcmdata/dcdicent.h"
%include "dcmtk/dcmdata/dcvr.h"

CONTROLOWNER_DERIVED(DcmCodeString, DcmTag)
CONTROLOWNER_DERIVED(DcmLongString, DcmTag)
CONTROLOWNER_DERIVED(DcmShortString, DcmTag)
CONTROLOWNER_DERIVED(DcmPersonName, DcmTag)
CONTROLOWNER_DERIVED(DcmDate, DcmTag)
CONTROLOWNER_DERIVED(DcmTime, DcmTag)
CONTROLOWNER_DERIVED(DcmUniqueIdentifier, DcmTag)

%inline
%{

DcmElement* castToDcmElement(DcmObject* pObj)
{
	return dynamic_cast<DcmElement*> (pObj);
}

//
// Helper function to get raw strings
// Pass in NULL for value, to get back just
// the length of the string
//
OFCondition findAndGetRawStringFromItemGetLength(DcmItem& item,
								  const DcmTagKey& tagKey,
								  int &lengthRequiredOfArray,
								  const OFBool searchIntoSub)
{
	DcmElement *elem;
	/* find the element */
	OFCondition status = item.findAndGetElement(tagKey, elem, searchIntoSub);
	if (status.good())
	{
		char *dummy_string = "Dummy String";
		char **holderOfString = &dummy_string;
		status = elem->getString(OFconst_cast(char *&, *holderOfString));
		if (status.bad())
			lengthRequiredOfArray = 0;
		else
			lengthRequiredOfArray = strlen(*holderOfString) + 1; 
	}

	return status;
}

OFCondition findAndGetRawStringFromItem(DcmItem& item,
								  const DcmTagKey& tagKey,
								  const char *&arrayForStringRawBytes,
								  int &lengthRequiredOfArray,
								  const OFBool searchIntoSub)
{
	DcmElement *elem;
	/* find the element */
	OFCondition status = item.findAndGetElement(tagKey, elem, searchIntoSub);
	if (status.good())
	{
		/* get the value */
		status = elem->getString(OFconst_cast(char *&, arrayForStringRawBytes));

		if (status.bad())
		{
			arrayForStringRawBytes = NULL;
			lengthRequiredOfArray = 0;
		}
		else
		{
			lengthRequiredOfArray = strlen(arrayForStringRawBytes) + 1;
		}
	}

	return status;
}

%}

namespace std
{
	%template(OFStringVector) vector<string>;
}

