# SDK .NET para Integração com MaxiPago PIX

[![NuGet Version](https://img.shields.io/nuget/v/MaxiPago.Pix.Sdk.svg)](https://www.nuget.org/packages/MaxiPago.Pix.Sdk/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Este é um SDK para a plataforma .NET, projetado para simplificar e acelerar a integração com a API PIX da **MaxiPago**.

O objetivo desta biblioteca é abstrair a complexidade da comunicação via XML e fornecer uma interface fluente, fortemente tipada e assíncrona para as principais operações do PIX, permitindo que você foque na lógica de negócio da sua aplicação.

## Funcionalidades

✔️ Geração de cobrança PIX via **Checkout** (com dados detalhados do cliente).
✔️ Criação de **Link de Pagamento** com opção de PIX.
✔️ **Cancelamento (Void)** de cobranças PIX não pagas.
✔️ **Estorno/Devolução (Refund)** de cobranças PIX já pagas.
✔️ Modelos C# para facilitar a desserialização de **Notificações (Webhooks)** enviadas pela MaxiPago.

## Instalação

A forma mais fácil de instalar o SDK é através do Gerenciador de Pacotes NuGet.

**Package Manager Console:**
```powershell
Install-Package MaxiPago.Pix.Sdk
```

**.NET CLI:**

```bash
dotnet add package MaxiPago.Pix.Sdk
```

## Como Usar

### 1\. Inicialização do Cliente

Primeiro, instancie o cliente principal, fornecendo suas credenciais (`merchantId`, `merchantKey`) e o ambiente desejado (`Test` ou `Production`).

```csharp
using MaxiPago.Pix.Sdk;
using MaxiPago.Pix.Sdk.Exceptions;
using System.Globalization;

// Garante que o separador decimal seja o ponto "." para a serialização do XML
CultureInfo.CurrentCulture = new CultureInfo("en-US");

const string merchantId = "SEU_MERCHANT_ID";
const string merchantKey = "SEU_MERCHANT_KEY";

var client = new MaxiPagoPixClient(merchantId, merchantKey, MaxiPagoEnvironment.Test);
```

### 2\. Criando uma Cobrança PIX (Modelo Checkout)

Use este modelo para gerar um QR Code PIX em um fluxo de checkout completo, com informações do pagador.

```csharp
var pixCheckoutRequest = new CreatePixCheckoutRequest
{
    ReferenceNum = $"PEDIDO_{DateTime.Now.Ticks}", // A referência única do seu sistema
    ChargeTotal = 199.99m,
    ExpirationTimeInSeconds = 3600, // 1 hora

    // Informações do pagador
    PayerName = "Nome do Cliente",
    PayerCpfOrCnpj = "12345678901",
    PayerDocumentType = "CPF",
    PayerEmail = "cliente@email.com",
    PayerPhone = "11999998888",
    PayerAddress = "Avenida Principal, 123",
    PayerDistrict = "Centro",
    PayerCity = "Sao Paulo",
    PayerState = "SP",
    PayerPostalCode = "01001000"
};

try
{
    var response = await client.CreatePixCheckoutAsync(pixCheckoutRequest);
    Console.WriteLine("PIX (Checkout) criado com sucesso!");
    Console.WriteLine($"Transaction ID: {response.TransactionID}");
    Console.WriteLine($"Chave PIX (Copia e Cola): {response.Emv}");
}
catch (MaxiPagoException ex)
{
    Console.WriteLine($"Erro da API: [{ex.ErrorCode}] {ex.Message}");
}
```

### 3\. Criando um Link de Pagamento com PIX

Ideal para enviar cobranças por e-mail, WhatsApp ou redes sociais.

```csharp
var paymentLinkRequest = new CreatePaymentLinkRequest
{
    ReferenceNum = $"LINK_{DateTime.Now.Ticks}",
    Amount = 59.90m,
    PayerFirstName = "Nome Cliente",
    PayerEmail = "cliente.link@email.com",
    Description = "Pagamento do Produto Exemplo",
    EmailSubject = "Seu link para pagamento está pronto!",
    ExpirationDate = DateTime.Now.AddDays(3).ToString("MM/dd/yyyy") // Formato MM/DD/YYYY
};

try
{
    var response = await client.CreatePaymentLinkAsync(paymentLinkRequest);
    Console.WriteLine("Link de Pagamento criado com sucesso!");
    Console.WriteLine($"URL do Pagamento: {response.Result.Url}");
}
catch (MaxiPagoException ex)
{
    Console.WriteLine($"Erro da API: [{ex.ErrorCode}] {ex.Message}");
}
```

### 4\. Cancelando um PIX (Void)

Use para cancelar um PIX que foi gerado mas **ainda não foi pago**.

```csharp
// Você precisa do transactionID retornado na criação da cobrança
string transactionIdParaCancelar = "ID_DA_TRANSACAO_AQUI";

try
{
    var response = await client.VoidPixAsync(transactionIdParaCancelar);
    Console.WriteLine($"PIX cancelado com sucesso! Mensagem: {response.ResponseMessage}");
}
catch (MaxiPagoException ex)
{
    Console.WriteLine($"Erro da API: [{ex.ErrorCode}] {ex.Message}");
}
```

### 5\. Estornando um PIX (Refund)

Use para devolver o valor de um PIX que **já foi pago**.

```csharp
var refundRequest = new CreatePixRefundRequest
{
    // Você precisa do orderID e referenceNum da transação original
    OrderID = "ORDER_ID_DA_TRANSACAO_PAGA",
    ReferenceNum = "REFERENCE_NUM_DA_TRANSACAO_PAGA",
    Amount = 199.99m // Valor a ser estornado
};

try
{
    var response = await client.RefundPixAsync(refundRequest);
    Console.WriteLine($"Estorno processado com sucesso! Mensagem: {response.ResponseMessage}");
}
catch (MaxiPagoException ex)
{
    Console.WriteLine($"Erro da API: [{ex.ErrorCode}] {ex.Message}");
}
```

## Licença

Este projeto está licenciado sob a Licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## Disclaimer

Este é um SDK não oficial, desenvolvido para facilitar a integração com a plataforma MaxiPago. Ele não é mantido ou endossado pela MaxiPago.