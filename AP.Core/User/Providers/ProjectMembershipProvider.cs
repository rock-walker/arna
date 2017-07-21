using System.Text.RegularExpressions;
using WebMatrix.WebData;

namespace AP.Core.User.Providers
{
	public class ProjectMembershipProvider : SimpleMembershipProvider
	{
		public override bool ValidateUser(string login, string password)
		{
			// check to see if the login passed is an email address
			if (IsValidEmail(login))
			{
				string actualUsername = base.GetUserNameByEmail(login);
				return base.ValidateUser(actualUsername, password);
			}
			return base.ValidateUser(login, password);
		}

		static bool IsValidEmail(string strIn)
		{
			// Return true if strIn is in valid e-mail format.
			return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}
	}
}