%module OffisDcm

%{
#pragma warning (disable:4267)
#pragma warning (disable:4800)
%}

////////////////////////////////////////////////////////////
//
// MACRO: Add a constructor to a derived type that allows 
// it to be constructed with a boolean indicating whether
// or not the destructor should call the underlying C++
// delete to deallocate the object, i.e. manually set
// the cMemoryOwn field at construction time.
//
// type is the type that we want to add the method to,
// arg_type is the type of an internal field that is
// used during construction. For example, for 
// DcmCodeString, the arg_type is DcmTag since a
// DcmTag object is passed in to define the Group 
// and Element of the Code String that will be created.
//
%define CONTROLOWNER_DERIVED(type, construct_arg_type)
%typemap(csbody_derived) type %{
  private HandleRef swigCPtr;

  internal type ## (IntPtr cPtr, bool cMemoryOwn) : base(OffisDcmPINVOKE. ## type ## Upcast(cPtr), cMemoryOwn) {
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr( ## type obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  public type ## ( ## construct_arg_type  tag, bool cMemoryOwn) : this(OffisDcmPINVOKE.new_ ## type ## __SWIG_1( ## construct_arg_type ## .getCPtr(tag)), cMemoryOwn) {
    if (OffisDcmPINVOKE.SWIGPendingException.Pending) throw OffisDcmPINVOKE.SWIGPendingException.Retrieve();
  }

%}
%enddef

CONTROLOWNER_DERIVED(DcmCodeString, DcmTag)
CONTROLOWNER_DERIVED(DcmLongString, DcmTag)
CONTROLOWNER_DERIVED(DcmShortString, DcmTag)
CONTROLOWNER_DERIVED(DcmPersonName, DcmTag)
CONTROLOWNER_DERIVED(DcmDate, DcmTag)
CONTROLOWNER_DERIVED(DcmTime, DcmTag)
CONTROLOWNER_DERIVED(DcmUniqueIdentifier, DcmTag)

