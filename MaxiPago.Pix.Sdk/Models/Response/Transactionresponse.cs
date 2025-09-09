using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Models.Response
{
    [XmlRoot("transaction-response")]
    public class TransactionResponse
    {
        [XmlElement("authCode")]
        public string AuthCode { get; set; }

        [XmlElement("orderID")]
        public string OrderID { get; set; }

        [XmlElement("referenceNum")]
        public string ReferenceNum { get; set; }

        [XmlElement("transactionID")]
        public string TransactionID { get; set; }

        [XmlElement("transactionTimestamp")]
        public string TransactionTimestamp { get; set; }

        [XmlElement("responseCode")]
        public string ResponseCode { get; set; }

        [XmlElement("responseMessage")]
        public string ResponseMessage { get; set; }

        [XmlElement("avsResponseCode")]
        public string AvsResponseCode { get; set; }

        [XmlElement("cvvResponseCode")]
        public string CvvResponseCode { get; set; }

        [XmlElement("processorCode")]
        public string ProcessorCode { get; set; }

        [XmlElement("processorMessage")]
        public string ProcessorMessage { get; set; }

        [XmlElement("processorName")]
        public string ProcessorName { get; set; }

        [XmlElement("errorMessage")]
        public string ErrorMessage { get; set; }

        [XmlElement("processorTransactionID")]
        public string ProcessorTransactionID { get; set; }

        [XmlElement("processorReferenceNumber")]
        public string ProcessorReferenceNumber { get; set; }

        // Campos específicos do PIX
        [XmlElement("onlineDebitUrl")]
        public string OnlineDebitUrl { get; set; }

        [XmlElement("emv")]
        public string Emv { get; set; } // O famoso "Copia e Cola"

        [XmlElement("imagem_base64")]
        public string ImagemBase64 { get; set; }
    }
}