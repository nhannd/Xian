%include "typemaps.i"

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
// This macro is for base types, rather than derived types
//
%define CONTROLACCESSPUBLIC(type)
%typemap(csbody) type %{
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  public type ## (IntPtr cPtr, bool cMemoryOwn) 
  {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  public static HandleRef getCPtr( ## type obj) 
  {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }
%}
%enddef

// 
// This macro is for base types, rather than derived types
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
/*
%define INPUT_ARRAY_TYPEMAP(TYPE, CTYPE, CSTYPE)
%typemap(ctype) TYPE *INPUT_ARRAY "CTYPE"
%typemap(imtype) TYPE *INPUT_ARRAY "CSTYPE"
%typemap(cstype) TYPE *INPUT_ARRAY "CSTYPE"
%typemap(csin) TYPE *INPUT_ARRAY "$csinput"
%typemap(in) TYPE *INPUT_ARRAY %{ $1 = ($1_ltype)$input; %}
%enddef

INPUT_ARRAY_TYPEMAP(const unsigned char,	const unsigned char*,	byte[])
INPUT_ARRAY_TYPEMAP(const short,			const short*,			short[])
INPUT_ARRAY_TYPEMAP(const unsigned short,	const unsigned short*,	ushort[])
INPUT_ARRAY_TYPEMAP(const int,				const int*,				int[])
INPUT_ARRAY_TYPEMAP(const unsigned int,		const unsigned int*,	uint[])
INPUT_ARRAY_TYPEMAP(const long,				const long*,			int[])
INPUT_ARRAY_TYPEMAP(const unsigned long,	const unsigned long*,	uint[])
INPUT_ARRAY_TYPEMAP(const float,			const float*,			float[])
INPUT_ARRAY_TYPEMAP(const double,			const double*,			double[])

%apply const unsigned char *INPUT_ARRAY {Uint8 *};
%apply const unsigned short *INPUT_ARRAY {Uint16 *};
%apply const short *INPUT_ARRAY {Sint16 *};
%apply const unsigned int *INPUT_ARRAY {Uint32 *};
%apply const int *INPUT_ARRAY {Sint32 *};
%apply const float *INPUT_ARRAY {Float32 *};
%apply const double *INPUT_ARRAY {Float64 *};




%apply unsigned char *OUTPUT {Uint8 &};
%apply short *OUTPUT {Sint16 &};
%apply unsigned int *OUTPUT {Uint32 &};
%apply int *OUTPUT {Sint32 &};
%apply int *OUTPUT {long int &};
%apply float *OUTPUT {Float32 &};
%apply double *OUTPUT {Float64 &};
%apply double *OUTPUT {double &};
*/

// 
// This macro defines typemaps that are useful when Offis functions
// take in a reference to a point, usually in order to populate
// an array of raw data, c.f. DcmItem::findAndGetUInt16Array()
//
%define OUTPUT_ARRAY_TYPEMAP(TYPE, CTYPE, CSTYPE, TYPECHECKPRECEDENCE)
%typemap(ctype) TYPE *&OUTPUT_ARRAY "CTYPE"
%typemap(imtype) TYPE *&OUTPUT_ARRAY "ref CSTYPE"
%typemap(cstype) TYPE *&OUTPUT_ARRAY "ref CSTYPE"
%typemap(csin) TYPE *&OUTPUT_ARRAY "ref $csinput"
%typemap(in) TYPE *&OUTPUT_ARRAY %{ $1 = ($1_ltype)$input; %}
%typecheck(SWIG_TYPECHECK_##TYPECHECKPRECEDENCE) TYPE *&OUTPUT ""
%enddef

OUTPUT_ARRAY_TYPEMAP(char,				char**,				IntPtr,		INT8_PTR)
OUTPUT_ARRAY_TYPEMAP(unsigned char,		unsigned char**,	IntPtr,		UINT8_PTR)
OUTPUT_ARRAY_TYPEMAP(short,				short**,			IntPtr,		INT16_PTR)
OUTPUT_ARRAY_TYPEMAP(unsigned short,	unsigned short**,	IntPtr,		UINT16_PTR)
OUTPUT_ARRAY_TYPEMAP(int,				int**,				IntPtr,		INT32_PTR)
OUTPUT_ARRAY_TYPEMAP(unsigned int,		unsigned int**,		IntPtr,		UINT32_PTR)
OUTPUT_ARRAY_TYPEMAP(long,				long**,				IntPtr,		INT32_PTR)
OUTPUT_ARRAY_TYPEMAP(unsigned long,		unsigned long**,	IntPtr,		UINT32_PTR)
OUTPUT_ARRAY_TYPEMAP(float,				float**,			IntPtr,		FLOAT_PTR)
OUTPUT_ARRAY_TYPEMAP(double,			double**,			IntPtr,		DOUBLE_PTR)

#undef OUTPUT_ARRAY_TYPEMAP

//
// Use our own typemaps for Offis types
%apply unsigned short *&OUTPUT_ARRAY { Uint16*& };
%apply unsigned char *&OUTPUT_ARRAY { Uint8*& };


// 
// Use the SWIG library C# typemaps for Offis types
//
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

%apply char *&OUTPUT_ARRAY {char *&};
%apply unsigned char *&OUTPUT_ARRAY {Uint8 *&};
%apply unsigned short *&OUTPUT_ARRAY {Uint16 *&};
%apply short *&OUTPUT_ARRAY {Sint16 *&};
%apply unsigned int *&OUTPUT_ARRAY {Uint32 *&};
%apply int *&OUTPUT_ARRAY {Sint32 *&};
%apply float *&OUTPUT_ARRAY {Float32 *&};
%apply double *&OUTPUT_ARRAY {Float64 *&};

//
// Handle the std::string type
//
%typemap(ctype) std::string& "char *"
%typemap(imtype) std::string& "StringBuilder"
%typemap(cstype) std::string& "StringBuilder" 
%typemap(csin) std::string& "$csinput.Remove(0, $csinput.Length)"

%typemap(in, canthrow=1) std::string&
%{ 
	if (!$input) 
		SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);

	std::string $1_str;
	$1 = &$1_str;
%}


%typemap(argout) std::string&
%{
	if ($1_str.c_str() != 0 && $1_str.length() > 0)
	{
		memcpy($input, $1_str.c_str(), $1_str.length());
		$input[$1_str.length()] = 0;
	}
%}

//
// Make sure that generated C# interface files have the correct
// using import declarations. These using statements will appear
// in all the C# classes that are generated by SWIG.
//
%typemap(csimports) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "\nusing System;\nusing System.Text;\nusing System.Runtime.InteropServices;\n"

// -----------------------------------------------------------
// This typemap adds code to functions that declare
// that they can throw exceptions, so that the exceptions
// will be routed to the appropriate C# exception class
// -----------------------------------------------------------
%typemap(throws, canthrow=1) dicom_runtime_error {
	OFCondition* pcondition = new OFCondition($1._condition);
	SWIG_CSharpSetPendingExceptionDicomRuntime(pcondition, $1.what());
	return $null;
}

%rename(__parameters__) params;
%rename(__baseClass__) base;
%rename(__outStream__) out;

//
// TODO: 
// dcmtk v3.5.4 has header file for DcmMetaInfo that includes an
// assignment operator, but implementation file does not implement it.
// Since SWIG references the header file, I suspect that the signature
// it expects can't be matched with the compiler-generated assignment
// operator, and that's why the linker balks (unresolved reference).
// For now, I'll ignore the DcmMetaInfo assignment operator
%ignore DcmMetaInfo::operator=;
%rename(AssignToMe) operator=; 
%rename(CompareIsEqualTo) operator==;
%rename(CompareIsNotEqualTo) operator!=;
%rename(CompareIsLessThan) operator<;
%rename(CompareIsGreaterThan) operator>;
%rename(CompareIsLessThanOrEqualTo) operator<=;
%rename(CompareIsGreaterThanOrEqualTo) operator>=;
%rename(ReceiveAsInput) operator<<;

%pragma(csharp) moduleimports=%{
using System;
using System.Text;
%}

