using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace PRN232.LMS.API.ResponseModels;

// The Lab 1 response uses projected dictionaries for fields/expand. XmlSerializer
// cannot serialize dictionaries, so preserve that same contract via an XML formatter.
public sealed class EnvelopeXmlFormatter : TextOutputFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public EnvelopeXmlFormatter()
    {
        SupportedMediaTypes.Add("application/xml");
        SupportedMediaTypes.Add("text/xml");
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }
    protected override bool CanWriteType(Type? type) => type == typeof(ApiResponseModel);
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var json = JsonSerializer.SerializeToElement(context.Object, JsonOptions);
        var xml = new XDocument(new XDeclaration("1.0", selectedEncoding.WebName, null), Convert("response", json));
        await context.HttpContext.Response.WriteAsync(xml.ToString(), selectedEncoding, context.HttpContext.RequestAborted);
    }
    private static XElement Convert(string name, JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.Object => new(name, value.EnumerateObject().Select(x => Convert(XmlConvert.EncodeLocalName(x.Name), x.Value))),
        JsonValueKind.Array => new(name, value.EnumerateArray().Select(x => Convert("item", x))),
        JsonValueKind.Null => new(name, new XAttribute(XName.Get("nil", "http://www.w3.org/2001/XMLSchema-instance"), "true")),
        _ => new(name, value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText())
    };
}
