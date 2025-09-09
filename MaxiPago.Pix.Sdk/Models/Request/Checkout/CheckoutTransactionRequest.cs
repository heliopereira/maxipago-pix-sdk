using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Models.Request.Checkout
{
    // A classe TransactionRequest raiz pode ser reutilizada, mas seu conteúdo <order> muda.
    // Para clareza, vamos criar uma específica aqui.
    [XmlRoot("transaction-request")]
    public class CheckoutTransactionRequest
    {
        [XmlElement("version")]
        public string Version { get; set; } = "3.1.1.15";

        [XmlElement("verification")]
        public Verification Verification { get; set; }

        [XmlElement("order")]
        public CheckoutOrder Order { get; set; }
    }

    public class Verification
    {
        [XmlElement("merchantId")]
        public string MerchantId { get; set; }

        [XmlElement("merchantKey")]
        public string MerchantKey { get; set; }
    }

    public class CheckoutOrder
    {
        [XmlElement("sale")]
        public Sale Sale { get; set; }
    }

    public class Sale
    {
        [XmlElement("processorID")]
        public string ProcessorID { get; set; } = "206";

        [XmlElement("referenceNum")]
        public string ReferenceNum { get; set; }

        [XmlElement("customerIdExt")]
        public string? CustomerIdExt { get; set; }

        [XmlElement("billing")]
        public Billing Billing { get; set; }

        [XmlElement("transactionDetail")]
        public TransactionDetail TransactionDetail { get; set; }

        [XmlElement("payment")]
        public Payment Payment { get; set; }
    }

    public class Billing
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("address")]
        public string Address1 { get; set; }

        [XmlElement("address2")]
        public string? Address2 { get; set; }

        [XmlElement("district")]
        public string District { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("state")]
        public string State { get; set; }

        [XmlElement("postalcode")]
        public string PostalCode { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        [XmlArray("documents")]
        [XmlArrayItem("document")]
        public List<Document> Documents { get; set; } = new List<Document>();
    }

    public class Document
    {
        [XmlElement("documentType")]
        public string DocumentType { get; set; } // "CPF" ou "CNPJ"

        [XmlElement("documentValue")]
        public string DocumentValue { get; set; }
    }

    public class TransactionDetail
    {
        [XmlElement("payType")]
        public PayType PayType { get; set; }
    }

    public class PayType
    {
        [XmlElement("pix")]
        public PixCheckoutData Pix { get; set; }
    }

    public class PixCheckoutData
    {
        // O valor default é 86400 segundos (24 horas)
        [XmlElement("expirationTime")]
        public int ExpirationTime { get; set; } = 86400;

        [XmlElement("paymentInfo")]
        public string? PaymentInfo { get; set; }

        [XmlArray("extraInfo")]
        [XmlArrayItem("info")]
        public List<Info> ExtraInfo { get; set; } = new List<Info>();

        /// <summary>
        /// Controla a serialização da propriedade ExtraInfo.
        /// A tag <extraInfo> só será gerada se a lista não for nula e tiver pelo menos um item.
        /// </summary>
        public bool ShouldSerializeExtraInfo()
        {
            return ExtraInfo != null && ExtraInfo.Count > 0;
        }
    }

    public class Info
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }
    }

    public class Payment
    {
        [XmlElement("chargeTotal")]
        public decimal ChargeTotal { get; set; }
    }
}
