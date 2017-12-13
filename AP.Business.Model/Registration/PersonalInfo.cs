namespace AP.Business.Model.Registration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PersonalInfo : IEquatable<PersonalInfo>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [RegularExpression(@"[\w-]+(\.?[\w-])*\@[\w-]+(\.[\w-]+)+", ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        #region Equality

        public static bool operator ==(PersonalInfo obj1, PersonalInfo obj2)
        {
            return PersonalInfo.Equals(obj1, obj2);
        }

        public static bool operator !=(PersonalInfo obj1, PersonalInfo obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(PersonalInfo other)
        {
            return PersonalInfo.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return PersonalInfo.Equals(this, obj as PersonalInfo);
        }

        public static bool Equals(PersonalInfo obj1, PersonalInfo obj2)
        {
            if (Object.Equals(obj1, null) && Object.Equals(obj2, null)) return true;
            if (Object.ReferenceEquals(obj1, obj2)) return true;

            if (Object.Equals(null, obj1) ||
                Object.Equals(null, obj2) ||
                obj1.GetType() != obj2.GetType())
                return false;

            // Compare your object properties
            return string.Equals(obj1.Email, obj2.Email, StringComparison.CurrentCultureIgnoreCase) &&
                obj1.FirstName == obj2.FirstName &&
                obj1.LastName == obj2.LastName;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Email != null)
                hash ^= Email.GetHashCode();
            if (FirstName != null)
                hash ^= FirstName.GetHashCode();
            if (LastName != null)
                hash ^= LastName.GetHashCode();

            return hash;
        }

        #endregion
    }
}
