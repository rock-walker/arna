using System;
using System.Collections.Generic;

namespace AP.Core.User
{
    public class UserData
    {
        /// <summary>
        /// User identifier of local server
        /// according to Domain/UserName
        /// </summary>
        public Int32 UserId;

        /// <summary>
        /// User domain
        /// </summary>
        public string Domain;

        /// <summary>
        /// User login in Domain
        /// </summary>
        public string UserName;

        /// <summary>
        /// Server identifier on local server
        /// </summary>
        public Int32 ServerId;

        /// <summary>
        /// Return in what context handler is called
        /// </summary>
        //public Destination Destination;

        /// <summary>
        /// Prefix to main handler
        /// </summary>
        //[XmlElement(Names.Query.Prefix)]
        public String Prefix;

        /// <summary>
        /// ProductId from UserInfo
        /// </summary>
        public int ProductId;

        /// <summary>
        /// User security identifier
        /// </summary>
        //[XmlElement(Names.Query.Prefix)]
        public String UserSecId;

        /// <summary>
        /// Connected user version
        /// </summary>
        public Int32 RemoteVersion;

        /// <summary>
        /// User license languages
        /// CrossLanguageDescriptor.Language contains list of licensed languages
        /// </summary>
        private List<CrossLanguageDescriptor> _licenseLanguages;

        //[XmlElement(Names.Query.LicenseLanguages)]
        public List<CrossLanguageDescriptor> LicenseLanguages
        {
            get { return _licenseLanguages; }
            set { _licenseLanguages = value; }
        }

        public bool ContainsLicenseLanguage(int language)
        {
            if (_licenseLanguages == null)
                return false;

            foreach (CrossLanguageDescriptor _descr in _licenseLanguages)
                if (_descr.Language == language)
                    return true;

            return false;

        }
    }
	public class CrossLanguageDescriptor
	{
		/// <summary>
		/// Initial language
		/// </summary>
		//[XmlElement(Names.Query.Language)]
		public int Language;

		/// <summary>
		/// List of cross languages for initial language
		/// </summary>
		//[XmlElement(Names.Query.CrossLanguages)]
		public List<int> CrossLanguages;
	}
}
