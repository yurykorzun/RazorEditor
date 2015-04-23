using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using RazorEditor.Services;
using SimpleInjector;

namespace RazorEditor
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var configuration = GlobalConfiguration.Configuration;
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);

            var container = new Container();
            container.Register<IFileService, FileService>();
            container.Register<IImageService, ImageService>();
            container.Register<ITemplateService, TemplateService>();


            container.Verify();
            //DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}