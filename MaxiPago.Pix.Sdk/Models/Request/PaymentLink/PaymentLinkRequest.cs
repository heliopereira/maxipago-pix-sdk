using MaxiPago.Pix.Sdk.Models.Request.Checkout;
using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Models.Request.PaymentLink
{
    [XmlRoot("api-request")]
    public class PaymentLinkRequest
    {
        [XmlElement("verification")]
        public Verification Verification { get; set; }

        [XmlElement("command")]
        public string Command { get; set; } = "add-payment-order";

        [XmlElement("request")]
        public RequestDetails Request { get; set; }
    }

    public class RequestDetails
    {
        [XmlElement("consumerAuthentication")]
        public string ConsumerAuthentication { get; set; } = "N";

        [XmlElement("referenceNum")]
        public string ReferenceNum { get; set; }

        [XmlElement("fraudCheck")]
        public string FraudCheck { get; set; } = "N";

        [XmlElement("billing")]
        public Billing Billing { get; set; }

        [XmlElement("transactionDetail")]
        public TransactionDetail TransactionDetail { get; set; }
    }

    public class Billing
    {
        [XmlElement("email")]
        public string Email { get; set; }

        [XmlElement("language")]
        public string Language { get; set; } = "pt";

        [XmlElement("firstName")]
        public string FirstName { get; set; }
    }

    public class TransactionDetail
    {
        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("emailSubject")]
        public string EmailSubject { get; set; }

        [XmlElement("expirationDate")]
        public string ExpirationDate { get; set; } // Formato: MM/DD/YYYY

        [XmlElement("acceptPix")]
        public string AcceptPix { get; set; } = "Y";

        [XmlElement("payType")]
        public PayType PayType { get; set; }
    }

    public class PayType
    {
        [XmlElement("creditCard")]
        public CreditCard CreditCard { get; set; }
    }

    public class CreditCard
    {
        [XmlElement("amount")]
        public decimal Amount { get; set; }
    }
}