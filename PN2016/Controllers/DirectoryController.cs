using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PN2016.DBModels;
using PN2016.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PN2016.Controllers
{
    public class DirectoryController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Create");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ContactInfoViewModel model)
        {
            ValidateModel(model);
            
            var hpf = model.FamilyPic;
            /*
            if (hpf != null && hpf.ContentLength != 0)
            {
                var fileExtension = Path.GetExtension(hpf.FileName).ToLower();
                if (fileExtension != ".jpg" || fileExtension != ".png" || fileExtension != ".gif" || fileExtension != ".jpeg")
                {
                    ModelState.AddModelError("Family Picture", "Only Images are allowed for Family Picture.");
                }
            }
            */
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Process Data to Create Data Model
            FamilyContactModel familyContact = ProcessDBModel(model);

            //Insert into DB
            ContactInfoDB contactInfoDB = new ContactInfoDB();
            contactInfoDB.InsertFamilyInfo(familyContact);

            //Upload Pic to Azure.
            if (hpf != null && hpf.ContentLength != 0)
            {
                var FileExtension = Path.GetExtension(hpf.FileName);
                var mediaFileName = familyContact.FamilyContactGuid + FileExtension;
                new AzureFileStorage().UploadFile(mediaFileName, hpf.InputStream);
            }
            
            //Send Email.
            GMailDispatcher mailDispatcher = new GMailDispatcher();
            mailDispatcher.SendPN2016Message(familyContact.Email, familyContact.FirstName+ " "+ familyContact.LastName);
            
            return View("CreateConfirm");
        }

        private void ValidateModel(ContactInfoViewModel model)
        {
            var mStatus = model.MaritalStatus ?? string.Empty;
            if (mStatus == "M")
            {
                // Check Spouse Info
                if (string.IsNullOrWhiteSpace(model.SpouseFirstName))
                    ModelState.AddModelError("Spouse First Name", "Spouse's First Name is required");
                if (string.IsNullOrWhiteSpace(model.SpouseLastName))
                    ModelState.AddModelError("Spouse Last Name", "Spouse's Last Name is required");
                if (string.IsNullOrWhiteSpace(model.SpouseKovil))
                    ModelState.AddModelError("Spouse Kovil", "Spouse's Kovil at Birth is required");
                if (string.IsNullOrWhiteSpace(model.SpouseKovilPirivu))
                    ModelState.AddModelError("Spouse Kovil Pirivu", "Spouse's Kovil Pirivu at Birth is required");
            }
        }

        private static FamilyContactModel ProcessDBModel(ContactInfoViewModel model)
        {
            var familyContact = new FamilyContactModel();
            familyContact.FamilyContactGuid = Guid.NewGuid().ToString("N");
            familyContact.FirstName = string.IsNullOrWhiteSpace(model.FirstName) ? string.Empty : model.FirstName;
            familyContact.LastName = string.IsNullOrWhiteSpace(model.LastName) ? string.Empty : model.LastName;
            familyContact.Gender = string.IsNullOrWhiteSpace(model.Gender) ? string.Empty : model.Gender;
            familyContact.MaritalStatus = string.IsNullOrWhiteSpace(model.MaritalStatus) ? string.Empty : model.MaritalStatus;

            familyContact.Email = string.IsNullOrWhiteSpace(model.Email) ? string.Empty : model.Email;
            familyContact.HomePhone = string.IsNullOrWhiteSpace(model.HomePhone) ? string.Empty : model.HomePhone;
            familyContact.MobilePhone = string.IsNullOrWhiteSpace(model.MobilePhone) ? null : model.MobilePhone; //Nullable

            familyContact.Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address; //Nullable
            familyContact.City = string.IsNullOrWhiteSpace(model.City) ? string.Empty : model.City;
            familyContact.State = string.IsNullOrWhiteSpace(model.State) ? string.Empty : model.State;
            familyContact.ZipCode = string.IsNullOrWhiteSpace(model.ZipCode) ? null : model.ZipCode; //Nullable

            familyContact.Kovil = string.IsNullOrWhiteSpace(model.Kovil) ? string.Empty : model.Kovil;
            familyContact.KovilPirivu = string.IsNullOrWhiteSpace(model.KovilPirivu) ? string.Empty : model.KovilPirivu;
            familyContact.NativePlace = string.IsNullOrWhiteSpace(model.NativePlace) ? string.Empty : model.NativePlace;

            //Attach Spouse
            if (familyContact.MaritalStatus == "M")
            {
                var spouseInfo = new SpouseInfoModel();
                spouseInfo.FirstName = string.IsNullOrWhiteSpace(model.SpouseFirstName) ? string.Empty : model.SpouseFirstName;
                spouseInfo.LastName = string.IsNullOrWhiteSpace(model.SpouseLastName) ? string.Empty : model.SpouseLastName;

                spouseInfo.Email = string.IsNullOrWhiteSpace(model.SpouseEmail) ? null : model.SpouseEmail;
                spouseInfo.MobilePhone = string.IsNullOrWhiteSpace(model.SpouseMobilePhone) ? null : model.SpouseMobilePhone;

                spouseInfo.Kovil = string.IsNullOrWhiteSpace(model.SpouseKovil) ? string.Empty : model.SpouseKovil;
                spouseInfo.KovilPirivu = string.IsNullOrWhiteSpace(model.SpouseKovilPirivu) ? string.Empty : model.SpouseKovilPirivu;
                spouseInfo.NativePlace = string.IsNullOrWhiteSpace(model.SpouseNativePlace) ? string.Empty : model.SpouseNativePlace;

                familyContact.Spouse = spouseInfo;
            }

            familyContact.Kids = new List<KidsInfoModel>();


            if (!string.IsNullOrWhiteSpace(model.Kid1FirstName) || !string.IsNullOrWhiteSpace(model.Kid1Age) || !string.IsNullOrWhiteSpace(model.Kid1Gender))
            {
                var kid1 = new KidsInfoModel();
                kid1.FirstName = string.IsNullOrWhiteSpace(model.Kid1FirstName) ? string.Empty : model.Kid1FirstName;
                var kidAgeStr = string.IsNullOrWhiteSpace(model.Kid1Age) ? "0" : model.Kid1Age;
                int KidAge = 0;
                int.TryParse(kidAgeStr, out KidAge);
                kid1.Age = KidAge;
                kid1.Gender = string.IsNullOrWhiteSpace(model.Kid1Gender) ? string.Empty : model.Kid1Gender;
                familyContact.Kids.Add(kid1);
            }

            if (!string.IsNullOrWhiteSpace(model.Kid2FirstName) || !string.IsNullOrWhiteSpace(model.Kid2Age) || !string.IsNullOrWhiteSpace(model.Kid2Gender))
            {
                var kid2 = new KidsInfoModel();
                kid2.FirstName = string.IsNullOrWhiteSpace(model.Kid2FirstName) ? string.Empty : model.Kid2FirstName;
                var kidAgeStr = string.IsNullOrWhiteSpace(model.Kid2Age) ? "0" : model.Kid2Age;
                int KidAge = 0;
                int.TryParse(kidAgeStr, out KidAge);
                kid2.Age = KidAge;
                kid2.Gender = string.IsNullOrWhiteSpace(model.Kid2Gender) ? string.Empty : model.Kid2Gender;
                familyContact.Kids.Add(kid2);
            }

            if (!string.IsNullOrWhiteSpace(model.Kid3FirstName) || !string.IsNullOrWhiteSpace(model.Kid3Age) || !string.IsNullOrWhiteSpace(model.Kid3Gender))
            {
                var kid3 = new KidsInfoModel();
                kid3.FirstName = string.IsNullOrWhiteSpace(model.Kid3FirstName) ? string.Empty : model.Kid3FirstName;
                var kidAgeStr = string.IsNullOrWhiteSpace(model.Kid3Age) ? "0" : model.Kid3Age;
                int KidAge = 0;
                int.TryParse(kidAgeStr, out KidAge);
                kid3.Age = KidAge;
                kid3.Gender = string.IsNullOrWhiteSpace(model.Kid3Gender) ? string.Empty : model.Kid3Gender;
                familyContact.Kids.Add(kid3);
            }

            if (!string.IsNullOrWhiteSpace(model.Kid4FirstName) || !string.IsNullOrWhiteSpace(model.Kid4Age) || !string.IsNullOrWhiteSpace(model.Kid4Gender))
            {
                var kid4 = new KidsInfoModel();
                kid4.FirstName = string.IsNullOrWhiteSpace(model.Kid4FirstName) ? string.Empty : model.Kid4FirstName;
                var kidAgeStr = string.IsNullOrWhiteSpace(model.Kid4Age) ? "0" : model.Kid4Age;
                int KidAge = 0;
                int.TryParse(kidAgeStr, out KidAge);
                kid4.Age = KidAge;
                kid4.Gender = string.IsNullOrWhiteSpace(model.Kid4Gender) ? string.Empty : model.Kid4Gender;
                familyContact.Kids.Add(kid4);
            }

            if (!string.IsNullOrWhiteSpace(model.Kid5FirstName) || !string.IsNullOrWhiteSpace(model.Kid5Age) || !string.IsNullOrWhiteSpace(model.Kid5Gender))
            {
                var kid5 = new KidsInfoModel();
                kid5.FirstName = string.IsNullOrWhiteSpace(model.Kid5FirstName) ? string.Empty : model.Kid5FirstName;
                var kidAgeStr = string.IsNullOrWhiteSpace(model.Kid5Age) ? "0" : model.Kid5Age;
                int KidAge = 0;
                int.TryParse(kidAgeStr, out KidAge);
                kid5.Age = KidAge;
                kid5.Gender = string.IsNullOrWhiteSpace(model.Kid5Gender) ? string.Empty : model.Kid5Gender;
                familyContact.Kids.Add(kid5);
            }

            return familyContact;
        }
    }

    public class GMailDispatcher
    {
        MailAddress FromAddress;
        SmtpClient GMailClient;

        public GMailDispatcher()
        {
            FromAddress = new MailAddress("nsna.northeast@gmail.com", "NSNA NorthEast");
            GMailClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential("nsna.northeast@gmail.com", "rGK8vbqT2gTC")
            };
        }

        public bool SendPN2016Message(string ToEmailAddress, string name)
        {
            string subject = "Thanks for registering.";
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendFormat("Hi {0}, <br/>", name);
            htmlBuilder.Append("Thanks for adding your contact info to 2016 Directory.<br/>We are very excited to meet you all in person during our Pillayar Nonbu event.<br/>");
            htmlBuilder.AppendFormat("For more up to date information, Please visit <a href='{0}'>{0}</a><br/><br/>", "http://nsna-ne.azurewebsites.net/");
            htmlBuilder.Append("Thanks, <br/>2016 Pillayar Nonbu Team");

            var ToAddress = new MailAddress(ToEmailAddress, name);
            var from = FromAddress;
            var status = false;
            using (var mailMessage = new MailMessage(from, ToAddress))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = htmlBuilder.ToString();
                mailMessage.IsBodyHtml = true;
                GMailClient.Send(mailMessage);
                status = true;
            }
            return status;
        }
    }

    public class ContactInfoDB
    {
        string connectionstring;
        public ContactInfoDB()
        {
            connectionstring = ConfigurationManager.ConnectionStrings["AzureDB"].ConnectionString;
        }
        public ContactInfoDB(string CSName)
        {
            connectionstring = ConfigurationManager.ConnectionStrings[CSName].ConnectionString;
        }

        public void InsertFamilyInfo(FamilyContactModel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                InsertFamilyContact(connection, model);
                InsertKidsInfo(connection, model);
                connection.Close();
            }
        }

        public void InsertFamilyContact(SqlConnection connection, FamilyContactModel model)
        {
            string commonfields = "FamilyContactGuid, FirstName, Lastname, Gender, Email, HomePhone, MobilePhone, Address, City, State, ZipCode, Kovil, KovilPirivu, NativePlace,MaritalStatus";
            string commonParams = "@FamilyContactGuid, @FirstName, @Lastname, @Gender, @Email, @HomePhone, @MobilePhone, @Address, @City, @State, @ZipCode, @Kovil, @KovilPirivu, @NativePlace, @MaritalStatus";
            string spousefields = "SpouseFirstName,SpouseLastName,SpouseEmail,SpouseMobilePhone,SpouseKovil,SpouseKovilPirivu,SpouseNativePlace";
            string spouseParams = "@SpouseFirstName,@SpouseLastName,@SpouseEmail,@SpouseMobilePhone,@SpouseKovil,@SpouseKovilPirivu,@SpouseNativePlace";

            string query = "INSERT INTO dbo.FamilyContact (" + commonfields + ", CreatedOn) VALUES (" + commonParams + ",@CreatedOn )";
            if (model.Spouse != null)
                query = "INSERT INTO dbo.FamilyContact (" + commonfields + "," + spousefields + ", CreatedOn) VALUES (" + commonParams + "," + spouseParams + ",@CreatedOn )";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                // Primary Contact Value
                cmd.Parameters.Add("@FamilyContactGuid", SqlDbType.VarChar, 128).Value = model.FamilyContactGuid;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = model.FirstName;
                cmd.Parameters.Add("@Lastname", SqlDbType.VarChar, 50).Value = model.LastName;
                cmd.Parameters.Add("@Gender", SqlDbType.VarChar, 1).Value = model.Gender;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = model.Email;
                cmd.Parameters.Add("@HomePhone", SqlDbType.VarChar, 25).Value = model.HomePhone;
                cmd.Parameters.Add("@MobilePhone", SqlDbType.VarChar, 25).Value = string.IsNullOrEmpty(model.MobilePhone) ? Convert.DBNull : model.MobilePhone;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar, 128).Value = string.IsNullOrEmpty(model.Address) ? Convert.DBNull : model.Address;
                cmd.Parameters.Add("@City", SqlDbType.VarChar, 128).Value = model.City;
                cmd.Parameters.Add("@State", SqlDbType.VarChar, 128).Value = model.State;
                cmd.Parameters.Add("@ZipCode", SqlDbType.VarChar, 25).Value = string.IsNullOrEmpty(model.ZipCode) ? Convert.DBNull : model.ZipCode;
                cmd.Parameters.Add("@Kovil", SqlDbType.VarChar, 50).Value = model.Kovil;
                cmd.Parameters.Add("@KovilPirivu", SqlDbType.VarChar, 50).Value = model.KovilPirivu;
                cmd.Parameters.Add("@NativePlace", SqlDbType.VarChar, 128).Value = model.NativePlace;
                cmd.Parameters.Add("@MaritalStatus", SqlDbType.VarChar, 1).Value = model.MaritalStatus;

                if (model.Spouse != null)
                {
                    var spouse = model.Spouse;
                    cmd.Parameters.Add("@SpouseFirstName", SqlDbType.VarChar, 50).Value = spouse.FirstName;
                    cmd.Parameters.Add("@SpouseLastName", SqlDbType.VarChar, 50).Value = spouse.LastName;

                    cmd.Parameters.Add("@SpouseEmail", SqlDbType.VarChar, 128).Value = string.IsNullOrEmpty(spouse.Email) ? Convert.DBNull : spouse.Email;
                    cmd.Parameters.Add("@SpouseMobilePhone", SqlDbType.VarChar, 25).Value = string.IsNullOrEmpty(spouse.MobilePhone) ? Convert.DBNull : spouse.MobilePhone;

                    cmd.Parameters.Add("@SpouseKovil", SqlDbType.VarChar, 50).Value = spouse.Kovil;
                    cmd.Parameters.Add("@SpouseKovilPirivu", SqlDbType.VarChar, 50).Value = spouse.KovilPirivu;
                    cmd.Parameters.Add("@SpouseNativePlace", SqlDbType.VarChar, 128).Value = spouse.NativePlace;
                }
                cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now.ToLocalTime();
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertKidsInfo(SqlConnection connection, FamilyContactModel model)
        {
            foreach (var kidsInfo in model.Kids)
            {
                string kidsFields = "KidsInfoGuid,FamilyContactGuid,FirstName,Age,Gender,CreatedOn";
                string kidsParams = "@KidsInfoGuid,@FamilyContactGuid,@FirstName,@Age,@Gender,@CreatedOn";
                string query = "INSERT INTO dbo.KidsInfo (" + kidsFields + ") VALUES (" + kidsParams + ")";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    // Primary Contact Value
                    cmd.Parameters.Add("@KidsInfoGuid", SqlDbType.VarChar, 128).Value = Guid.NewGuid().ToString("N");
                    cmd.Parameters.Add("@FamilyContactGuid", SqlDbType.VarChar, 128).Value = model.FamilyContactGuid;
                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = kidsInfo.FirstName;
                    cmd.Parameters.Add("@Age", SqlDbType.SmallInt, 50).Value = kidsInfo.Age;
                    cmd.Parameters.Add("@Gender", SqlDbType.VarChar, 1).Value = kidsInfo.Gender;
                    cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now.ToLocalTime();
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }

    public class AzureFileStorage
    {
        private CloudStorageAccount storageAccount; 
        public AzureFileStorage()
        {
            storageAccount = CloudStorageAccount.Parse(
                 CloudConfigurationManager.GetSetting("AzureStorage"));
        }

        public bool UploadFile(string fileName, Stream mediaStream)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("profilepic");
            if (!container.Exists()) return false;
            CloudBlockBlob imageBlob = container.GetBlockBlobReference(fileName);
            imageBlob.UploadFromStream(mediaStream);
            return true;
        }
    }
}