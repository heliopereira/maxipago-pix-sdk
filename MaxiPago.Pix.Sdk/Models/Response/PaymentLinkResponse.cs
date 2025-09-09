using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Models.Response
{
    [XmlRoot("api-response")]
    public class PaymentLinkResponse
    {
        [XmlElement("errorCode")]
        public string ErrorCode { get; set; }

        [XmlElement("errorMessage")]
        public string ErrorMessage { get; set; }

        [XmlElement("command")]
        public string Command { get; set; }

        [XmlElement("time")]
        public string Time { get; set; }

        [XmlElement("result")]
        public Result Result { get; set; }
    }

    public class Result
    {
        [XmlElement("pay_order_id")]
        public string PayOrderId { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }
    }
}