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
using System.Web.Mvc;

namespace PN2016.Controllers
{
    public class DirectoryController : Controller
    {

        string[] allowedExtension = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
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
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Process Data to Create Data Model
            FamilyInfoDBModel familyContact = ProcessViewModeltoDBModel(model);

            //Insert into DB
            ContactInfoDB contactInfoDB = new ContactInfoDB();
            contactInfoDB.InsertFamilyInfo(familyContact);

            //Upload Pic to Azure.
            var familyPic = model.FamilyPic;
            if (familyPic != null && familyPic.ContentLength != 0 && !string.IsNullOrWhiteSpace(familyContact.FamilyPicFileName))
            {
                var mediaFileName = familyContact.FamilyPicFileName;
                new AzureFileStorage().UploadFile(mediaFileName, familyPic.InputStream);
            }
            
            //Send Email.
            GMailDispatcher mailDispatcher = new GMailDispatcher();
            mailDispatcher.SendPN2016Message(familyContact.Email, familyContact.FirstName+ " "+ familyContact.LastName, familyContact.FamilyContactGuid);
            
            return View("CreateConfirm");
        }

        public ActionResult List()
        {
            throw new NotImplementedException("List All - Coming Soon");
            //return View();
        }

        public ActionResult Detail(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return View("Error", new HandleErrorInfo(new Exception("Id is invalid."), "Directory", "Detail"));
            }

