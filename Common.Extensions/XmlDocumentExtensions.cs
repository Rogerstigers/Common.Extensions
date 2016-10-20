using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Common.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static void Sign(this XmlDocument document, X509Certificate2 certificate)
        {
            var signedXml = new SignedXml(document) { SigningKey = certificate.PrivateKey };

            var reference = new Reference { Uri = string.Empty };

            //sign whole document
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            document.DocumentElement?.AppendChild(document.ImportNode(signedXml.GetXml(), true));
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

        public override XmlElement GetIdElement(XmlDocument doc, string id) => base.GetIdElement(doc, id) ?? doc.DocumentElement;
    }
}
