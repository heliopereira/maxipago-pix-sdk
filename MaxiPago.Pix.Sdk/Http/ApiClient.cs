using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MaxiPago.Pix.Sdk.Http
{
    internal class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
           where TRequest : class
           where TResponse : class
        {
            var xmlRequest = SerializeToXml(requestData);
            var content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");

            var httpResponse = await _httpClient.PostAsync(endpoint, content);
            httpResponse.EnsureSuccessStatusCode();

            var xmlResponse = await httpResponse.Content.ReadAsStringAsync();
            return DeserializeFromXml<TResponse>(xmlResponse);
        }

        private string SerializeToXml<T>(T value)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamespaces);
                return stream.ToString();
            }
        }

        private T DeserializeFromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}