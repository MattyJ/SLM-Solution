using System.Web.Mvc;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IContextManager _contextManager;

        protected BaseController(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        protected JsonPayload GetJsonErrorResponse(string message)
        {
            _contextManager.ResponseManager.StatusCode = 500;
            _contextManager.ResponseManager.AppendHeader(ModelStateErrorNames.ErrorMessage, message);
            return new JsonPayload
            {
                Message = message,
                Success = false
            };
        }

        protected JsonPayload GetJsonSuccessResponse()
        {
            return new JsonPayload
            {
                Message = "Success",
                Success = true
            };
        }
    }
}