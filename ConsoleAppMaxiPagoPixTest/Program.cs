using MaxiPago.Pix.Sdk;
using MaxiPago.Pix.Sdk.Exceptions;
using MaxiPago.Pix.Sdk.Models.Request.Checkout;
using System.Globalization;

// Garante que o separador decimal seja o ponto "." para a serialização do XML
CultureInfo.CurrentCulture = new CultureInfo("en-US");

// Substitua com suas credenciais de teste
const string merchantId = "35384";
const string merchantKey = "wuojojbgawdbwafbzloh5rou";

Console.WriteLine("Iniciando criação de PIX via SDK MaxiPago (Modelo Checkout)...");

var client = new MaxiPagoPixClient(merchantId, merchantKey, MaxiPagoEnvironment.Test);

// Usando o novo DTO para o modelo checkout
var pixCheckoutRequest = new CreatePixCheckoutRequest
{
    ReferenceNum = $"PIX_SDK_HELIO_{DateTime.Now.Ticks}", // Referência única
    ChargeTotal = 10.50m, // Valor total (use 'm' para decimal)
    ExpirationTimeInSeconds = 1800, // 30 minutos

    PayerName = "Helio Desenvolvedor Teste",
    PayerCpfOrCnpj = "18283341898", // CPF/CNPJ válido
    PayerDocumentType = "CPF",
    PayerEmail = "contato@heliopereira.net.br",
    PayerPhone = "11999998888",
    PayerAddress = "Avenida Paulista",
    PayerAddress2 = "Andar 10",
    PayerDistrict = "Bela Vista",
    PayerCity = "Sao Paulo",
    PayerState = "SP",
    PayerPostalCode = "01310000",

    // EXEMPLO: Enviando informações extras
    ExtraInfoItems = new List<Info>
    {
        new Info { Name = "Produto", Value = "SDK Teste" },
        new Info { Name = "PedidoID", Value = "12345" }
    }
};

try
{
    var response = await client.CreatePixCheckoutAsync(pixCheckoutRequest);

    Console.WriteLine("\nPIX (Checkout) criado com sucesso!");
    Console.WriteLine("--------------------------------");
    Console.WriteLine($"Order ID: {response.OrderID}");
    Console.WriteLine($"Transaction ID: {response.TransactionID}");
    Console.WriteLine($"Reference Number: {response.ReferenceNum}");
    Console.WriteLine($"\nChave PIX (Copia e Cola):\n{response.Emv}");
    Console.WriteLine("\n--------------------------------");
    Console.WriteLine("Resposta completa recebida:");
    Console.WriteLine($"Response Code: {response.ResponseCode}");
    Console.WriteLine($"Response Message: {response.ResponseMessage}");
}
catch (MaxiPagoException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nErro ao comunicar com a MaxiPago:");
    Console.WriteLine($"Código do Erro: {ex.ErrorCode}");
    Console.WriteLine($"Mensagem: {ex.Message}");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nOcorreu um erro inesperado:");
    Console.WriteLine(ex.Message);
    Console.ResetColor();
}

Console.WriteLine("\n======================================================\n");
Console.WriteLine("Iniciando criação de Link de Pagamento com PIX...");

// A API espera o formato de data MM/DD/YYYY
var expiration = DateTime.Now.AddDays(3).ToString("MM/dd/yyyy");

var paymentLinkRequest = new CreatePaymentLinkRequest
{
    ReferenceNum = $"LINK_SDK_HELIO_{DateTime.Now.Ticks}",
    Amount = 59.90m,
    PayerFirstName = "Helio Dev Link",
    PayerEmail = "contato@heliopereira.net.br",
    Description = "Pagamento via Link - Produto Exemplo",
    EmailSubject = "Seu link para pagamento está pronto!",
    ExpirationDate = expiration
};

try
{
    var response = await client.CreatePaymentLinkAsync(paymentLinkRequest);

    Console.WriteLine("\nLink de Pagamento criado com sucesso!");
    Console.WriteLine("--------------------------------");
    Console.WriteLine($"Payment Order ID: {response.Result.PayOrderId}");
    Console.WriteLine($"Mensagem: {response.Result.Message}");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\nURL do Pagamento: {response.Result.Url}");
    Console.ResetColor();
    Console.WriteLine("--------------------------------");

}
catch (MaxiPagoException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nErro ao criar o Link de Pagamento:");
    Console.WriteLine($"Código do Erro: {ex.ErrorCode}");
    Console.WriteLine($"Mensagem: {ex.Message}");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\nOcorreu um erro inesperado:");
    Console.WriteLine(ex.Message);
    Console.ResetColor();
}

