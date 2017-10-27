using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpHeaders
    {
        public const string XParamOverridePrefix = "X-Param-Override-";

        public const string XHttpMethodOverride = "X-Http-Method-Override";

        public const string XAutoBatchCompleted = "X-AutoBatch-Completed"; // How many requests were completed before first failure

        public const string XTag = "X-Tag";

        public const string XUserAuthId = "X-UAId";

        public const string XTrigger = "X-Trigger"; // Trigger Events on UserAgent

        public const string XForwardedFor = "X-Forwarded-For"; // IP Address

        public const string XForwardedPort = "X-Forwarded-Port";  // 80

        public const string XForwardedProtocol = "X-Forwarded-Proto"; // http or https

        public const string XRealIp = "X-Real-IP";

        public const string XLocation = "X-Location";

        public const string XStatus = "X-Status";

        public const string Referer = "Referer";

        public const string CacheControl = "Cache-Control";

        public const string IfModifiedSince = "If-Modified-Since";

        public const string IfUnmodifiedSince = "If-Unmodified-Since";

        public const string IfNoneMatch = "If-None-Match";

        public const string IfMatch = "If-Match";

        public const string LastModified = "Last-Modified";

        public const string Accept = "Accept";

        public const string AcceptEncoding = "Accept-Encoding";

        public const string ContentType = "Content-Type";

        public const string ContentEncoding = "Content-Encoding";

        public const string ContentLength = "Content-Length";

        public const string ContentDisposition = "Content-Disposition";

        public const string Location = "Location";

        public const string SetCookie = "Set-Cookie";

        public const string ETag = "ETag";

        public const string Age = "Age";

        public const string Expires = "Expires";

        public const string Vary = "Vary";

        public const string Authorization = "Authorization";

        public const string WwwAuthenticate = "WWW-Authenticate";

        public const string AllowOrigin = "Access-Control-Allow-Origin";

        public const string AllowMethods = "Access-Control-Allow-Methods";

        public const string AllowHeaders = "Access-Control-Allow-Headers";

        public const string AllowCredentials = "Access-Control-Allow-Credentials";

        public const string ExposeHeaders = "Access-Control-Expose-Headers";

        public const string AccessControlMaxAge = "Access-Control-Max-Age";

        public const string Origin = "Origin";

        public const string RequestMethod = "Access-Control-Request-Method";

        public const string RequestHeaders = "Access-Control-Request-Headers";

        public const string AcceptRanges = "Accept-Ranges";

        public const string ContentRange = "Content-Range";

        public const string Range = "Range";

        public const string SOAPAction = "SOAPAction";

        public const string Allow = "Allow";

        public const string AcceptCharset = "Accept-Charset";

        public const string AcceptLanguage = "Accept-Language";

        public const string Connection = "Connection";

        public const string Cookie = "Cookie";

        public const string ContentLanguage = "Content-Language";

        public const string Expect = "Expect";

        public const string Pragma = "Pragma";

        public const string ProxyAuthenticate = "Proxy-Authenticate";

        public const string ProxyAuthorization = "Proxy-Authorization";

        public const string ProxyConnection = "Proxy-Connection";

        public const string SetCookie2 = "Set-Cookie2";

        public const string TE = "TE";

        public const string Trailer = "Trailer";

        public const string TransferEncoding = "Transfer-Encoding";

        public const string Upgrade = "Upgrade";

        public const string Via = "Via";

        public const string Warning = "Warning";

        public const string Date = "Date";
        public const string Host = "Host";
        public const string UserAgent = "User-Agent";

        public static HashSet<string> RestrictedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Accept,
            Connection,
            ContentLength,
            ContentType,
            Date,
            Expect,
            Host,
            IfModifiedSince,
            Range,
            Referer,
            TransferEncoding,
            UserAgent,
            ProxyConnection,
        };
    }
    /// <summary>
    /// 
    /// </summary>
    public static class MimeTypes
    {
        public static Dictionary<string, string> ExtensionMimeTypes = new Dictionary<string, string>();

        public const string Html = "text/html";
        public const string Xml = "application/xml";
        public const string XmlText = "text/xml";
        public const string Json = "application/json";
        public const string JsonText = "text/json";
        public const string Jsv = "application/jsv";
        public const string JsvText = "text/jsv";
        public const string Csv = "text/csv";
        public const string ProtoBuf = "application/x-protobuf";
        public const string JavaScript = "text/javascript";

        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string MultiPartFormData = "multipart/form-data";
        public const string JsonReport = "text/jsonreport";
        public const string Soap11 = "text/xml; charset=utf-8";
        public const string Soap12 = "application/soap+xml";
        public const string Yaml = "application/yaml";
        public const string YamlText = "text/yaml";
        public const string PlainText = "text/plain";
        public const string MarkdownText = "text/markdown";
        public const string MsgPack = "application/x-msgpack";
        public const string Wire = "application/x-wire";
        public const string NetSerializer = "application/x-netserializer";

        public const string ImagePng = "image/png";
        public const string ImageGif = "image/gif";
        public const string ImageJpg = "image/jpeg";

        public const string Bson = "application/bson";
        public const string Binary = "application/octet-stream";
        public const string ServerSentEvents = "text/event-stream";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static string GetExtension(string mimeType)
        {
            switch (mimeType)
            {
                case ProtoBuf:
                    return ".pbuf";
            }

            var parts = mimeType.Split('/');
            if (parts.Length == 1) return "." + parts[0];
            if (parts.Length == 2) return "." + parts[1];

            throw new NotSupportedException("Unknown mimeType: " + mimeType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNameOrExt"></param>
        /// <returns></returns>
        public static string GetMimeType(string fileNameOrExt)
        {
            if (string.IsNullOrEmpty(fileNameOrExt))
                throw new ArgumentNullException("fileNameOrExt");

            var parts = fileNameOrExt.Split('.');
            var fileExt = parts[parts.Length - 1];

            string mimeType;
            if (ExtensionMimeTypes.TryGetValue(fileExt, out mimeType))
            {
                return mimeType;
            }

            switch (fileExt)
            {
                case "jpeg":
                case "gif":
                case "png":
                case "tiff":
                case "bmp":
                case "webp":
                    return "image/" + fileExt;

                case "jpg":
                    return "image/jpeg";

                case "tif":
                    return "image/tiff";

                case "svg":
                    return "image/svg+xml";

                case "htm":
                case "html":
                case "shtml":
                    return "text/html";

                case "js":
                    return "text/javascript";

                case "ts":
                    return "text/x.typescript";

                case "jsx":
                    return "text/jsx";

                case "csv":
                case "css":
                case "sgml":
                    return "text/" + fileExt;

                case "txt":
                    return "text/plain";

                case "wav":
                    return "audio/wav";

                case "mp3":
                    return "audio/mpeg3";

                case "mid":
                    return "audio/midi";

                case "qt":
                case "mov":
                    return "video/quicktime";

                case "mpg":
                    return "video/mpeg";

                case "avi":
                case "mp4":
                case "ogg":
                case "webm":
                    return "video/" + fileExt;

                case "ogv":
                    return "video/ogg";

                case "rtf":
                    return "application/" + fileExt;

                case "xls":
                    return "application/x-excel";

                case "doc":
                    return "application/msword";

                case "ppt":
                    return "application/powerpoint";

                case "gz":
                case "tgz":
                    return "application/x-compressed";

                case "eot":
                    return "application/vnd.ms-fontobject";

                case "ttf":
                    return "application/octet-stream";

                case "woff":
                    return "application/font-woff";
                case "woff2":
                    return "application/font-woff2";

                default:
                    return "application/" + fileExt;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class HttpMethods
    {
        /// <summary>
        /// 
        /// </summary>
        static readonly string[] allVerbs = new[] {
            "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", // RFC 2616
            "PROPFIND", "PROPPATCH", "MKCOL", "COPY", "MOVE", "LOCK", "UNLOCK",    // RFC 2518
            "VERSION-CONTROL", "REPORT", "CHECKOUT", "CHECKIN", "UNCHECKOUT",
            "MKWORKSPACE", "UPDATE", "LABEL", "MERGE", "BASELINE-CONTROL", "MKACTIVITY",  // RFC 3253
            "ORDERPATCH", // RFC 3648
            "ACL",        // RFC 3744
            "PATCH",      // https://datatracker.ietf.org/doc/draft-dusseault-http-patch/
            "SEARCH",     // https://datatracker.ietf.org/doc/draft-reschke-webdav-search/
            "BCOPY", "BDELETE", "BMOVE", "BPROPFIND", "BPROPPATCH", "NOTIFY",
            "POLL",  "SUBSCRIBE", "UNSUBSCRIBE" //MS Exchange WebDav: http://msdn.microsoft.com/en-us/library/aa142917.aspx
        };
        /// <summary>
        /// 
        /// </summary>
        public static HashSet<string> AllVerbs = new HashSet<string>(allVerbs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpVerb"></param>
        /// <returns></returns>
        public static bool HasVerb(string httpVerb)
        {
            return AllVerbs.Contains(httpVerb.ToUpper());
        }
        /// <summary>
        /// 
        /// </summary>
        public const string Get = "GET";
        public const string Put = "PUT";
        public const string Post = "POST";
        public const string Delete = "DELETE";
        public const string Options = "OPTIONS";
        public const string Head = "HEAD";
        public const string Patch = "PATCH";
    }
}
