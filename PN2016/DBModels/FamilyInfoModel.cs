using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PN2016.DBModels
{
    public class FamilyContactModel
    {
        public string FamilyContactGuid { get; set; }

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

        public SpouseInfoModel Spouse { get; set; }

        public List<KidsInfoModel> Kids { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

    }

    public class SpouseInfoModel
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

    public class KidsInfoModel
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