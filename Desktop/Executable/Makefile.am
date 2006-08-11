projectName=Desktop.Executable
projectDir=Desktop/Executable
projectNamespace=ClearCanvas.$(projectName)
targetFilename=$(projectNamespace).exe

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.Common.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
MOSTLYCLEANFILES=@BUILD_PATH@/$(targetFilename) 
EXTRA_DIST=\
	Tests \
	$(ClearCanvas.Desktop.Executable_sources) \
	App.* \
	ClearCanvas* \
	Logging.config \
	postbuild.sh \
	SR.resx		

ClearCanvas.Desktop.Executabledir=@SOURCE_PATH@/$(projectDir)
ClearCanvas.Desktop.Executable_SCRIPTS=$(targetFilename)
ClearCanvas.Desktop.Executable_sources = *.cs
ClearCanvas.Desktop.Executable_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.Desktop.Executable_sources))

CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:exe\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-resource:$(SR_RESOURCES),$(projectNamespace).$(SR_RESOURCES)\
	-reference:$(REFERENCES)

$(targetFilename) : $(DEPENDENCIES)
	sed -i '/System.Diagnostics/d' $(SR_DESIGNER)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.Desktop.Executable_build_sources)

install-exec-local:
	cp @BUILD_PATH@/$(targetFilename) @INSTALL_PATH@
