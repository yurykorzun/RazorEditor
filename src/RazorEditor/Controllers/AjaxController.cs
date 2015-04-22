using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RazorEditor.Models;
using RazorEditor.Services;

namespace RazorEditor.Controllers
{
    public class AjaxController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetTemplateNames()
        {
            var templateNames = new TemplateService().GetTemplateNames();
            var idList        = Enumerable.Range(0, templateNames.Length);

            var result = templateNames.Zip(idList, (x, y) => new
            {
                text= x, id = y
            }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetModelNames()
        {
            var modelNames = new TemplateService().GetModelNames();
            var idList     = Enumerable.Range(0, modelNames.Length);

            var result = modelNames.Zip(idList, (x, y) => new
            {
                text = x,
                id = y
            }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetTemplate(string templateName, string modelName)
        {
            var template = new TemplateService().GetTemplate(templateName, modelName);

            return Request.CreateResponse(HttpStatusCode.OK, template);
        }

        [HttpPost]
        public HttpResponseMessage ConvertTemplate(UpdateTemplateModel template)
        {
            var result = new TemplateService().ConvertTemplate(template);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage SaveTemplate(UpdateTemplateModel template)
        {
            new TemplateService().SaveTemplate(template);

            return Request.CreateResponse(HttpStatusCode.OK, template);
        }



    }
}