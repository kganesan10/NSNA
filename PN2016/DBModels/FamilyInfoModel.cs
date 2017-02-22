using System;
using System.Collections.Generic;

namespace PN2016.DBModels
{
    public class FamilyInfoDBModel
    {
        public string FamilyContactGuid { get; set; }
        public int FamilyContactId { get; set; }

        //Personal Info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set;}

        //Contact Info
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }

        //Address Info
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        //Kovil+Native Info
        public string Kovil { get; set; }
        public string KovilPirivu { get; set; }
        public string NativePlace { get; set; }

        public SpouseInfoDBModel Spouse { get; set; }

        public List<KidsInfoDBModel> Kids { get; set; }

        public string FamilyPicFileName { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

    }

    public class SpouseInfoDBModel
    {

        //Personal Info
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //Contact Info
        public string Email { get; set; }
        public string MobilePhone { get; set; }

        //Kovil+Native Info
        public string Kovil { get; set; }
        public string KovilPirivu { get; set; }
        public string NativePlace { get; set; }

    }

    public class KidsInfoDBModel
    {
        public string FamilyContactGuid { get; set; }
        public string KidsInfoGuid { get; set; }

        public string FirstName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}