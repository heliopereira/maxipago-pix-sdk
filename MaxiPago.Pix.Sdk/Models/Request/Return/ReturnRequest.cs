using MaxiPago.Pix.Sdk.Models.Request.Checkout;
using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Models.Request.Return
{
    [XmlRoot("transaction-request")]
    public class ReturnRequest
    {
        [XmlElement("version")]
        public string Version { get; set; } = "3.1.1.15";

        [XmlElement("verification")]
        public Verification Verification { get; set; }

        [XmlElement("order")]
        public ReturnOrder Order { get; set; }
    }

    public class Payment
    {
        [XmlElement("chargeTotal")]
        public decimal ChargeTotal { get; set; }
    }

    public class ReturnOrder
    {
        // Mapeia <pixReturn>
        [XmlElement("pixReturn")]
        public PixReturnDetails PixReturn { get; set; }
    }

    public class PixReturnDetails
    {
        // Mapeia <orderID>
        [XmlElement("orderID")]
        public string OrderID { get; set; }

        // Mapeia <referenceNum>
        [XmlElement("referenceNum")]
        public string ReferenceNum { get; set; }

        // Mapeia <payment>
        [XmlElement("payment")]
        public Payment Payment { get; set; }
    }
}