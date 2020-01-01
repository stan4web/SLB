using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.SharePoint.Client;
using SLBApp.ViewModels;
using SP = Microsoft.SharePoint.Client;

namespace SLBApp.Controllers
{
    //[Authorize]
    public class InvoicesController : Controller
    {
        private IConfiguration _Configuration { get; }

        public InvoicesController(IConfiguration configuration)
        {
            _Configuration = configuration;
        }


        public IActionResult Index(string searchString)
        {
            try
            {
                string userName = _Configuration.GetSection("LoginDetails:Username").Value;
                SecureString password = GetPasswordFromConsoleInput(_Configuration);
                var ctx = new ClientContext("https://slb001.sharepoint.com/sites/scanningcenter");
                ctx.Credentials = new SharePointOnlineCredentials(userName, password);

                //ReadWebs(ctx);

                List customList = ctx.Web.Lists.GetByTitle("DemoVendorInvoice");
                ctx.Load(customList);
                ctx.ExecuteQuery();

                
                ViewBag.ListTitle = customList.Title;
                ViewBag.ListId = customList.Id;


                List businessContacts = ctx.Web.Lists.GetByTitle("DemoVendorInvoice");
                CamlQuery query = new CamlQuery();

                query.ViewXml = $"<View><Query><Where><Eq><FieldRef Name='Title'/><Value Type='Text'>{searchString}</Value></Eq></Where></Query></View>";
                ListItemCollection collListItem = businessContacts.GetItems(query);

                ctx.Load(collListItem);
                ctx.ExecuteQuery();

                var oListItem = collListItem.ToList();
                //////////////////////////////////////start
                ///
                
                foreach (var item  in oListItem)
                {

                    //legal entity
                    var LegalEntity = item["LegalEntity"] as FieldLookupValue;
                    


                    var vendor = item["Vendor"] as FieldLookupValue;
                    if (LegalEntity == null )
                    {
                        var childId_Value = vendor.LookupValue;
                        var childId_Id = vendor.LookupId;

                        var invoice = new VendorInvoice
                        {
                            Id = item.Id,
                            Amount =  item["Amount"].ToString(),
                            Title = item["Title"].ToString(),
                            Vendor = childId_Value.ToString(),
                            Country=item["Country"].ToString(),
                            Currency=item["Currency"].ToString(),
                            InvoiceDate =item["Invoice_x0020_Date"].ToString(),
                            InvoiceNo=item["Invoice_x0020_No"].ToString(),
                            Priority=item["Priority"].ToString(),

                            
                        };

                        ViewBag.Id = invoice.Id;
                        ViewBag.URN = invoice.Title;
                        ViewBag.Vendor = invoice.Vendor;
                        ViewBag.Amount = invoice.Amount;
                        ViewBag.Country = invoice.Country;
                        ViewBag.Currency = invoice.Currency;
                        ViewBag.InvoiceDate = invoice.InvoiceDate;
                        ViewBag.InvoiceNo = invoice.InvoiceNo;
                        ViewBag.Priority = invoice.Priority;
                    }
                    else
                    {
                        var legal_childId_Value = LegalEntity.LookupValue;
                        var legal_childId_Id = LegalEntity.LookupId;
                        var childId_Value = vendor.LookupValue;
                        var childId_Id = vendor.LookupId;

                        var invoice = new VendorInvoice
                        {
                            Id = item.Id,
                            Amount = item["Amount"].ToString(),
                            Title = item["Title"].ToString(),
                            Vendor = childId_Value.ToString(),
                            Country = item["Country"].ToString(),
                            Currency = item["Currency"].ToString(),
                            InvoiceDate = item["Invoice_x0020_Date"].ToString(),
                            InvoiceNo = item["Invoice_x0020_No"].ToString(),
                            Priority = item["Priority"].ToString(),
                            LegalEntity=legal_childId_Value.ToString(),

                        };

                        ViewBag.Id = invoice.Id;
                        ViewBag.URN = invoice.Title;
                        ViewBag.Vendor = invoice.Vendor;
                        ViewBag.Amount = invoice.Amount;
                        ViewBag.Country = invoice.Country;
                        ViewBag.Currency = invoice.Currency;
                        ViewBag.InvoiceDate = invoice.InvoiceDate;
                        ViewBag.InvoiceNo = invoice.InvoiceNo;
                        ViewBag.Priority = invoice.Priority;
                        ViewBag.LegalEntity = invoice.LegalEntity;
                    }
                    

                }
                ///////////////////////////////end
                ViewBag.List = oListItem;

            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return View();
            }
           
            return View();
        }


        public IActionResult UpdateList(string id)
        {
            string userName = _Configuration.GetSection("LoginDetails:Username").Value;
            SecureString password = GetPasswordFromConsoleInput(_Configuration);
            var clientContext = new ClientContext("https://slb001.sharepoint.com/sites/scanningcenter");
            clientContext.Credentials = new SharePointOnlineCredentials(userName, password);

           
            SP.List oList = clientContext.Web.Lists.GetByTitle("DemoVendorInvoice");
            ListItem oListItem = oList.GetItemById(id);

            oListItem["Status"] = "02. Awaiting Verification";
            oListItem["Scan_x0020_Date"] = DateTime.Now;
            oListItem.Update();
            clientContext.ExecuteQuery();

            TempData["success"] = "List updated successfully.";
            return RedirectToAction("Index");
        }
       static SecureString GetPasswordFromConsoleInput(IConfiguration _Configuration)
        {
             

        SecureString securePassword = new SecureString();
            string myPwd = _Configuration.GetSection("LoginDetails:Password").Value;

            foreach (var c in myPwd.ToCharArray())
            {
                securePassword.AppendChar(c);
            }
            return securePassword;
        }

        static void ReadWebs(ClientContext ctx)
        {
            
            ctx.Load(ctx.Web);
            ctx.Load(ctx.Web.Webs);
            ctx.ExecuteQuery();

            Console.WriteLine(ctx.Web.Title);
            foreach (var subweb in ctx.Web.Webs)
            {
                Console.WriteLine(subweb.Title);
            }
        }
        
    }
}