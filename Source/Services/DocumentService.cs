using DinkToPdf;
using DinkToPdf.Contracts;
using PDFDemo.Interfaces;
using PDFDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFDemo.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IConverter _converter;
        private readonly IRazorRendererHelper _razorRendererHelper;
        

        public DocumentService(
            IConverter converter,
            IRazorRendererHelper razorRendererHelper)
        {
            _converter = converter;
            _razorRendererHelper = razorRendererHelper;
        }

        public byte[] GeneratePdfFromString()
        {
            var htmlContent = $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <style>
                p{{
                    width: 80%;
                    font-weight:normal; 
                    font-family: Helvetica Neue; 
                    font-size: 11px; 
                    color: #666666;
                    border-right: .75px solid #CCCCCC
                }}

                h1{{
                    width: 80%;
                    font-weight:900; 
                    font-family: Lato; 
                    font-size: 20px; 
                    color: #3C3C3C;
                }}
                </style>
            </head>
            <body>
                <h1>AWS CodePipeline</h1>
                <p>AWS CodePipeline is a fully managed service that helps to automate the application’s continuous delivery process. CodePipeline automates the build, test, and deploy phases of your release process every time there is a code change.</p>
            </body>
            </html>
            ";

            return GeneratePdf(htmlContent);
        }

        public byte[] GeneratePdfFromRazorView()
        {
            var invoiceViewModel = GetInvoiceModel();
            var partialName = "/Views/PdfTemplate/InvoiceDetails.cshtml";
            var htmlContent = _razorRendererHelper.RenderPartialToString(partialName, invoiceViewModel);
            
            return GeneratePdf(htmlContent);
        }

        private byte[] GeneratePdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 18, Bottom = 18 },
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = "PDF demo from JeminPro", Line = true },
                LoadSettings = new LoadSettings
                {
                    BlockLocalFileAccess = false
                }
            };

            var htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        private InvoiceViewModel GetInvoiceModel()
        {
            var invoiceViewModel = new InvoiceViewModel
            {
                OrderDate = DateTime.Now,
                OrderId = 1234567890,
                DeliveryDate = DateTime.Now.AddDays(10),
                Products = new List<Product>()
                {
                    new Product
                    {
                        ItemName = "Hosting (12 months)",
                        Price = 200
                    },
                    new Product
                    {
                        ItemName = "Domain name (1 year)",
                        Price = 12
                    },
                    new Product
                    {
                        ItemName = "Website design",
                        Price = 1000

                    },
                    new Product
                    {
                        ItemName = "Maintenance",
                        Price = 300
                    },
                    new Product
                    {
                        ItemName = "Customization",
                        Price = 400
                    },
                }
            };

            invoiceViewModel.TotalAmount = invoiceViewModel.Products.Sum(p => p.Price);

            return invoiceViewModel;
        }
    }
}
