using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SoftFin.Web.Controllers
{
    public class NFESController : Controller
    {
        //
        // GET: /NFES/

        public ActionResult Index()
        {

            return View();
        }

        public JsonResult GetCertificados()
        {
            X509Certificate2Collection lcerts;
            X509Store lStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            lStore.Open(OpenFlags.ReadOnly);
            lcerts = lStore.Certificates;
            foreach (X509Certificate2 cert in lcerts)
            {
                if (cert.HasPrivateKey && cert.NotAfter > DateTime.Now && cert.NotBefore < DateTime.Now)
                {
                    var a = cert.FriendlyName;
                }
            }
            lStore.Close();

            return Json(lcerts,JsonRequestBehavior.AllowGet);
        }

        public JsonResult NotaFiscal(List<int> ids)
        {
            foreach (var item in ids)
            {
                GeraXML(item);
                AssinaXML(item);
            }
            
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        private void GeraXML(int id)
        {
            XmlTextWriter writer = new XmlTextWriter(@"c:\dados\filmes.xml", null);
            writer.WriteStartDocument();
            writer.WriteStartElement("filmes");
            writer.WriteElementString("titulo", "Cada & Companhia");
            writer.WriteElementString("titulo", "007 contra Godzila");
            writer.WriteElementString("titulo", "O segredo do Dr. Haus's");
            writer.WriteEndElement();
            writer.Close();
        }

        private void AssinaXML(int id)
        {

        }


    }




}
