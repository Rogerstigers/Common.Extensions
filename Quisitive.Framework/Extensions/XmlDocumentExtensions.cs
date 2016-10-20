using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Quisitive.Framework.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static void Sign(this XmlDocument document, X509Certificate2 certificate)
        {
            var signedXml = new SignedXml(document);

            signedXml.SigningKey = certificate.PrivateKey;

            var reference = new Reference();
            reference.Uri = string.Empty; //sign whole document
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();


            var nodes = document.DocumentElement.AppendChild(document.ImportNode(signedXml.GetXml(), true));
        }
    }

    public class SignedXmlWithId : SignedXml
    {
        public SignedXmlWithId(XmlDocument xml) : base(xml)
        {
        }

        public SignedXmlWithId(XmlElement xmlElement)
            : base(xmlElement)
        {
        }

        public override XmlElement GetIdElement(XmlDocument doc, string id)
        {
            // check to see if it's a standard ID reference
            XmlElement idElem = base.GetIdElement(doc, id);

            if (idElem == null) idElem = doc.DocumentElement;

            return idElem;
        }
    }
}
