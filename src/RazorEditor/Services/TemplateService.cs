using System.Drawing;
using System.Reflection;
using RazorEditor.Data;
using RazorEditor.Models;
using RazorEditor.Models.Datasource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

namespace RazorEditor.Services
{
    public class TemplateService
    {
        private readonly ImageService ImageService = new ImageService();
        private readonly FileService FileService   = new FileService();

        public TemplateModel GetTemplate(string templateName, string modelName)
        {
            var baseType  = typeof (BaseDatasourceModel);
            var modelType = baseType.Assembly.GetType(string.Format("{0}.{1}", baseType.Namespace, modelName));
            if (modelType == null)
            {
                throw new ArgumentException(string.Format("Model type {0} is not available", modelName));
            }

            var templateSource = FileService.ReadFileContents(templateName);

            var parsedSymbols = ParseSymbols(templateSource);
            var templateData  = GenerateTemplateModel(parsedSymbols, templateSource, modelType);

            return templateData;
        }

        public TemplateModel ConvertTemplate(UpdateTemplateModel template)
        {
            var baseType = typeof(BaseDatasourceModel);
            var modelType = baseType.Assembly.GetType(string.Format("{0}.{1}", baseType.Namespace, template.ModelType));
            if (modelType == null)
            {
                throw new ArgumentException(string.Format("Model type {0} is not available", template.ModelType));
            }

            var parsedSymbols = ParseSymbols(template.TemplateSource);
            var templateData  = GenerateTemplateModel(parsedSymbols, template.TemplateSource, modelType);

            return templateData;
        }


        public void SaveTemplate(UpdateTemplateModel template)
        {
            FileService.SaveTemplate(template.TemplateName, template.TemplateSource);
        }

        public string[] GetTemplateNames()
        {
            return FileService.GetFileNames();
        }

        public string[] GetModelNames()
        {
            var baseType = typeof(BaseDatasourceModel);
            var modelTypes = baseType.Assembly
                                            .GetTypes()
                                            .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract)
                                            .Select( type => type.Name)
                                            .ToArray();

            return modelTypes;
        }

        #region Private

        private List<Span> ParseSymbols(string templateSource)
        {
            var codeParser = new MvcCSharpRazorCodeParser();
            var markupParser = new HtmlMarkupParser();

            var context = new ParserContext(new SeekableTextReader(templateSource), codeParser, markupParser,
                markupParser);
            codeParser.Context = context;
            markupParser.Context = context;
            markupParser.ParseDocument();

            var results = context.CompleteParse();

            var resultList = results.Document.Flatten().Where(x => x.Kind == SpanKind.Code).ToList();

            return resultList;
        }

        private TemplateModel GenerateTemplateModel(List<Span> syntaxSymbolsList, string originalSource, Type modelType)
        {
            var propertyNames =
                modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(prop => prop.PropertyType == typeof (string)
                                   || prop.PropertyType == typeof (DateTime)
                                   || prop.PropertyType == typeof (int))
                    .OrderBy(x => x.Name)
                    .Select(x => x.Name)
                    .ToList();

            var templateData = new TemplateModel(originalSource);
            var proccessedString = new StringBuilder(originalSource).ToString();
            var imagePropertyCache = new Dictionary<string, string>();

            templateData.ModelType = modelType.Name;

            foreach (var propertyName in propertyNames)
            {
                var property = new TemplatePropertyModel
                {
                    DisplayText = propertyName,
                    EncodedImage = ImageService.GetEncodedImage(propertyName, Color.Black, FontStyle.Regular),
                    ReplacedText = string.Format("@Model.{0}", propertyName)
                };

                templateData.Properties.Add(property);
                imagePropertyCache.Add(propertyName, property.EncodedImage);
            }

            for (int resultIndex = 0; resultIndex < syntaxSymbolsList.Count; resultIndex++)
            {
                var currentResult = syntaxSymbolsList[resultIndex];
                var symbols = currentResult.Symbols.ToList();
                var tokenId = string.Format("##{0}##", resultIndex);

                //Move back a symbol to capture the @ symbol, move forward to capture the whole string
                var foundSnipped = originalSource.Substring(currentResult.Start.AbsoluteIndex - 1,
                    currentResult.Length + 1);
                var tokenTextColor = Color.Black;
                var tokenTextStyle = FontStyle.Regular;

                if (!string.IsNullOrWhiteSpace(foundSnipped) && proccessedString.Contains(foundSnipped))
                {
                    proccessedString = proccessedString.Replace(foundSnipped, tokenId);

                    var token = new TemplateTokenModel();

                    var symbolsEnumerator = symbols.GetEnumerator();
                    if (symbols.Count() > 1 && symbolsEnumerator.MoveNext() &&
                        symbolsEnumerator.Current.Content == "Model")
                    {
                        string propertyName = null;
                        if (symbolsEnumerator.MoveNext() && symbolsEnumerator.Current.Content == ".")
                        {
                            propertyName = symbols.Last().Content;
                        }

                        if (!propertyNames.Contains(propertyName))
                        {
                            token.DisplayText = "Missing property";
                            tokenTextColor = Color.Red;
                            tokenTextStyle = FontStyle.Bold;
                            ;
                        }
                        else
                        {
                            token.DisplayText = propertyName;
                        }
                    }
                    else
                    {
                        token.DisplayText = "Advanced code";
                        tokenTextStyle = FontStyle.Bold;
                        ;
                    }

                    if (!imagePropertyCache.ContainsKey(token.DisplayText))
                    {
                        token.EncodedImage = ImageService.GetEncodedImage(token.DisplayText, tokenTextColor,
                            tokenTextStyle);
                        imagePropertyCache.Add(token.DisplayText, token.EncodedImage);
                    }
                    else
                    {
                        token.EncodedImage = imagePropertyCache[token.DisplayText];
                    }

                    token.ReplacedText = foundSnipped;
                    token.TokenId = tokenId;

                    templateData.Tokens.Add(token);
                }

            }
            templateData.ProcessedSource = proccessedString;

            return templateData;

        }

        #endregion

    }
}