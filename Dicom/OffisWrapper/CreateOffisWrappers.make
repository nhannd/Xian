##################################################################
#
# Declaration of variables
# 
##################################################################
Configuration = Debug
SwigOutputCsharpDir = csharp
CppCompilerOutputDirRoot = cppwrapper
CppCompilerOutputDir = $(CppCompilerOutputDirRoot)\$(Configuration)
CppCompiler = cl.exe
SwigCompiler = swig.exe
SwigOutputNamespace = ClearCanvas.Dicom.OffisWrapper
CppCompilerParameters = /EHsc /TP /D "WIN32" /D "_WINDOWS" \
	/D "_CRT_SECURE_NO_DEPRECATE" /D "SWIG" /D "WITH_LIBPNG" /D "WITH_LIBTIFF" \
	/D "WITH_ZLIB" /D "_WINDLL" /D "MBCS" /D "HAVE_STD_STRING" \
	/D "HAVE_CXX_BOOL" /c
CppCompilerIncludeParameters = /I "..\..\OFFIS\dcmtk-3.5.3\config\include" \
	/I "..\..\OFFIS\dcmtk-3.5.3\ofstd\include" \
	/I "..\..\OFFIS\dcmtk-3.5.3\dcmdata\include" \
	/I "..\..\OFFIS\dcmtk-3.5.3\dcmnet\include" \
	/I "..\..\OFFIS\dcmtk-3.5.3\ofstd\include" \
	/I "..\..\OFFIS\dcmtk-3.5.3\dcmdata\include" \
	/I "..\..\OFFIS\dcmtk-3.5.3\zlib-1.2.1\include" 
CppLinkerObjectFiles = ofstd.lib dcmdata.lib dcmnet.lib ws2_32.lib \
	netapi32.lib kernel32.lib user32.lib gdi32.lib winspool.lib \
	comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib \
	uuid.lib odbc32.lib odbccp32.lib
ifeq ($(Configuration), Debug)
	CppCompilerDebugParameters = /Od /ZI /MTd
	CppLinkerParameters = /LIBPATH:"..\..\OFFIS\dcmtk-3.5.3\ofstd\libsrc\debug" \
	/LIBPATH:"..\..\OFFIS\dcmtk-3.5.3\dcmdata\libsrc\debug"  \
	/LIBPATH:"..\..\OFFIS\dcmtk-3.5.3\dcmnet\libsrc\debug" \
	/LIBPATH:"..\..\OFFIS\zlib-1.2.1\lib" /DLL 
	CppLinkerDebugParameters = /DEBUG 
	CppLinkerZlibFile = zlib_d.lib
	CscEmitDebugInformation = +
else
	CppCompilerDebugParameters = /O2 /MT
	CppLinkerParameters = /LIBPATH:"..\..\OFFIS\dcmtk-3.5.3\ofstd\libsrc\release" /LIBPATH:"..\..\OFFIS\dcmtk-3.5.3\dcmdata\libsrc\release" /LIBPATH:"..\..\OFFIS\dcmtk-3.5.3\dcmnet\libsrc\release" \
	/LIBPATH:"..\..\OFFIS\zlib-1.2.1\lib" /DLL 
	CppLinkerDebugParameters = 
	CppLinkerZlibFile = zlib_o.lib
	CscEmitDebugInformation = -
endif
SwigInterfaceFile = dcmdata.i
SwigCppOutputFile = Offis_Wrapper.cxx
VPATH = $(CppCompilerOutputDir):$(SwigOutputCSharpDir)\$(Configuration)

##################################################################
#
# Declaration of rules
# 
##################################################################

Offis_Wrapper.cxx : dcmdata.i dcmnet.i dcm_typemaps.i ofstd.i
	if exist $(SwigOutputCsharpDir). (rmdir $(SwigOutputCsharpDir) /s /q) 
	if not exist $(SwigOutputCsharpDir). (md $(SwigOutputCsharpDir))
	../../swig/swig.exe -csharp -c++ -D_WIN32 -DHAVE_STD_STRING -DHAVE_CXX_BOOL \
	-I../../OFFIS/dcmtk-3.5.3/config/include \
	-I../../OFFIS/dcmtk-3.5.3/ofstd/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmdata/include \
	-I../../OFFIS/dcmtk-3.5.3/dcmnet/include \
	-outdir $(SwigOutputCsharpDir) -Wall -makedefault -o $(SwigCppOutputFile) \
	-namespace $(SwigOutputNamespace) $(SwigInterfaceFile)

OffisDcm.dll : Offis_Wrapper.cxx 
	if exist $(CppCompilerOutputDirRoot). (rmdir $(CppCompilerOutputDirRoot) /s /q)
	if not exist $(CppCompilerOutputDirRoot). (md $(CppCompilerOutputDirRoot))
	if not exist $(CppCompilerOutputDir). (md $(CppCompilerOutputDir))
	cl.exe $(CppCompilerParameters) $(CppCompilerDebugParameters) $(CppCompilerIncludeParameters) \
	$(SwigCppOutputFile) /Fo$(CppCompilerOutputDir)\OffisDcm.obj \
	/Fd$(CppCompilerOutputDir)\OffisDcm.pdb /Fp$(CppCompierOutputDir)\OffisDcm.idb
	link.exe $(CppLinkerParameters) $(CppLinkerDebugParameters) \
	/OUT:$(CppCompilerOutputDir)\OffisDcm.dll $(CppLinkerObjectFiles) $(CppLinkerZlibFile) \
	$(CppCompilerOutputDir)\OffisDcm.obj

$(SwigOutputNamespace).dll : Offis_Wrapper.cxx
	if not exist $(SwigOutputCsharpDir)\$(Configuration) (md $(SwigOutputCsharpDir)\$(Configuration))
	csc.exe /out:$(SwigOutputCsharpDir)\$(Configuration)\$(SwigOutputNamespace).dll \
	/target:library /debug$(CscEmitDebugInformation) \
	$(SwigOutputCsharpDir)\\*.cs

all: $(SwigOutputNamespace).dll OffisDcm.dll
