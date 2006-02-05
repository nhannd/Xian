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
%include "osconfig.h"
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

#include "osconfig.h"

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

%rename(__assign__) DcmElement::operator=;

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

%include "dcmnet.i"
