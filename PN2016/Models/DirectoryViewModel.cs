using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PN2016.Models
{
    public class ContactInfoViewModel
    {
        public ContactInfoViewModel()
        {
            Kids = new List<KidsViewModel>();
        }

        [Required]
        [MaxLength(50)]
        [Display(Name = "FirstName", GroupName="Primary")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "LastName", GroupName = "Primary")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Gender", GroupName = "Primary")]
        public string Gender { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(128)]
        [Display(Name = "Email", GroupName = "Primary")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Home Phone", GroupName = "Primary")]
        public string HomePhone { get; set; }

        [Phone]
        [Display(Name = "Mobile Phone", GroupName = "Primary")]
        public string MobilePhone { get; set; }

        [MaxLength(128)]
        [Display(Name = "Address", GroupName = "Primary")]
        public string Address { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "City", GroupName = "Primary")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State", GroupName = "Primary")]
        public string State { get; set; }

        [MaxLength(10)]
        [Display(Name = "ZipCode", GroupName = "Primary")]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Kovil", GroupName = "Primary")]
        public string Kovil { get; set; }

        [Required]
        [Display(Name = "Kovil Pirivu", GroupName = "Primary")]
        public string KovilPirivu { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "Native Place", GroupName = "Primary")]
        public string NativePlace { get; set; }

        [Required]
        [Display(Name = "Marital status", GroupName = "Primary")]
        public string MaritalStatus { get; set; }

        
        [Display(Name = "FirstName", GroupName = "Spouse")]
        public string SpouseFirstName { get; set; }
        
        [Display(Name = "LastName", GroupName = "Spouse")]
        public string SpouseLastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email", GroupName = "Spouse")]
        public string SpouseEmail { get; set; }

        [Phone]
        [Display(Name = "Mobile Phone", GroupName = "Spouse")]
        public string SpouseMobilePhone { get; set; }

        [Display(Name = "Kovil", GroupName = "Spouse")]
        public string SpouseKovil { get; set; }

        [Display(Name = "Kovil Pirivu", GroupName = "Spouse")]
        public string SpouseKovilPirivu { get; set; }

        [Display(Name = "Native Place", GroupName = "Spouse")]
        public string SpouseNativePlace { get; set; }

        public List<KidsViewModel> Kids { get; set; }
        
        [Display(Name = "Family Picture", GroupName = "Family")]
        public HttpPostedFileBase FamilyPic { get; set; }

        public string FamilyPicFileExtn { get; set; }

        public string FamilyPicFilePath { get; set; }

        public string FamilyContactGuid { get; set; }
    }

    public class KidsViewModel
    {
        [Display(Name = "FirstName", GroupName = "Kids")]
        public string FirstName { get; set; }

        [Display(Name = "Age", GroupName = "Kids")]
        public int? Age { get; set; }

        [Display(Name = "Gender", GroupName = "Kids")]
        public string Gender { get; set; }
    }

    public class ContactListViewModel
    {
        public string FamilyContactGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Kovil { get; set; }
        public string KovilPirivu { get; set; }
        public string NativePlace { get; set; }
        public string MaritalStatus { get; set; }
    }
}