/////////////////////////////////////////////////////////
//
// MACRO: Change the access modifiers of the constructors 
// so that the creator of the object can manually specify
// cMemoryOwn. This is useful when we manually marshal
// DcmDataset pointers into the C# world; we can 
// simply construct a C# DcmDataset proxy object and give
// the constructor the C-pointer of the real object
// and at the same time, set cMemoryOwn appropriately.
// For example, in the query progress callback, the
// underly C++ DcmDataset object exists only on the stack
// and will be deallocated once the callback returns.
//
%define CONTROLACCESSPUBLIC_DERIVED(type)
%typemap(csbody_derived) type %{
  private HandleRef swigCPtr;

  public type ## (IntPtr cPtr, bool cMemoryOwn) 
  	: base(OffisDcmPINVOKE. ## type ## Upcast(cPtr), cMemoryOwn) {
    swigCPtr = new HandleRef(this, cPtr);
  }

  public static HandleRef getCPtr( ## type  obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }
%}
%enddef

CONTROLACCESSPUBLIC_DERIVED(DcmDataset)

//////////////////////////////////////////////////////////
//
// Typemap to override the ToString function of 
// DcmElement to return a reference to the string object
//
%typemap(csbody_derived) DcmElement %{
	private HandleRef swigCPtr;

	internal DcmElement(IntPtr cPtr, bool cMemoryOwn) 
		: base(OffisDcmPINVOKE.DcmElementUpcast(cPtr), cMemoryOwn) {
		swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(DcmElement obj) {
		return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
	}

	public override string ToString()
	{
		StringBuilder buffer = new StringBuilder(256);
		getOFStringArray(buffer);
		return buffer.ToString();
	}
%}

//////////////////////////////////////////////////////////
//
// Rest of interface file
//
%include "dcmtk/config/osconfig.h"

#ifdef HAVE_CONFIG_H
%include "cfunix.h"
#elif defined(_WIN32)
%include "dcmtk/config/cfwin32.h"
#endif

%include "typemaps.i"
%include "dcm_typemaps.i"
%include "std_string.i"

%apply unsigned char *OUTPUT {Uint8 &};
%apply unsigned short *OUTPUT {Uint16 &};
%apply short *OUTPUT {Sint16 &};
%apply unsigned int *OUTPUT {Uint32 &};
%apply int *OUTPUT {Sint32 &};
%apply int *OUTPUT {long int &};
%apply float *OUTPUT {Float32 &};
%apply double *OUTPUT {Float64 &};
%apply double *OUTPUT {double &};
%apply std::string & {std::string &};

%apply const unsigned char *INPUT_ARRAY {Uint8 *};
%apply const unsigned short *INPUT_ARRAY {Uint16 *};
%apply const short *INPUT_ARRAY {Sint16 *};
%apply const unsigned int *INPUT_ARRAY {Uint32 *};
%apply const int *INPUT_ARRAY {Sint32 *};
%apply const float *INPUT_ARRAY {Float32 *};
%apply const double *INPUT_ARRAY {Float64 *};

%apply char *&OUTPUT_ARRAY {char *&};
%apply unsigned char *&OUTPUT_ARRAY {Uint8 *&};
%apply unsigned short *&OUTPUT_ARRAY {Uint16 *&};
%apply short *&OUTPUT_ARRAY {Sint16 *&};
%apply unsigned int *&OUTPUT_ARRAY {Uint32 *&};
%apply int *&OUTPUT_ARRAY {Sint32 *&};
%apply float *&OUTPUT_ARRAY {Float32 *&};
%apply double *&OUTPUT_ARRAY {Float64 *&};

%include "ofstd.i"

%{

#include "dcmtk/config/osconfig.h"
#ifdef HAVE_CONFIG_H
#include "cfunix.h"
#elif defined(_WIN32)
#include "dcmtk/config/cfwin32.h"
#endif

// various headers
#include "dcmtk/dcmdata/dctypes.h"
#include "dcmtk/dcmdata/dcswap.h"
#include "dcmtk/dcmdata/dcistrma.h"
#include "dcmtk/dcmdata/dcostrma.h"
#include "dcmtk/dcmdata/dcvr.h"
#include "dcmtk/dcmdata/dcxfer.h"
#include "dcmtk/dcmdata/dcuid.h"
#include "dcmtk/dcmdata/dcvm.h"
#include "dcmtk/dcmdata/dcdefine.h"
#include "dcmtk/dcmdata/dcdebug.h"

// tags and dictionary
#include "dcmtk/ofstd/oflist.h"
#include "dcmtk/dcmdata/dctagkey.h"
#include "dcmtk/dcmdata/dctag.h"
#include "dcmtk/dcmdata/dcdicent.h"
#include "dcmtk/dcmdata/dchashdi.h"
#include "dcmtk/dcmdata/dcdict.h"
#include "dcmtk/dcmdata/dcdeftag.h"

// basis classes
#include "dcmtk/dcmdata/dcobject.h"
#include "dcmtk/dcmdata/dcelem.h"

// classes for management of sequences and other lists
#include "dcmtk/dcmdata/dcitem.h"
#include "dcmtk/dcmdata/dcmetinf.h"
#include "dcmtk/dcmdata/dcdatset.h"
#include "dcmtk/dcmdata/dcsequen.h"
#include "dcmtk/dcmdata/dcfilefo.h"
#include "dcmtk/dcmdata/dcdicdir.h"
#include "dcmtk/dcmdata/dcpixseq.h"

// element classes for string management (8-bit)
#include "dcmtk/dcmdata/dcbytstr.h"
#include "dcmtk/dcmdata/dcvrae.h"
#include "dcmtk/dcmdata/dcvras.h"
#include "dcmtk/dcmdata/dcvrcs.h"
#include "dcmtk/dcmdata/dcvrda.h"
#include "dcmtk/dcmdata/dcvrds.h"
#include "dcmtk/dcmdata/dcvrdt.h"
#include "dcmtk/dcmdata/dcvris.h"
#include "dcmtk/dcmdata/dcvrtm.h"
#include "dcmtk/dcmdata/dcvrui.h"

// element classes for string management (8-bit and/or 16-bit in later extensions)
#include "dcmtk/dcmdata/dcchrstr.h"
#include "dcmtk/dcmdata/dcvrlo.h"
#include "dcmtk/dcmdata/dcvrlt.h"
#include "dcmtk/dcmdata/dcvrpn.h"
#include "dcmtk/dcmdata/dcvrsh.h"
#include "dcmtk/dcmdata/dcvrst.h"
#include "dcmtk/dcmdata/dcvrut.h"

// element class for byte and word value representations
#include "dcmtk/dcmdata/dcvrobow.h"
#include "dcmtk/dcmdata/dcpixel.h"
#include "dcmtk/dcmdata/dcovlay.h"

// element classes for binary value fields
#include "dcmtk/dcmdata/dcvrat.h"
#include "dcmtk/dcmdata/dcvrss.h"
#include "dcmtk/dcmdata/dcvrus.h"
#include "dcmtk/dcmdata/dcvrsl.h"
#include "dcmtk/dcmdata/dcvrul.h"
#include "dcmtk/dcmdata/dcvrulup.h"
#include "dcmtk/dcmdata/dcvrfl.h"
#include "dcmtk/dcmdata/dcvrfd.h"
#include "dcmtk/dcmdata/dcvrof.h"
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

// Ignore these for now, since they are causing problems
%ignore getOriginalRepresentationKey;
%ignore getCurrentRepresentationKey;
%ignore normalizeString;
%ignore DcmDictEntryList;

%rename(__assign__) DcmElement::operator=;

// various headers
%include "dcmtk/dcmdata/dctypes.h"
%include "dcmtk/dcmdata/dcswap.h"
%include "dcmtk/dcmdata/dcistrma.h"
%include "dcmtk/dcmdata/dcostrma.h"
%include "dcmtk/dcmdata/dcvr.h"
%include "dcmtk/dcmdata/dcxfer.h"
%include "dcmtk/dcmdata/dcuid.h"
%include "dcmtk/dcmdata/dcvm.h"
%include "dcmtk/dcmdata/dcdefine.h"
%include "dcmtk/dcmdata/dcdebug.h"

// tags and dictionary
%include "dcmtk/ofstd/oflist.h"
%include "dcmtk/dcmdata/dctagkey.h"
%include "dcmtk/dcmdata/dctag.h"
%include "dcmtk/dcmdata/dcdicent.h"
%include "dcmtk/dcmdata/dchashdi.h"
%include "dcmtk/dcmdata/dcdict.h"
%include "dcmtk/dcmdata/dcdeftag.h"

// basis classes
%include "dcmtk/dcmdata/dcobject.h"
%include "dcmtk/dcmdata/dcelem.h"

// classes for management of sequences and other lists
%include "dcmtk/dcmdata/dcitem.h"
%include "dcmtk/dcmdata/dcmetinf.h"
%include "dcmtk/dcmdata/dcdatset.h"
%include "dcmtk/dcmdata/dcsequen.h"
%include "dcmtk/dcmdata/dcfilefo.h"
%include "dcmtk/dcmdata/dcdicdir.h"
%include "dcmtk/dcmdata/dcpixseq.h"

// element classes for string management (8-bit)
%include "dcmtk/dcmdata/dcbytstr.h"
%include "dcmtk/dcmdata/dcvrae.h"
%include "dcmtk/dcmdata/dcvras.h"
%include "dcmtk/dcmdata/dcvrcs.h"
%include "dcmtk/dcmdata/dcvrda.h"
%include "dcmtk/dcmdata/dcvrds.h"
%include "dcmtk/dcmdata/dcvrdt.h"
%include "dcmtk/dcmdata/dcvris.h"
%include "dcmtk/dcmdata/dcvrtm.h"
%include "dcmtk/dcmdata/dcvrui.h"

// element classes for string management (8-bit and/or 16-bit in later extensions)
%include "dcmtk/dcmdata/dcchrstr.h"
%include "dcmtk/dcmdata/dcvrlo.h"
%include "dcmtk/dcmdata/dcvrlt.h"
%include "dcmtk/dcmdata/dcvrpn.h"
%include "dcmtk/dcmdata/dcvrsh.h"
%include "dcmtk/dcmdata/dcvrst.h"
%include "dcmtk/dcmdata/dcvrut.h"

// element class for byte and word value representations
%include "dcmtk/dcmdata/dcvrobow.h"
%include "dcmtk/dcmdata/dcpixel.h"
%include "dcmtk/dcmdata/dcovlay.h"

// element classes for binary value fields
%include "dcmtk/dcmdata/dcvrat.h"
%include "dcmtk/dcmdata/dcvrss.h"
%include "dcmtk/dcmdata/dcvrus.h"
%include "dcmtk/dcmdata/dcvrsl.h"
%include "dcmtk/dcmdata/dcvrul.h"
%include "dcmtk/dcmdata/dcvrulup.h"
%include "dcmtk/dcmdata/dcvrfl.h"
%include "dcmtk/dcmdata/dcvrfd.h"
%include "dcmtk/dcmdata/dcvrof.h"

%include "dcmnet.i"
