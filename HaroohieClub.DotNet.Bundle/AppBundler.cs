using System.IO;

namespace HaroohieClub.DotNet.Bundle
{
    public class AppBundler
    {
        private readonly BundleAppTask _task;
        private readonly StructureBuilder _builder;

        public AppBundler(BundleAppTask task, StructureBuilder builder)
        {
            _task = task;
            _builder = builder;
        }

        public void Bundle()
        {
            CopyIcon(
                new DirectoryInfo(_builder.OutputDirectory),
                new DirectoryInfo(_builder.ResourcesDirectory));
            
            CopyFiles(
                new DirectoryInfo(_builder.PublishDirectory),
                new DirectoryInfo(_builder.MacosDirectory),
                new DirectoryInfo(_builder.AppDirectory));
        }

        private void CopyFiles(DirectoryInfo source, DirectoryInfo target, DirectoryInfo exclude)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (var fileInfo in source.GetFiles())
            {
                var path = Path.Combine(target.FullName, fileInfo.Name);
                
                _task.Log.LogMessage($"Copying file from: {fileInfo.FullName}");
                _task.Log.LogMessage($"Copying to destination: {path}");
                
                fileInfo.CopyTo(path, true);
            }

            foreach (var sourceSubDir in source.GetDirectories())
            {
                if (sourceSubDir.FullName != exclude.FullName)
                {
                    var targetSubDir = target.CreateSubdirectory(sourceSubDir.Name);
                    CopyFiles(sourceSubDir, targetSubDir, exclude);
                }
            }
        }

        private void CopyIcon(DirectoryInfo source, DirectoryInfo destination)
        {
            var iconName = _task.CFBundleIconFile;
            if (string.IsNullOrWhiteSpace(iconName))
            {
                _task.Log.LogMessage($"No icon is specified for bundle");
                return;
            }
            
            var sourcePath = Path.Combine(source.FullName, iconName);
            _task.Log.LogMessage($"Icon file source for bundle is: {sourcePath}");
            
            var targetPath = Path.Combine(destination.FullName, Path.GetFileName(iconName));
            _task.Log.LogMessage($"Icon file destination for bundle is: {targetPath}");
            
            var sourceFile = new FileInfo(sourcePath);
            
            if (sourceFile.Exists)
            {
                _task.Log.LogMessage($"Copying icon file to destination: {targetPath}");
                sourceFile.CopyTo(targetPath);
            }
        }
    }
}