            ContactInfoDB contactInfo = new ContactInfoDB();
            FamilyInfoDBModel familyInfoDBModel = contactInfo.SelectWithKidsInfo(id);
            if (familyInfoDBModel == null || familyInfoDBModel.FamilyContactGuid == null)
            {
                return View("Error", new HandleErrorInfo(new Exception("User Id not found."), "Directory", "Detail"));
            }
            else
            {
                ContactInfoViewModel viewModel = ProcessDBModeltoViewModel(familyInfoDBModel);
                if(viewModel == null)
                {
                    return View("Error", new HandleErrorInfo(new Exception("Error Processing Data."), "Directory", "Detail"));
                }
                else
                {
                    return View(viewModel);
                }
            }
        }

        public ActionResult Edit(string guid)
        {
            throw new NotImplementedException("Update - Coming Soon");
            //return View();
        }

        [HttpPost]
        public ActionResult Edit(ContactInfoViewModel model)
        {
            return View();
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

            var familyPic = model.FamilyPic;
            if (familyPic != null && familyPic.ContentLength != 0)
            {
                var fileExtension = Path.GetExtension(familyPic.FileName).ToLower();
                var index = Array.IndexOf(allowedExtension, fileExtension);
                if (index < 0)
                {
                    ModelState.AddModelError("Family Picture", "Only Images are allowed for Family Picture.");
                }
                model.FamilyPicFileExtn = fileExtension;
            }

            //Validate Kids Information if present
            foreach (var kidsInfo in model.Kids)
            {
                if (string.IsNullOrWhiteSpace(kidsInfo.FirstName) && !(kidsInfo.Age.HasValue) && string.IsNullOrWhiteSpace(kidsInfo.Gender))
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(kidsInfo.FirstName))
                    ModelState.AddModelError("Kid First Name", "Kid's First Name is required");
                if (string.IsNullOrWhiteSpace(kidsInfo.Gender))
                    ModelState.AddModelError("Kid Gender", "Kid's Gender is required");
                if (!(kidsInfo.Age.HasValue))
                {
                    ModelState.AddModelError("Kid Age", "Kid's Age is required");
                }
            }
        }

        private FamilyInfoDBModel ProcessViewModeltoDBModel(ContactInfoViewModel model)
        {
            var familyContact = new FamilyInfoDBModel();
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
            
            familyContact.FamilyPicFileName = string.IsNullOrEmpty(model.FamilyPicFileExtn)? null: string.Concat(familyContact.FamilyContactGuid, model.FamilyPicFileExtn);

            //Attach Spouse
            if (familyContact.MaritalStatus == "M")
            {
                var spouseInfo = new SpouseInfoDBModel();
                spouseInfo.FirstName = string.IsNullOrWhiteSpace(model.SpouseFirstName) ? string.Empty : model.SpouseFirstName;
                spouseInfo.LastName = string.IsNullOrWhiteSpace(model.SpouseLastName) ? string.Empty : model.SpouseLastName;

                spouseInfo.Email = string.IsNullOrWhiteSpace(model.SpouseEmail) ? null : model.SpouseEmail;
                spouseInfo.MobilePhone = string.IsNullOrWhiteSpace(model.SpouseMobilePhone) ? null : model.SpouseMobilePhone;

                spouseInfo.Kovil = string.IsNullOrWhiteSpace(model.SpouseKovil) ? string.Empty : model.SpouseKovil;
                spouseInfo.KovilPirivu = string.IsNullOrWhiteSpace(model.SpouseKovilPirivu) ? string.Empty : model.SpouseKovilPirivu;
                spouseInfo.NativePlace = string.IsNullOrWhiteSpace(model.SpouseNativePlace) ? string.Empty : model.SpouseNativePlace;

                familyContact.Spouse = spouseInfo;
            }

            familyContact.Kids = new List<KidsInfoDBModel>();

            foreach(KidsViewModel kidsInfo in model.Kids)
            {
                if (!string.IsNullOrWhiteSpace(kidsInfo.FirstName) || kidsInfo.Age.HasValue || !string.IsNullOrWhiteSpace(kidsInfo.Gender))
                {
                    var kid = new KidsInfoDBModel();
                    kid.FirstName = string.IsNullOrWhiteSpace(kidsInfo.FirstName) ? string.Empty : kidsInfo.FirstName;
                    kid.Age = kidsInfo.Age.HasValue ? kidsInfo.Age.Value : 0;
                    kid.Gender = string.IsNullOrWhiteSpace(kidsInfo.Gender) ? string.Empty : kidsInfo.Gender;
                    familyContact.Kids.Add(kid);
                }
            }

            return familyContact;
        }

        private ContactInfoViewModel ProcessDBModeltoViewModel(FamilyInfoDBModel familyContactModel)
        {
            if (familyContactModel == null)
                return null;

            ContactInfoViewModel viewModel = new ContactInfoViewModel();

            viewModel.FamilyContactGuid = familyContactModel.FamilyContactGuid;

            viewModel.FirstName = familyContactModel.FirstName;
            viewModel.LastName = familyContactModel.LastName;
            viewModel.Gender = familyContactModel.Gender;
            viewModel.MaritalStatus = familyContactModel.MaritalStatus;

            viewModel.Email = familyContactModel.Email;
            viewModel.HomePhone = familyContactModel.HomePhone;
            viewModel.MobilePhone = familyContactModel.MobilePhone;

            viewModel.Address = familyContactModel.Address;
            viewModel.City = familyContactModel.City;
            viewModel.State = familyContactModel.State;
            viewModel.ZipCode = familyContactModel.ZipCode;

            viewModel.Kovil = familyContactModel.Kovil;
            viewModel.KovilPirivu = familyContactModel.KovilPirivu;
            viewModel.NativePlace = familyContactModel.NativePlace;
            
            if(familyContactModel.Spouse != null && familyContactModel.MaritalStatus == "M")
            {
                viewModel.SpouseFirstName = familyContactModel.Spouse.FirstName;
                viewModel.SpouseLastName = familyContactModel.Spouse.LastName;

                viewModel.SpouseEmail = familyContactModel.Spouse.Email;
                viewModel.SpouseMobilePhone = familyContactModel.Spouse.MobilePhone;

                viewModel.SpouseKovil = familyContactModel.Spouse.Kovil;
                viewModel.SpouseKovilPirivu = familyContactModel.Spouse.KovilPirivu;
                viewModel.SpouseNativePlace = familyContactModel.Spouse.NativePlace;
            }

            if(!string.IsNullOrEmpty(familyContactModel.FamilyPicFileName))
            {
                viewModel.FamilyPicFilePath = "https://nsnane.blob.core.windows.net/profilepic/" + familyContactModel.FamilyPicFileName;
            }
            
            if (familyContactModel.Kids.Count == 0)
                return viewModel;

            foreach(var kidsDBModel in familyContactModel.Kids)
            {
                var kid = new KidsViewModel
                {
                    FirstName = kidsDBModel.FirstName,
                    Age = kidsDBModel.Age,
                    Gender = kidsDBModel.Gender
                };
                viewModel.Kids.Add(kid);
            }

            return viewModel;
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

        public bool SendPN2016Message(string ToEmailAddress, string name, string familyGuid)
        {
            string subject = "Thanks for registering.";
            var viewLink = "http://nsna-ne.azurewebsites.net/directory/detail/" + familyGuid;
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.AppendFormat("Hi {0}, <br/>", name);
            htmlBuilder.AppendFormat("Thanks for adding your contact info to 2016 Directory.<br/>You can view the details here - <a href='{0}'>{0}</a><br/>", viewLink );
            htmlBuilder.Append("We are very excited to meet you all in person during our Pillayar Nonbu event.<br/>");
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

        public void InsertFamilyInfo(FamilyInfoDBModel model)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                InsertFamilyContact(connection, model);
                InsertKidsInfo(connection, model);
                connection.Close();
            }
        }

        public void InsertFamilyContact(SqlConnection connection, FamilyInfoDBModel model)
        {
            string commonfields = "FamilyContactGuid, FirstName, Lastname, Gender, Email, HomePhone, MobilePhone, Address, City, State, ZipCode, Kovil, KovilPirivu, NativePlace,MaritalStatus,FamilyPicFileName";
            string commonParams = "@FamilyContactGuid, @FirstName, @Lastname, @Gender, @Email, @HomePhone, @MobilePhone, @Address, @City, @State, @ZipCode, @Kovil, @KovilPirivu, @NativePlace, @MaritalStatus,@FamilyPicFileName";
            string spousefields = "SpouseFirstName,SpouseLastName,SpouseEmail,SpouseMobilePhone,SpouseKovil,SpouseKovilPirivu,SpouseNativePlace";
            string spouseParams = "@SpouseFirstName,@SpouseLastName,@SpouseEmail,@SpouseMobilePhone,@SpouseKovil,@SpouseKovilPirivu,@SpouseNativePlace";

            string query = "INSERT INTO dbo.FamilyContact (" + commonfields + ", CreatedOn, LastModifiedOn) VALUES (" + commonParams + ",@CreatedOn, @CreatedOn )";
            if (model.Spouse != null)
                query = "INSERT INTO dbo.FamilyContact (" + commonfields + "," + spousefields + ", CreatedOn, LastModifiedOn) VALUES (" + commonParams + "," + spouseParams + ",@CreatedOn,@CreatedOn )";

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
                cmd.Parameters.Add("FamilyPicFileName", SqlDbType.VarChar, 128).Value = string.IsNullOrEmpty(model.FamilyPicFileName) ? Convert.DBNull : model.FamilyPicFileName;
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

        public void InsertKidsInfo(SqlConnection connection, FamilyInfoDBModel model)
        {
            foreach (var kidsInfo in model.Kids)
            {
                string kidsFields = "KidsInfoGuid,FamilyContactGuid,FirstName,Age,Gender,CreatedOn,LastModifiedOn";
                string kidsParams = "@KidsInfoGuid,@FamilyContactGuid,@FirstName,@Age,@Gender,@CreatedOn,@CreatedOn";
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

        public FamilyInfoDBModel SelectWithKidsInfo(string id)
        {
            FamilyInfoDBModel familyDBModel = new FamilyInfoDBModel();
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                var fields = "FamilyContactGuid, FirstName, Lastname, Gender, Email, HomePhone, MobilePhone, Address, City, State, ZipCode, Kovil, KovilPirivu, NativePlace,MaritalStatus,FamilyPicFileName";
                var spouseFields = "SpouseFirstName,SpouseLastName,SpouseEmail,SpouseMobilePhone,SpouseKovil,SpouseKovilPirivu,SpouseNativePlace";
                string query = "SELECT " + fields + ","+ spouseFields + ", CreatedOn, LastModifiedOn FROM FamilyContact WHERE FamilyContactGuid = @FamilyContactGuid";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@FamilyContactGuid", SqlDbType.VarChar, 128).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                familyDBModel.FamilyContactGuid = reader.GetString(0);
                                familyDBModel.FirstName = reader.GetString(1);
                                familyDBModel.LastName = reader.GetString(2);
                                familyDBModel.Gender = reader.GetString(3);

                                familyDBModel.Email = reader.GetString(4);
                                familyDBModel.HomePhone = reader.GetString(5);
                                familyDBModel.MobilePhone = reader.IsDBNull(6) ? null : reader.GetString(6);

                                familyDBModel.Address = reader.IsDBNull(7) ? null : reader.GetString(7);
                                familyDBModel.City = reader.GetString(8);
                                familyDBModel.State = reader.GetString(9);
                                familyDBModel.ZipCode = reader.IsDBNull(10) ? null : reader.GetString(10);

                                familyDBModel.Kovil = reader.GetString(11);
                                familyDBModel.KovilPirivu = reader.GetString(12);
                                familyDBModel.NativePlace = reader.GetString(13);
                                familyDBModel.MaritalStatus = reader.GetString(14);
                                familyDBModel.FamilyPicFileName = reader.IsDBNull(15) ? null : reader.GetString(15);

                                if (familyDBModel.MaritalStatus == "M")
                                {
                                    familyDBModel.Spouse = new SpouseInfoDBModel();
                                    familyDBModel.Spouse.FirstName = reader.GetString(16);
                                    familyDBModel.Spouse.LastName = reader.GetString(17);

                                    familyDBModel.Spouse.Email = reader.IsDBNull(18) ? null : reader.GetString(18); ;
                                    familyDBModel.Spouse.MobilePhone = reader.IsDBNull(19) ? null : reader.GetString(19); ;

                                    familyDBModel.Spouse.Kovil = reader.GetString(20);
                                    familyDBModel.Spouse.KovilPirivu = reader.GetString(21);
                                    familyDBModel.Spouse.NativePlace = reader.GetString(22);
                                }

                                familyDBModel.CreatedOn = reader.IsDBNull(23) ? DateTime.MinValue : reader.GetDateTime(23);
                                familyDBModel.LastModifiedOn = reader.IsDBNull(24) ? DateTime.MinValue : reader.GetDateTime(24);
                            }
                        }
                        reader.Close();
                    }
                }
                var kidsField = "KidsInfoGuid,FirstName,Age,Gender";
                string kidsInfoQuery = "SELECT " + kidsField + " FROM KidsInfo WHERE FamilyContactGuid = @FamilyContactGuid";
                using (SqlCommand cmd = new SqlCommand(kidsInfoQuery, connection))
                {
                    cmd.Parameters.Add("@FamilyContactGuid", SqlDbType.VarChar, 128).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            familyDBModel.Kids = new List<KidsInfoDBModel>();
                            while (reader.Read())
                            {
                                var kidsInfo = new KidsInfoDBModel();
                                kidsInfo.KidsInfoGuid = reader.GetString(0);
                                kidsInfo.FirstName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                kidsInfo.Age = reader.GetInt16(2);
                                kidsInfo.Gender = reader.IsDBNull(3) ? null : reader.GetString(3);
                                familyDBModel.Kids.Add(kidsInfo);
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return familyDBModel;
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