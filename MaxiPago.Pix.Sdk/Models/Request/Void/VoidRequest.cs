using MaxiPago.Pix.Sdk.Models.Request.Checkout;
using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Models.Request.Void
{
    [XmlRoot("transaction-request")]
    public class VoidRequest
    {
        [XmlElement("version")]
        public string Version { get; set; } = "3.1.1.15";

        [XmlElement("verification")]
        public Verification Verification { get; set; }

        [XmlElement("order")]
        public VoidOrder Order { get; set; }
    }

    public class VoidOrder
    {
        [XmlElement("void")]
        public VoidDetails Void { get; set; }
    }

    public class VoidDetails
    {
        [XmlElement("transactionId")]
        public string TransactionId { get; set; }
    }
}