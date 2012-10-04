require 'find'

dirs = ["."]
excludes = [".svn"]
for dir in dirs
  Find.find(dir) do |path|
    if FileTest.directory?(path)
      if excludes.include?(File.basename(path))
        Find.prune       # Don't look any further into this directory.
      end
    else
		if(/.\.csproj$/ =~ path || /.\.proj$/ =~ path)
			x = ''
			File.open(path, "r") { |f| x = f.read }
			y = x.gsub('NHibernate2.1', 'NHibernate2.2')
			if(y != x)
				p path
				File.open(path, "w") { |f| f.write(y) }
			end
		end
    end
  end
end
