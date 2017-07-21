namespace AP.Core.User
{
	public class Session
	{
		public class AccountData
		{
			public string NetLogin { get; private set; }
			public string FullName { get; private set; }
			public string Details { get; private set; }
			public string Email { get; private set; }
			public string Domain { get; private set; }
			public string Name { get; private set; }

			// GFIN8 #35637 This property is not used according to Pavel Baravik
			//public int? Language { get; private set; }

			public AccountData(string domain, string name, string netLogin, string fullName, string details, string email)
			{
				Domain = domain;
				Name = name;
				NetLogin = netLogin;
				FullName = fullName;
				Details = details;
				Email = email;
			}
		}

		public class LoginKey
		{
			public string Computer { get; private set; }
			public string Ethernet { get; private set; }
			public string Disk { get; private set; }
			public string IP { get; private set; }
			public string IseSessionId { get; set; }
			public string[] IsePcns { get; set; }
			public string IseProductId { get; set; }

			public LoginKey(string ip = null, string computer = null, string ethernet = null, string disk = null, string iseSessionId = null, string[] isePcns = null, string iseProductId = null)
			{
				Computer = computer ?? "";
				IP = ip ?? "";
				Ethernet = ethernet ?? "";
				Disk = disk ?? "";
				IseSessionId = iseSessionId ?? "";
				IseProductId = iseProductId ?? "";
			}
		}
	}
}
