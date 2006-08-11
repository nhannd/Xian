projectName=ImageViewer.Renderer.GDI
projectDir=ImageViewer/Renderer/GDI
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.ImageViewer.dll @BUILD_PATH@/ClearCanvas.Common.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST= \
	Tests \
	$(ClearCanvas.ImageViewer.Renderer.GDI_sources) \
	ClearCanvas* \
	SR.resx

ClearCanvas.ImageViewer.Renderer.GDIdir=@SOURCE_PATH@/$(projectDir)
ClearCanvas.ImageViewer.Renderer.GDI_SCRIPTS=$(dllFilename)
ClearCanvas.ImageViewer.Renderer.GDI_sources = *.cs
ClearCanvas.ImageViewer.Renderer.GDI_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.ImageViewer.Renderer.GDI_sources))

CSHARP_FLAGS =\
	-d:MONO,LINUX\
	-out:@BUILD_PATH@/$@\
	-target:library\
	-debug$(GMCS_DEBUG_FLAG)\
	-lib:@BUILD_PATH@\
	-pkg:dotnet\
	-resource:$(SR_RESOURCES),$(projectNamespace).$(SR_RESOURCES)\
	-reference:$(REFERENCES)\
	-unsafe

$(dllFilename) : $(DEPENDENCIES)
	sed -i '/System.Diagnostics/d' $(SR_DESIGNER)
	resgen $(SR_RESX)
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.ImageViewer.Renderer.GDI_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
