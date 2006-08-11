projectName=ImageViewer
projectNamespace=ClearCanvas.$(projectName)
dllFilename=$(projectNamespace).dll

commaChar:= ,
empty:=
spaceChar:= $(empty) $(empty)
DEPENDENCIES=@BUILD_PATH@/ClearCanvas.Dicom.dll @BUILD_PATH@/ClearCanvas.Dicom.OffisWrapper.dll @BUILD_PATH@/ClearCanvas.Common.dll @BUILD_PATH@/ClearCanvas.Desktop.dll
REFERENCES=$(subst $(spaceChar),$(commaChar),$^)
MOSTLYCLEANFILES=@BUILD_PATH@/$(dllFilename) 
EXTRA_DIST=\
	Automation \
	Dashboard \
	StudyFinders \
	StudyLoaders \
	Tests \
	DynamicOverlays \
	Imaging \
	Layers \
	Properties \
	StudyManagement \
	View/WinForms \
	$(ClearCanvas.ImageViewer_sources)\
	ClearCanvas* \
	*.xml \
	*.cd \
	SR.resx	

ClearCanvas.ImageViewerdir=@SOURCE_PATH@/$(projectName)
ClearCanvas.ImageViewer.StudyManagementdir=@SOURCE_PATH@/$(projectName)/StudyManagement
ClearCanvas.ImageViewer.Imagingdir=@SOURCE_PATH@/$(projectName)/Imaging
ClearCanvas.ImageViewer.Layersdir=@SOURCE_PATH@/$(projectName)/Layers
ClearCanvas.ImageViewer.DynamicOverlaysdir=@SOURCE_PATH@/$(projectName)/DynamicOverlays
ClearCanvas.ImageViewer.Toolsdir=@SOURCE_PATH@/$(projectName)/Tools
ClearCanvas.ImageViewer.Propertiesdir=@SOURCE_PATH@/$(projectName)/Properties
ClearCanvas.ImageViewer_SCRIPTS=$(dllFilename)


ClearCanvas.ImageViewer_sources = *.cs
ClearCanvas.ImageViewer.StudyManagement_sources = *.cs
ClearCanvas.ImageViewer.Imaging_sources = *.cs
ClearCanvas.ImageViewer.Layers_sources = *.cs
ClearCanvas.ImageViewer.DynamicOverlays_sources = *.cs
ClearCanvas.ImageViewer.Tools_sources = *.cs
ClearCanvas.ImageViewer.Properties_sources = *.cs

ClearCanvas.ImageViewer_build_sources=$(addprefix @srcdir@/, $(ClearCanvas.ImageViewer_sources))
ClearCanvas.ImageViewer_build_sources+=$(addprefix @srcdir@/StudyManagement/, $(ClearCanvas.ImageViewer.StudyManagement_sources))
ClearCanvas.ImageViewer_build_sources+=$(addprefix @srcdir@/Imaging/, $(ClearCanvas.ImageViewer.Imaging_sources))
ClearCanvas.ImageViewer_build_sources+=$(addprefix @srcdir@/Layers/, $(ClearCanvas.ImageViewer.Layers_sources))
ClearCanvas.ImageViewer_build_sources+=$(addprefix @srcdir@/DynamicOverlays/, $(ClearCanvas.ImageViewer.DynamicOverlays_sources))
ClearCanvas.ImageViewer_build_sources+=$(addprefix @srcdir@/Tools/, $(ClearCanvas.ImageViewer.Tools_sources))
ClearCanvas.ImageViewer_build_sources+=$(addprefix @srcdir@/Properties/, $(ClearCanvas.ImageViewer.Properties_sources))

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
	$(GMCS) $(CSHARP_FLAGS) $(ClearCanvas.ImageViewer_build_sources) 

install-exec-local:
	cp @BUILD_PATH@/$(dllFilename) @INSTALL_PLUGIN_PATH@
	cp @SOURCE_PATH@/ImageViewer/actionmodels.xml @INSTALL_PATH@
