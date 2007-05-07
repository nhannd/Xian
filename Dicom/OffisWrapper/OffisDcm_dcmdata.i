
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
// 
OFCondition findAndGetRawStringFromItem(DcmItem& item,
								  const DcmTagKey& tagKey,
								  const char *&value,
								  int &length,
								  const OFBool searchIntoSub)
{
	DcmElement *elem;
	/* find the element */
	OFCondition status = item.findAndGetElement(tagKey, elem, searchIntoSub);
	if (status.good())
	{
		/* get the value */
		status = elem->getString(OFconst_cast(char *&, value));
	}

	/* reset value */
	if (status.bad())
	{
		value = NULL;
		length = 0;
	}
	else
	{
		length = strlen(value);
	}
	return status;
}

%}

namespace std
{
	%template(OFStringVector) vector<string>;
}

