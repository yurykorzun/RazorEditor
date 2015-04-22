using System.Collections.Generic;
using System.Web.Mvc;

namespace RazorEditor.Models
{
    public class TemplateModel
    {
        public TemplateModel(string originalSource)
        {
            OriginalSource = originalSource;
            Tokens         = new List<TemplateTokenModel>();
            Properties     = new List<TemplatePropertyModel>();
        }

        [AllowHtml]
        public string OriginalSource { get; set; }
        [AllowHtml]
        public string ProcessedSource { get; set; }
        public string ModelType { get; set; }
        public List<TemplateTokenModel> Tokens { get; set; }
        public List<TemplatePropertyModel> Properties { get; set; } 
    }
}