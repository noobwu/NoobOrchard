using System.IO;

namespace Orchard.Client.Web
{
    public interface IContentTypeWriter
    {


        ResponseSerializerDelegate GetResponseSerializer(string contentType);
    }

    public delegate string TextSerializerDelegate(object dto);

    public delegate void StreamSerializerDelegate(IRequest requestContext, object dto, Stream outputStream);

    public delegate void ResponseSerializerDelegate(IRequest requestContext, object dto, IResponse httpRes);
}