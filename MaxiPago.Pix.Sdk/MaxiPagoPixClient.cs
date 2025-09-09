using MaxiPago.Pix.Sdk.Exceptions;
using MaxiPago.Pix.Sdk.Http;
using MaxiPago.Pix.Sdk.Models.Request.Checkout;
using MaxiPago.Pix.Sdk.Models.Request.PaymentLink;
using MaxiPago.Pix.Sdk.Models.Request.Return;
using MaxiPago.Pix.Sdk.Models.Request.Void;
using MaxiPago.Pix.Sdk.Models.Response;

namespace MaxiPago.Pix.Sdk
{
    // DTO amigável para o usuário do SDK para o modelo Checkout
    public class CreatePixCheckoutRequest
    {
        public required string ReferenceNum { get; set; }
        public required decimal ChargeTotal { get; set; }
        public int ExpirationTimeInSeconds { get; set; } = 86400;

        // Billing Info
        public required string PayerName { get; set; }
        public required string PayerCpfOrCnpj { get; set; }
        public required string PayerDocumentType { get; set; } // "CPF" ou "CNPJ"
        public required string PayerEmail { get; set; }
        public required string PayerPhone { get; set; }
        public required string PayerAddress { get; set; }
        public string? PayerAddress2 { get; set; }
        public required string PayerDistrict { get; set; }
        public required string PayerCity { get; set; }
        public required string PayerState { get; set; }
        public required string PayerPostalCode { get; set; }
        public string PayerCountry { get; set; } = "BR";
        public List<Info>? ExtraInfoItems { get; set; }
    }

    public class CreatePaymentLinkRequest
    {
        public required string ReferenceNum { get; set; }
        public required decimal Amount { get; set; }
        public required string PayerFirstName { get; set; }
        public required string PayerEmail { get; set; }
        public required string Description { get; set; }
        public string EmailSubject { get; set; } = "Instruções para Pagamento";
        /// <summary>
        /// Data de expiração do link. Deve estar no formato MM/DD/YYYY.
        /// </summary>
        public required string ExpirationDate { get; set; }
    }

    public class CreatePixRefundRequest
    {
        /// <summary>
        /// O orderID retornado na transação de criação do Pix.
        /// </summary>
        public required string OrderID { get; set; }
        public required string ReferenceNum { get; set; }
        /// <summary>
        /// O valor a ser devolvido.
        /// </summary>
        public required decimal Amount { get; set; }
    }

    public class MaxiPagoPixClient
    {
        private readonly ApiClient _apiClient;
        private readonly string _merchantId;
        private readonly string _merchantKey;

        private static readonly Dictionary<MaxiPagoEnvironment, string> EnvironmentUrls = new Dictionary<MaxiPagoEnvironment, string>
        {
            { MaxiPagoEnvironment.Test, "https://testapi.maxipago.net" },
            { MaxiPagoEnvironment.Production, "https://api.maxipago.net" }
        };

        public MaxiPagoPixClient(string merchantId, string merchantKey, MaxiPagoEnvironment environment)
        {
            if (string.IsNullOrWhiteSpace(merchantId)) throw new ArgumentNullException(nameof(merchantId));
            if (string.IsNullOrWhiteSpace(merchantKey)) throw new ArgumentNullException(nameof(merchantKey));

            _merchantId = merchantId;
            _merchantKey = merchantKey;
            _apiClient = new ApiClient(EnvironmentUrls[environment]);
        }

        public async Task<TransactionResponse> CreatePixCheckoutAsync(CreatePixCheckoutRequest requestDto)
        {
            var checkoutRequest = new CheckoutTransactionRequest
            {
                Verification = new Verification { MerchantId = _merchantId, MerchantKey = _merchantKey },
                Order = new CheckoutOrder
                {
                    Sale = new Sale
                    {
                        ReferenceNum = requestDto.ReferenceNum,
                        Payment = new Models.Request.Checkout.Payment { ChargeTotal = requestDto.ChargeTotal },
                        Billing = new Models.Request.Checkout.Billing
                        {
                            Name = requestDto.PayerName,
                            Email = requestDto.PayerEmail,
                            Phone = requestDto.PayerPhone,
                            Address1 = requestDto.PayerAddress,
                            Address2 = requestDto.PayerAddress2,
                            District = requestDto.PayerDistrict,
                            City = requestDto.PayerCity,
                            State = requestDto.PayerState,
                            PostalCode = requestDto.PayerPostalCode,
                            Country = requestDto.PayerCountry,
                            Documents = new List<Document>
                            {
                                new Document
                                {
                                    DocumentType = requestDto.PayerDocumentType,
                                    DocumentValue = requestDto.PayerCpfOrCnpj
                                }
                            }
                        },
                        TransactionDetail = new Models.Request.Checkout.TransactionDetail
                        {
                            PayType = new Models.Request.Checkout.PayType
                            {
                                Pix = new PixCheckoutData
                                {
                                    ExpirationTime = requestDto.ExpirationTimeInSeconds,
                                    ExtraInfo = requestDto.ExtraInfoItems ?? new List<Info>()
                                }
                            }
                        }
                    }
                }
            };

            // Definindo o novo endpoint
            const string paymentLinkEndpoint = "/UniversalAPI/postXML";

            var response = await _apiClient.PostAsync<CheckoutTransactionRequest, TransactionResponse>(paymentLinkEndpoint, checkoutRequest);

            // A lógica de erro agora se baseia no `responseCode`
            if (response.ResponseCode != "0")
            {
                // Concatenamos as mensagens de erro para um diagnóstico mais completo
                string errorMessage = !string.IsNullOrEmpty(response.ErrorMessage)
                    ? response.ErrorMessage
                    : response.ResponseMessage;
                throw new MaxiPagoException(response.ResponseCode, errorMessage);
            }

            return response;
        }

