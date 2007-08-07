
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
		if (status.bad() || (*holderOfString) == NULL)
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

		if (status.bad() || arrayForStringRawBytes == NULL)
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

DcmItem*    findAndGetSequenceItemFromItem(DcmItem& item,
					   const DcmTagKey& seqTagKey,
					   const signed long itemNum=0)
{
	DcmItem *newItem = NULL;
	/* find the item */
        OFCondition status = item.findAndGetSequenceItem(seqTagKey, newItem, itemNum); 
	if (status.good())
	{
            return newItem;
	}

	return NULL;
}

DcmElement*    findAndGetElementFromItem(DcmItem& item,
					 const DcmTagKey& seqTagKey,
					 const OFBool searchIntoSub=OFFalse) 
{
	DcmElement *element = NULL;
	/* find the element */
        OFCondition status = item.findAndGetElement(seqTagKey, element, searchIntoSub); 
	if (status.good())
	{
            return element;
	}

	return NULL;
}

OFCondition getRawStringFromElementGetLength(DcmElement& element, int &lengthRequiredOfArray)
{
        char *dummy_string = "Dummy String";
        char **holderOfString = &dummy_string;

        OFCondition status = element.getString(OFconst_cast(char *&, *holderOfString));

        if (status.bad() || NULL == *holderOfString)
                lengthRequiredOfArray = 0;
        else
                lengthRequiredOfArray = strlen(*holderOfString);

        return status;
}

OFCondition getRawStringFromElement(DcmElement& element, const char *&arrayForStringRawBytes, int &lengthRequiredOfArray)
{
        OFCondition status = element.getString(OFconst_cast(char *&, arrayForStringRawBytes));

        if (status.bad() || NULL == arrayForStringRawBytes)
                lengthRequiredOfArray = 0;
        else
                lengthRequiredOfArray = strlen(arrayForStringRawBytes);

        return status;
}

OFCondition putAndInsertRawStringIntoItem(DcmItem& item, const DcmTagKey& tagKey, const char *arrayOfStringRawBytes)
{
        // TODO
        // I cannot decide whether or not we have to make a copy of
        // arrayOfStringRawBytes so that we can null-terminate it since
        // the conversion using GetEncoding() does not append a
        // null terminator
        OFCondition status = item.putAndInsertString(tagKey, arrayOfStringRawBytes, true);
        return status;
}

%}

namespace std
{
	%template(OFStringVector) vector<string>;
}

