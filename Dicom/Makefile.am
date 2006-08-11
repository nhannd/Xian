projectName=Dicom
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.Common.dll @BUILD_PATH@/ClearCanvas.Codecs.dll @BUILD_PATH@/ClearCanvas.Dicom.OffisWrapper.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST=\
	DataStore \
	Network	\
	Properties \
	Tests \
	$(ClearCanvas.Dicom_sources) \
	ClearCanvas.* \
	SR.resx

ClearCanvas.Dicomdir=@SOURCE_PATH@/$(projectName)
ClearCanvas.Dicom.Networkdir=@SOURCE_PATH@/$(projectName)/Network
ClearCanvas.Dicom.Propertiesdir=@SOURCE_PATH@/$(projectName)/Properties
ClearCanvas.Dicom_SCRIPTS=$(dllFilename)
ClearCanvas.Dicom_sources = *.cs
ClearCanvas.Dicom.Network_sources = *.cs
ClearCanvas.Dicom.Properties_sources = *.cs
ClearCanvas.Dicom_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.Dicom_sources))
ClearCanvas.Dicom_build_sources+=$(addprefix @srcdir@/Network/, $(ClearCanvas.Dicom.Network_sources))
ClearCanvas.Dicom_build_sources+=$(addprefix @srcdir@/Properties/, $(ClearCanvas.Dicom.Properties_sources))


CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:library\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-resource:$(SR_RESOURCES),$(projectNamespace).$(SR_RESOURCES)\
	-reference:$(REFERENCES)

$(dllFilename) : $(DEPENDENCIES)
	sed -i '/System.Diagnostics/d' $(SR_DESIGNER)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.Dicom_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PATH@
