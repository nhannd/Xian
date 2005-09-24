///////////////////////////////////////////////////////////////////////////////
//
// SWIG Interface file for dcmdata library
//
// $Log: dcmdata.i,v $
// Revision 1.2  2004/11/15 21:02:01  Norman
// Initial version
//
//
///////////////////////////////////////////////////////////////////////////////


%{
// various headers
#include "dctypes.h"
#include "dcswap.h"
#include "dcistrma.h"
#include "dcostrma.h"
#include "dcvr.h"
#include "dcxfer.h"
#include "dcuid.h"
#include "dcvm.h"
#include "dcdefine.h"
#include "dcdebug.h"

// tags and dictionary
#include "dctagkey.h"
#include "dctag.h"
#include "dcdicent.h"
#include "dchashdi.h"
#include "dcdict.h"
#include "dcdeftag.h"

// basis classes
#include "dcobject.h"
#include "dcelem.h"

// classes for management of sequences and other lists
#include "dcitem.h"
#include "dcmetinf.h"
#include "dcdatset.h"
#include "dcsequen.h"
#include "dcfilefo.h"
#include "dcdicdir.h"
#include "dcpixseq.h"

// element classes for string management (8-bit)
#include "dcbytstr.h"
#include "dcvrae.h"
#include "dcvras.h"
#include "dcvrcs.h"
#include "dcvrda.h"
#include "dcvrds.h"
#include "dcvrdt.h"
#include "dcvris.h"
#include "dcvrtm.h"
#include "dcvrui.h"

// element classes for string management (8-bit and/or 16-bit in later extensions)
#include "dcchrstr.h"
#include "dcvrlo.h"
#include "dcvrlt.h"
#include "dcvrpn.h"
#include "dcvrsh.h"
#include "dcvrst.h"
#include "dcvrut.h"

// element class for byte and word value representations
#include "dcvrobow.h"
#include "dcpixel.h"
#include "dcovlay.h"

// element classes for binary value fields
#include "dcvrat.h"
#include "dcvrss.h"
#include "dcvrus.h"
#include "dcvrsl.h"
#include "dcvrul.h"
#include "dcvrulup.h"
#include "dcvrfl.h"
#include "dcvrfd.h"
#include "dcvrof.h"
%}

%inline
%{

DcmElement* castToDcmElement(DcmObject* pObj)
{
	return dynamic_cast<DcmElement*> (pObj);
}

%}


// "out" is a keyword in C#, so rename it to "outStream"
#define out outStream

// "value" is a keyword in C#, so rename it to "val"
// This will generate two compiler errors.  They are fixed by editing the
// dcmtk_wrap.cxx file so that "val" is changed back to "value".
#define value val

// Ignore these for now, since they're causing problems
%ignore getOriginalRepresentationKey;
%ignore getCurrentRepresentationKey;
%ignore normalizeString;


// various headers
%include "dctypes.h"
%include "dcswap.h"
%include "dcistrma.h"
%include "dcostrma.h"
%include "dcvr.h"
%include "dcxfer.h"
%include "dcuid.h"
%include "dcvm.h"
%include "dcdefine.h"
%include "dcdebug.h"

// tags and dictionary
%include "dctagkey.h"
%include "dctag.h"
%include "dcdicent.h"
%include "dchashdi.h"
%include "dcdict.h"
%include "dcdeftag.h"

// basis classes
%include "dcobject.h"
%include "dcelem.h"

// classes for management of sequences and other lists
%include "dcitem.h"
%include "dcmetinf.h"
%include "dcdatset.h"
%include "dcsequen.h"
%include "dcfilefo.h"
%include "dcdicdir.h"
%include "dcpixseq.h"

// element classes for string management (8-bit)
%include "dcbytstr.h"
%include "dcvrae.h"
%include "dcvras.h"
%include "dcvrcs.h"
%include "dcvrda.h"
%include "dcvrds.h"
%include "dcvrdt.h"
%include "dcvris.h"
%include "dcvrtm.h"
%include "dcvrui.h"

// element classes for string management (8-bit and/or 16-bit in later extensions)
%include "dcchrstr.h"
%include "dcvrlo.h"
%include "dcvrlt.h"
%include "dcvrpn.h"
%include "dcvrsh.h"
%include "dcvrst.h"
%include "dcvrut.h"

// element class for byte and word value representations
%include "dcvrobow.h"
%include "dcpixel.h"
%include "dcovlay.h"

// element classes for binary value fields
%include "dcvrat.h"
%include "dcvrss.h"
%include "dcvrus.h"
%include "dcvrsl.h"
%include "dcvrul.h"
%include "dcvrulup.h"
%include "dcvrfl.h"
%include "dcvrfd.h"
%include "dcvrof.h"