Console.WriteLine("\n======================================================\n");
Console.WriteLine("Iniciando fluxo completo: Criação e Cancelamento de PIX...");

// DTO para a criação
var pixToCancelRequest = new CreatePixCheckoutRequest
{
    ReferenceNum = $"PIX_SDK_CANCEL_{DateTime.Now.Ticks}", // Referência única
    ChargeTotal = 10.50m, // Valor total (use 'm' para decimal)
    ExpirationTimeInSeconds = 1800, // 30 minutos

    PayerName = "Helio Desenvolvedor Teste",
    PayerCpfOrCnpj = "18283341898", // CPF/CNPJ válido
    PayerDocumentType = "CPF",
    PayerEmail = "contato@heliopereira.net.br",
    PayerPhone = "11999998888",
    PayerAddress = "Avenida Paulista",
    PayerAddress2 = "Andar 10",
    PayerDistrict = "Bela Vista",
    PayerCity = "Sao Paulo",
    PayerState = "SP",
    PayerPostalCode = "01310000",

    // EXEMPLO: Enviando informações extras
    ExtraInfoItems = new List<Info>
    {
        new Info { Name = "Produto", Value = "SDK Teste" },
        new Info { Name = "PedidoID", Value = "12345" }
    }
};

string transactionIdParaCancelar = string.Empty;

// --- ETAPA 1: CRIAR O PIX ---
try
{
    Console.WriteLine("\n[ETAPA 1] Criando o PIX para posterior cancelamento...");
    var createResponse = await client.CreatePixCheckoutAsync(pixToCancelRequest);
    transactionIdParaCancelar = createResponse.TransactionID;

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"PIX criado com sucesso! Transaction ID: {transactionIdParaCancelar}");
    Console.ResetColor();
}
catch (MaxiPagoException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nErro ao criar o PIX: {ex.Message}");
    Console.ResetColor();
}


// --- ETAPA 2: CANCELAR O PIX (se ele foi criado) ---
if (!string.IsNullOrEmpty(transactionIdParaCancelar))
{
    try
    {
        Console.WriteLine($"\n[ETAPA 2] Cancelando o PIX com Transaction ID: {transactionIdParaCancelar}...");

        // Aguardar um pouco para simular uma ação real
        await Task.Delay(2000);

        var voidResponse = await client.VoidPixAsync(transactionIdParaCancelar);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("PIX cancelado com sucesso!");
        Console.WriteLine($"Mensagem da API: {voidResponse.ResponseMessage}");
        Console.WriteLine($"Transaction ID: {voidResponse.TransactionID}");
        Console.ResetColor();
    }
    catch (MaxiPagoException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nErro ao cancelar o PIX: {ex.Message}");
        Console.WriteLine($"Código do Erro: {ex.ErrorCode}");
        Console.ResetColor();
    }
}


// ... (usings, configuração do CultureInfo, credenciais e cliente) ...

Console.WriteLine("\n======================================================\n");
Console.WriteLine("Iniciando fluxo de Devolução de PIX (Return)...");

// Para uma devolução, você precisaria do orderID e referenceNum
// de uma transação PAGA. Vamos simular com dados de exemplo.
string orderIdDaTransacaoPaga = "0A0115B1:017F0B15166E:C6AA:06ACF34A"; // Exemplo! Substitua pelo ID de um pedido real.
string referenceNumDaTransacaoPaga = "MXP_PIX_130120211126"; // Exemplo!

var returnRequest = new CreatePixRefundRequest
{
    OrderID = orderIdDaTransacaoPaga,
    ReferenceNum = referenceNumDaTransacaoPaga,
    Amount = 15.00m // O valor a ser devolvido
};

try
{
    Console.WriteLine($"\n[ETAPA 3] Devolvendo o PIX com Order ID: {returnRequest.OrderID}...");

    var returnResponse = await client.RefundPixAsync(returnRequest);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("Devolução de PIX processada com sucesso!");
    Console.WriteLine($"Mensagem da API: {returnResponse.ResponseMessage}");
    Console.WriteLine($"Transaction ID da Devolução: {returnResponse.TransactionID}");
    Console.ResetColor();
}
catch (MaxiPagoException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nErro ao processar a devolução do PIX: {ex.Message}");
    Console.WriteLine($"Código do Erro: {ex.ErrorCode}");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nOcorreu um erro inesperado: {ex.Message}");
    Console.ResetColor();
}

Console.ReadLine();