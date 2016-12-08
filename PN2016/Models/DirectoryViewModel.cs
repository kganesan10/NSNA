using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PN2016.Models
{
    public class ContactInfoViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "FirstName", GroupName="Primary", Order =1)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "LastName", GroupName = "Primary", Order = 2)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Gender", GroupName = "Primary", Order = 3)]
        public string Gender { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(128)]
        [Display(Name = "Email", GroupName = "Primary", Order = 4)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Home Phone Number", GroupName = "Primary", Order = 5)]
        public string HomePhone { get; set; }

        [Phone]
        [Display(Name = "Mobile Phone Number", GroupName = "Primary", Order = 6)]
        public string MobilePhone { get; set; }

        [MaxLength(128)]
        [Display(Name = "Address", GroupName = "Primary", Order = 7)]
        public string Address { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "City", GroupName = "Primary", Order = 8)]
        public string City { get; set; }

        [Required]
        [Display(Name = "State", GroupName = "Primary", Order = 9)]
        public string State { get; set; }

        [MaxLength(10)]
        [Display(Name = "ZipCode", GroupName = "Primary", Order = 9)]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Kovil", GroupName = "Primary", Order = 10)]
        public string Kovil { get; set; }

        [Required]
        [Display(Name = "KovilPirivu", GroupName = "Primary", Order = 11)]
        public string KovilPirivu { get; set; }

        [Required]
        [MaxLength(128)]
        [Display(Name = "Native Place", GroupName = "Primary", Order = 12)]
        public string NativePlace { get; set; }

        [Required]
        [Display(Name = "Marital status", GroupName = "Primary", Order = 13)]
        public string MaritalStatus { get; set; }

        
        [Display(Name = "Spouse - FirstName", GroupName = "Spouse", Order = 14)]
        public string SpouseFirstName { get; set; }
        
        [Display(Name = "Spouse - LastName", GroupName = "Spouse", Order = 15)]
        public string SpouseLastName { get; set; }

        [EmailAddress]
        [Display(Name = "Spouse - Email", GroupName = "Spouse", Order = 16)]
        public string SpouseEmail { get; set; }

        [Phone]
        [Display(Name = "Spouse - Mobile Phone Number", GroupName = "Spouse", Order = 17)]
        public string SpouseMobilePhone { get; set; }

        [Display(Name = "Spouse - Kovil", GroupName = "Spouse", Order = 18)]
        public string SpouseKovil { get; set; }

        [Display(Name = "Spouse - KovilPirivu", GroupName = "Spouse", Order = 19)]
        public string SpouseKovilPirivu { get; set; }

        [Display(Name = "Spouse - Native Place", GroupName = "Spouse", Order = 20)]
        public string SpouseNativePlace { get; set; }

        
        [Display(Name = "Kids - FirstName", GroupName = "Kids-1", Order = 21)]
        public string Kid1FirstName { get; set; }

        [Display(Name = "Kids - Age", GroupName = "Kids-1", Order = 22)]
        public string Kid1Age { get; set; }

        [Display(Name = "Kids - Gender", GroupName = "Kids-1", Order = 23)]
        public string Kid1Gender { get; set; }

        
        [Display(Name = "Kids - FirstName", GroupName = "Kids-2", Order = 24)]
        public string Kid2FirstName { get; set; }

        [Display(Name = "Kids - Age", GroupName = "Kids-2", Order = 25)]
        public string Kid2Age { get; set; }

        [Display(Name = "Kids - Gender", GroupName = "Kids-2", Order = 26)]
        public string Kid2Gender { get; set; }

        
        [Display(Name = "Kids - FirstName", GroupName = "Kids-3", Order = 27)]
        public string Kid3FirstName { get; set; }

        [Display(Name = "Kids - Age", GroupName = "Kids-3", Order = 28)]
        public string Kid3Age { get; set; }

        [Display(Name = "Kids - Gender", GroupName = "Kids-3", Order = 29)]
        public string Kid3Gender { get; set; }

        
        [Display(Name = "Kids - FirstName", GroupName = "Kids-4", Order = 30)]
        public string Kid4FirstName { get; set; }

        [Display(Name = "Kids - Age", GroupName = "Kids-4", Order = 31)]
        public string Kid4Age { get; set; }

        [Display(Name = "Kids - Gender", GroupName = "Kids-4", Order = 32)]
        public string Kid4Gender { get; set; }


        [Display(Name = "Kids - FirstName", GroupName = "Kids-5", Order = 33)]
        public string Kid5FirstName { get; set; }

        [Display(Name = "Kids - Age", GroupName = "Kids-5", Order = 34)]
        public string Kid5Age { get; set; }

        [Display(Name = "Kids - Gender", GroupName = "Kids-5", Order = 35)]
        public string Kid5Gender { get; set; }


        [Display(Name = "Family Picture", GroupName = "Family", Order = 36)]
        public HttpPostedFileBase FamilyPic { get; set; }

    }
}