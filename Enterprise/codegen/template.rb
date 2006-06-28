require "metamodel"
require "fileutils"
require "erb"

# Processes the specified template for every class in the specified model
class Template
  # templateFile - the name of the template file to process
  # outputFile - the name of the file to output, which must contain exactly one * which will be substituted by the class name
  def initialize(codeTemplateFile, outFilenameTemplate, overwrite)
    @codeTemplateFile = codeTemplateFile
    @outFilenameTemplate = outFilenameTemplate
    @overwrite = overwrite
  end
  
  # runs this template on the specified ClassDef
  def run(elementDef, templateDir, destDir)
    #use the namespace of the model to extend the destDir
    #e.g. if destDir = . and the namespace is Foo.Bar, destDir becomes ./Foo/Bar
    destDir = File.expand_path(elementDef.namespace.gsub('.', "/"), destDir)
    
    # create the erb once only, since it can be re-used
    @outFilenameERB = ERB.new(@outFilenameTemplate) if @outFilenameERB == nil
    
    # generate the output path, including the output file name
    outputPath = File.expand_path(@outFilenameERB.result(elementDef.get_binding), destDir)

    # abort if the file exists already and should not be overwritten
    return if(!@overwrite && File.exist?(outputPath))
    
    # create the erb once only, since it can be re-used
    @codeERB = ERB.new(IO.read(File.expand_path(@codeTemplateFile, templateDir)), nil, "") if @codeERB == nil
  
    #create output folder if doesn't exist
    FileUtils.mkdir_p(File.split(outputPath)[0])

    #write the ERB output to a file
    File.open(outputPath, "w") do |outStream|
      outStream.print(@codeERB.result(elementDef.get_binding))
    end
    outputPath  #return the name of the generated file
  end
end
