using System;
using System.IO;
using System.Linq;
using System.Web;

namespace RazorEditor.Data
{
    public class FileService
    {
        private const string PathToTemplates = @"\TestTemplates\";

        public string[] GetFileNames()
        {
            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(PathToTemplates), "*.cshtml", SearchOption.TopDirectoryOnly)
                                    .Select(Path.GetFileName)
                                    .ToArray(); ;
            return files;
        }

        public string ReadFileContents(string templateName)
        {
            var pathToTemplate = HttpContext.Current.Server.MapPath(Path.Combine(PathToTemplates, templateName));

            if (!File.Exists(pathToTemplate))
            {
                throw new ArgumentException(string.Format("File {0} is not available", templateName));
            }

            using (var file = new StreamReader(pathToTemplate))
            {
                var templateSource = file.ReadToEnd();

                return templateSource;
            }   
        }

        public void SaveTemplate(string templateName, string templateSource)
        {
            var pathToTemplate = HttpContext.Current.Server.MapPath(Path.Combine(PathToTemplates, templateName));

            using (var file = new StreamWriter(pathToTemplate))
            {
                file.Write(templateSource);
            }
        }
    }
}