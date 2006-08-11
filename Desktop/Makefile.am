projectName=Desktop
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.Dicom.dll @BUILD_PATH@/ClearCanvas.Dicom.OffisWrapper.dll @BUILD_PATH@/ClearCanvas.Common.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST=\
	Actions \
	Dashboard \
	ExtensionBrowser \
	Help \
	Properties \
	Tests \
	View/WinForms \
	Tools \
	$(ClearCanvas.Desktop_sources) \
	ClearCanvas.* \
	SR.resx

ClearCanvas.Desktopdir=@SOURCE_PATH@/$(projectName)
ClearCanvas.Desktop.Toolsdir=@SOURCE_PATH@/$(projectName)/Tools
ClearCanvas.Desktop.Actionsdir=@SOURCE_PATH@/$(projectName)/Actions
ClearCanvas.Desktop.Propertiesdir=@SOURCE_PATH@/$(projectName)/Properties
ClearCanvas.Desktop_SCRIPTS=$(dllFilename)

ClearCanvas.Desktop_sources = *.cs
ClearCanvas.Desktop.Tools_sources = *.cs
ClearCanvas.Desktop.Actions_sources = *.cs
ClearCanvas.Desktop.Properties_sources = *.cs

ClearCanvas.Desktop_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.Desktop_sources))
ClearCanvas.Desktop_build_sources+=$(addprefix @srcdir@/Tools/, $(ClearCanvas.Desktop.Tools_sources))
ClearCanvas.Desktop_build_sources+=$(addprefix @srcdir@/Actions/, $(ClearCanvas.Desktop.Actions_sources))
ClearCanvas.Desktop_build_sources+=$(addprefix @srcdir@/Properties/, $(ClearCanvas.Desktop.Properties_sources))

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
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.Desktop_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
