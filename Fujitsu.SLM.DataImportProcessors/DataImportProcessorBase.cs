using System;
using System.IO;
using System.Linq;

namespace Fujitsu.SLM.DataImportProcessors
{
    public class DataImportProcessorBase
    {
        // Checks a file exists
        protected bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        // Checks that a list of files exists
        protected bool FilesExist(params string[] fileNames)
        {
            return fileNames.All(File.Exists);
        }

        // Checks whether the file (name) has changed since the last recorded date time.
        protected bool FileHasChanged(DateTime lastModifiedDateTime, string fileName)
        {
            if (FileExists(fileName))
            {
                var fileLastUpdatedDateTime = File.GetLastWriteTime(fileName);
                return fileLastUpdatedDateTime.Subtract(lastModifiedDateTime).TotalSeconds >= 1;
            }

            return false;
        }

        protected DateTime LastUpdatedDateTime(string fileName)
        {
            return File.GetLastWriteTime(fileName);
        }
    }
}
