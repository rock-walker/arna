using System;
using System.Collections.Concurrent;
using System.Security.Principal;

namespace AP.Core.User.Principal
{
	public interface ISessionPrincipal : IPrincipal
	{
		Session.LoginKey LoginKey { get; }
		Session.AccountData AccountData { get; }

		int UserId { get; }
		Guid UserGuid { get; }
		string Sid { get; }
		string SessionToken { get; }

		UserData UserData { get; }

		ConcurrentDictionary<string, object> CustomData { get; }

		int Language { get; }
	}
}