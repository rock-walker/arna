using System.Runtime.Serialization;
using AP.DataContract.Enums;

namespace AP.DataContract
{
	/// <summary>
	/// Describes error 
	/// </summary>
	[DataContract]
	public class ErrorDataContract
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="error"></param>
		public ErrorDataContract(ErrorCodes error)
		{
			FaultCode = (int)error;
			FaultName = ((ErrorCodes)FaultCode).ToString();
		}

		/// <summary>
		/// Code of error
		/// </summary>
		[DataMember(Name = "errorCode", Order = 0)]
		public int FaultCode;

		/// <summary>
		/// Name of error
		/// </summary>
		[DataMember(Name = "errorName", Order = 10)]
		public string FaultName;

		/// <summary>
		/// Message
		/// </summary>
		[DataMember(Name = "userMessage", EmitDefaultValue = false, Order = 20)]
		public string Message;

		/// <summary>
		/// String that can be used to find the error in server's log files 
		/// </summary>
		[DataMember(Name = "correlationId", EmitDefaultValue = false, Order = 30)]
		public string CorrelationId;

		/// <summary>
		/// Additional information.
		/// </summary>
		[DataMember(Name = "moreInfo", EmitDefaultValue = false, Order = 40)]
		public string MoreInfo;

		/// <summary>
		/// Techincal message
		/// </summary>
		[DataMember(Name = "developerMessage", EmitDefaultValue = false, Order = 10000, IsRequired = false)]
		public string DeveloperMessage;

		/// <summary>
		/// Specific details for particular error code
		/// </summary>
		[DataMember(Name = "details", EmitDefaultValue = false, Order = 10001, IsRequired = false)]
		public object Details;
	}
}
