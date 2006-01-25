
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

%typemap(ctype) std::string& "char *"
%typemap(imtype) std::string& "StringBuilder"
%typemap(cstype) std::string& "StringBuilder"
%typemap(csin) std::string& "$csinput"

%typemap(in) std::string&
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


%typemap(csimports) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "\nusing System;\nusing System.Text;\nusing System.Runtime.InteropServices;\n"

%pragma(csharp) moduleimports=%{
using System;
using System.Text;
%}

