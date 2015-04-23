using System.Web.Mvc;

namespace RazorEditor.Attributes
{
    /// <summary>
    /// This attribute will limit the access to an action to be only Ajax
    /// usage:  [AjaxOnly]
    /// </summary>
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}
