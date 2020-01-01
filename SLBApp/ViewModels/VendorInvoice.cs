using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLBApp.ViewModels
{
    public class VendorInvoice
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public string Vendor { get; set; }
        public string Country { get; set; }
        public string Priority { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string Currency { get; set; }
        //public string ScanDate { get; set; }
       // public string NoOfPages { get; set; }
        public string LegalEntity { get; set; }
    }
}