        public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest requestDto)
        {
            var paymentLinkRequest = new PaymentLinkRequest
            {
                Verification = new Verification { MerchantId = _merchantId, MerchantKey = _merchantKey },
                Request = new RequestDetails
                {
                    ReferenceNum = requestDto.ReferenceNum,
                    Billing = new Models.Request.PaymentLink.Billing
                    {
                        Email = requestDto.PayerEmail,
                        FirstName = requestDto.PayerFirstName
                    },
                    TransactionDetail = new Models.Request.PaymentLink.TransactionDetail
                    {
                        Description = requestDto.Description,
                        EmailSubject = requestDto.EmailSubject,
                        ExpirationDate = requestDto.ExpirationDate,
                        PayType = new Models.Request.PaymentLink.PayType
                        {
                            // Abstraindo a regra de negócio da API: o nó <creditCard> é obrigatório.
                            CreditCard = new CreditCard { Amount = requestDto.Amount }
                        }
                    }
                }
            };

            // Definindo o novo endpoint
            const string paymentLinkEndpoint = "/UniversalAPI/postAPI";

            var response = await _apiClient.PostAsync<PaymentLinkRequest, PaymentLinkResponse>(paymentLinkEndpoint, paymentLinkRequest);

            if (response.ErrorCode != "0")
            {
                throw new MaxiPagoException(response.ErrorCode, response.ErrorMessage);
            }

            return response;
        }

        public async Task<TransactionResponse> VoidPixAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentException("O ID da transação (transactionId) é obrigatório.", nameof(transactionId));
            }

            var voidRequest = new VoidRequest
            {
                Verification = new Verification { MerchantId = _merchantId, MerchantKey = _merchantKey },
                Order = new VoidOrder
                {
                    Void = new VoidDetails { TransactionId = transactionId }
                }
            };

            // Reutilizando o endpoint da API Universal
            const string universalApiEndpoint = "/UniversalAPI/postXML";

            // Reutilizando o modelo de resposta TransactionResponse
            var response = await _apiClient.PostAsync<VoidRequest, TransactionResponse>(universalApiEndpoint, voidRequest);

            if (response.ResponseCode != "0")
            {
                string errorMessage = !string.IsNullOrEmpty(response.ErrorMessage)
                    ? response.ErrorMessage
                    : response.ResponseMessage;
                throw new MaxiPagoException(response.ResponseCode, errorMessage);
            }

            return response;
        }

        public async Task<TransactionResponse> RefundPixAsync(CreatePixRefundRequest requestDto)
        {
            var returnRequest = new ReturnRequest
            {
                Verification = new Verification { MerchantId = _merchantId, MerchantKey = _merchantKey },
                Order = new ReturnOrder
                {
                    PixReturn = new PixReturnDetails
                    {
                        OrderID = requestDto.OrderID,
                        ReferenceNum = requestDto.ReferenceNum,
                        Payment = new Models.Request.Return.Payment { ChargeTotal = requestDto.Amount }
                    }
                }
            };

            const string universalApiEndpoint = "/UniversalAPI/postXML";
            var response = await _apiClient.PostAsync<ReturnRequest, TransactionResponse>(universalApiEndpoint, returnRequest);
            if (response.ResponseCode != "0")
            {
                string errorMessage = !string.IsNullOrEmpty(response.ErrorMessage) ? response.ErrorMessage : response.ResponseMessage;
                throw new MaxiPagoException(response.ResponseCode, errorMessage);
            }
            return response;
        }
    }
}