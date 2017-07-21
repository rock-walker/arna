using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using AP.Infrastructure.CrossConcern;
using AP.Infrastructure.WebApiCore;

namespace AP.Application.Infrastructure
{
	public class ApiExceptionHandlerFilter : ExceptionFilterAttribute
	{
		readonly ApiExceptionConvertor _exceptionConvertor;
		readonly ILogger _log;

		public ApiExceptionHandlerFilter(ApiExceptionConvertor exceptionConvertor, ILogger log)
		{
			_exceptionConvertor = exceptionConvertor; //new ApiExceptionConvertor(new ExceptionSettings());//
			_log = log;
		}

		public override void OnException(HttpActionExecutedContext filterContext)
		{
			if (filterContext.Response != null)
				return;

			HttpStatusCode httpCode;
			var error = _exceptionConvertor.ConvertException(filterContext.Exception, out httpCode);

			ApiExceptionConvertor.LogException(_log, filterContext.Exception, error, httpCode);

			filterContext.Response = filterContext.Request.CreateResponse(httpCode, error);
			filterContext.Response.Headers.Add("el-api-fault-corrid", error.CorrelationId);
		}
	}
}
