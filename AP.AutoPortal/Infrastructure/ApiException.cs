using System;
using System.Net;
using System.Text;
using AP.DataContract.Enums;

namespace AP.Application.Infrastructure
{
	public class ApiException: Exception
	{
		public ErrorCodes FaultCode;
		public HttpStatusCode StatusCode;
		public string DeveloperMessage;
		public string CorrelationId;
		public object Details;

		public static string BuildDevMsg(Exception ex, string devMsg = null)
		{
			if (ex == null)
				return devMsg;

			var sb = new StringBuilder(devMsg);

			if (sb.Length > 0)
				sb.AppendLine();

			bool showNext = false;
			while (ex != null)
			{
				if(showNext)
					sb.AppendLine("------------------------------------ NEXT EXCEPTION ----------------------------");
				else
					sb.AppendLine("-------------------------------------- EXCEPTION -------------------------------");

				sb.AppendFormat("Exception: {0}, Message: {1}\n", ex.GetType().Name, ex.Message);
				sb.AppendLine(ex.ToString());

				ex = ex.InnerException;

				showNext = true;
			}

			return sb.ToString();
		}

		public ApiException(ErrorCodes code, string message, string devMessage = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, Exception innerException = null, string correlationId = null, object details = null)
			: base(string.Format((string) "{0}. {1}", (object) code, message), innerException)
		{
			FaultCode = code;
			StatusCode = statusCode;
			CorrelationId = correlationId ?? Guid.NewGuid().ToString("N");
			Details = details;

			DeveloperMessage = BuildDevMsg(InnerException, devMessage);
		}
	}
}
