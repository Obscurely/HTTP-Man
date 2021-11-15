namespace HTTPMan
{
    /// <summary>
    /// Enum that contains the http content types for requests supported by Requester.cs.
    /// </summary>
    public enum HttpContentType : int
    {
        TextPlain = 0,
        ApplicationJson = 1,
        ApplicationXML = 2,
        TextXML = 3,
        TextHTML = 4,
        MultipartFormData = 5,
        ApplicationXWWWFormUrlEncoded = 6,
        ApplicationJavaScript = 7,
        ApplicationPDF = 8,
        ApplicationZIP = 9,
        AudioMPEG = 10,
        AudioXWAV = 11,
        ImageGIF = 12,
        ImageJPEG = 13,
        ImagePNG = 14,
        ImageTIFF = 15,
        ImageXIcon = 16,
        ImageSVGAndXML = 17,
        MultipartMixed = 18,
        TextCSS = 19,
        TextCSV = 20,
        VideoMPEG = 21,
        VideoMP4 = 22,
        VideoXFLV = 23,
        VideoWEBM = 24
    }

    /// <summary>
    /// Used for getting the string representation of a value from the enum HttpContentType.
    /// </summary>
    public static class HttpContentTypeExtension
    {
        // Fields
        private const string _textPlain = "text/plain";
        private const string _applicationJson = "application/json";
        private const string _applicationXML = "application/xml";
        private const string _textXML = "text/xml";
        private const string _textHTML = "text/html";
        private const string _multipartFormData = "multipart/form-data";
        private const string _applicationXWWWFormUrlEncoded = "application/x-www-form-urlencoded";
        private const string _applicationJavaScript = "application/javascript";
        private const string _applicationPDF = "application/pdf";
        private const string _applicationZIP = "application/zip";
        private const string _audioMPEG = "audio/mpeg";
        private const string _audioXWAV = "audio/x-wav";
        private const string _imageGIF = "image/gif";
        private const string _imageJPEG = "image/jpeg";
        private const string _imagePNG = "image/png";
        private const string _imageTIFF = "image/tiff";
        private const string _imageXIcon = "image/x-icon";
        private const string _imageSVGAndXML = "image/svg+xml";
        private const string _multipartMixed = "multipart/mixed";
        private const string _textCSS = "text/css";
        private const string _textCSV = "text/csv";
        private const string _videoMPEG = "video/mpeg";
        private const string _videoMP4 = "video/mp4";
        private const string _videoXFLV = "video/x-flv";
        private const string _videoWEBM = "video/webm";

        // Properties
        private static string TextPlain { get { return _textPlain; } }
        private static string ApplicationJson { get { return _applicationJson; } }
        private static string ApplicationXML { get { return _applicationXML; } }
        private static string TextXML { get { return _textXML; } }
        private static string TextHTML { get { return _textHTML; } }
        private static string MultipartFormData { get { return _multipartFormData; } }
        private static string ApplicationXWWWFormUrlEncoded { get { return _applicationXWWWFormUrlEncoded; } }
        private static string ApplicationJavaScript { get { return _applicationJavaScript; } }
        private static string ApplicationPDF { get { return _applicationPDF; } }
        private static string ApplicationZIP { get { return _applicationZIP; } }
        private static string AudioMPEG { get { return _audioMPEG; } }
        private static string AudioXWAV { get { return _audioXWAV; } }
        private static string ImageGIF { get { return _imageGIF; } }
        private static string ImageJPEG { get { return _imageJPEG; } }
        private static string ImagePNG { get { return _imagePNG; } }
        private static string ImageTIFF { get { return _imageTIFF; } }
        private static string ImageXIcon { get { return _imageXIcon; } }
        private static string ImageSVGAndXML { get { return _imageSVGAndXML; } }
        private static string MultipartMixed { get { return _multipartMixed; } }
        private static string TextCSS { get { return _textCSS; } }
        private static string TextCSV { get { return _textCSV; } }
        private static string VideoMPEG { get { return _videoMPEG; } }
        private static string VideoMP4 { get { return _videoMP4; } }
        private static string VideoXFLV { get { return _videoXFLV; } }
        private static string VideoWEBM { get { return _videoWEBM; } }

        /// <summary>
        /// Returns the string representation of the current HttpContentType object.
        /// </summary>
        /// <param name="contentType">This object of type HttpContentType.</param>
        /// <returns>The string representation of the current HttpContentType object.</returns>
        public static string GetString(this HttpContentType contentType)
        {
            switch (contentType)
            {
                case HttpContentType.TextPlain:
                    return TextPlain;
                case HttpContentType.ApplicationJson:
                    return ApplicationJson;
                case HttpContentType.ApplicationXML:
                    return ApplicationXML;
                case HttpContentType.TextXML:
                    return TextXML;
                case HttpContentType.TextHTML:
                    return TextHTML;
                case HttpContentType.MultipartFormData:
                    return MultipartFormData;
                case HttpContentType.ApplicationXWWWFormUrlEncoded:
                    return ApplicationXWWWFormUrlEncoded;
                case HttpContentType.ApplicationJavaScript:
                    return ApplicationJavaScript;
                case HttpContentType.ApplicationPDF:
                    return ApplicationPDF;
                case HttpContentType.ApplicationZIP:
                    return ApplicationZIP;
                case HttpContentType.AudioMPEG:
                    return AudioMPEG;
                case HttpContentType.AudioXWAV:
                    return AudioXWAV;
                case HttpContentType.ImageGIF:
                    return ImageGIF;
                case HttpContentType.ImageJPEG:
                    return ImageJPEG;
                case HttpContentType.ImagePNG:
                    return ImagePNG;
                case HttpContentType.ImageTIFF:
                    return ImageTIFF;
                case HttpContentType.ImageXIcon:
                    return ImageXIcon;
                case HttpContentType.ImageSVGAndXML:
                    return ImageSVGAndXML;
                case HttpContentType.MultipartMixed:
                    return MultipartMixed;
                case HttpContentType.TextCSS:
                    return TextCSS;
                case HttpContentType.TextCSV:
                    return TextCSV;
                case HttpContentType.VideoMPEG:
                    return VideoMPEG;
                case HttpContentType.VideoMP4:
                    return VideoMP4;
                case HttpContentType.VideoXFLV:
                    return VideoXFLV;
                case HttpContentType.VideoWEBM:
                    return VideoWEBM;

                default:
                    return TextPlain;
            }
        }

        /// <summary>
        /// Returns the string representation of the current HttpContentType object.
        /// </summary>
        /// <param name="id">This object of type HttpContentType.</param>
        /// <returns>The string representation of the current HttpContentType object.</returns>
        public static string GetString(this HttpContentType? contentType)
        {
            switch (contentType)
            {
                case HttpContentType.TextPlain:
                    return TextPlain;
                case HttpContentType.ApplicationJson:
                    return ApplicationJson;
                case HttpContentType.ApplicationXML:
                    return ApplicationXML;
                case HttpContentType.TextXML:
                    return TextXML;
                case HttpContentType.TextHTML:
                    return TextHTML;
                case HttpContentType.MultipartFormData:
                    return MultipartFormData;
                case HttpContentType.ApplicationXWWWFormUrlEncoded:
                    return ApplicationXWWWFormUrlEncoded;
                case HttpContentType.ApplicationJavaScript:
                    return ApplicationJavaScript;
                case HttpContentType.ApplicationPDF:
                    return ApplicationPDF;
                case HttpContentType.ApplicationZIP:
                    return ApplicationZIP;
                case HttpContentType.AudioMPEG:
                    return AudioMPEG;
                case HttpContentType.AudioXWAV:
                    return AudioXWAV;
                case HttpContentType.ImageGIF:
                    return ImageGIF;
                case HttpContentType.ImageJPEG:
                    return ImageJPEG;
                case HttpContentType.ImagePNG:
                    return ImagePNG;
                case HttpContentType.ImageTIFF:
                    return ImageTIFF;
                case HttpContentType.ImageXIcon:
                    return ImageXIcon;
                case HttpContentType.ImageSVGAndXML:
                    return ImageSVGAndXML;
                case HttpContentType.MultipartMixed:
                    return MultipartMixed;
                case HttpContentType.TextCSS:
                    return TextCSS;
                case HttpContentType.TextCSV:
                    return TextCSV;
                case HttpContentType.VideoMPEG:
                    return VideoMPEG;
                case HttpContentType.VideoMP4:
                    return VideoMP4;
                case HttpContentType.VideoXFLV:
                    return VideoXFLV;
                case HttpContentType.VideoWEBM:
                    return VideoWEBM;

                default:
                    return TextPlain;
            }
        }

        /// <summary>
        /// Return the matching HttpContentType object for the given string or null if not valid.
        /// </summary>
        /// <param name="contentTypeString"></param>
        /// <returns>The matching HttpContentType object for the given string or null if not valid.</returns>
#nullable enable
        public static HttpContentType? ToHttpContentType(this string contentTypeString)
        {
            switch (contentTypeString)
            {
                case _textPlain:
                    return HttpContentType.TextPlain;
                case _applicationJson:
                    return HttpContentType.ApplicationJson;
                case _applicationXML:
                    return HttpContentType.ApplicationXML;
                case _textXML:
                    return HttpContentType.TextXML;
                case _textHTML:
                    return HttpContentType.TextHTML;
                case _multipartFormData:
                    return HttpContentType.MultipartFormData;
                case _applicationXWWWFormUrlEncoded:
                    return HttpContentType.ApplicationXWWWFormUrlEncoded;
                case _applicationJavaScript:
                    return HttpContentType.ApplicationJavaScript;
                case _applicationPDF:
                    return HttpContentType.ApplicationPDF;
                case _applicationZIP:
                    return HttpContentType.ApplicationZIP;
                case _audioMPEG:
                    return HttpContentType.AudioMPEG;
                case _audioXWAV:
                    return HttpContentType.AudioXWAV;
                case _imageGIF:
                    return HttpContentType.ImageGIF;
                case _imageJPEG:
                    return HttpContentType.ImageJPEG;
                case _imagePNG:
                    return HttpContentType.ImagePNG;
                case _imageTIFF:
                    return HttpContentType.ImageTIFF;
                case _imageXIcon:
                    return HttpContentType.ImageXIcon;
                case _imageSVGAndXML:
                    return HttpContentType.ImageSVGAndXML;
                case _multipartMixed:
                    return HttpContentType.MultipartMixed;
                case _textCSS:
                    return HttpContentType.TextCSS;
                case _textCSV:
                    return HttpContentType.TextCSV;
                case _videoMPEG:
                    return HttpContentType.VideoMPEG;
                case _videoMP4:
                    return HttpContentType.VideoMP4;
                case _videoXFLV:
                    return HttpContentType.VideoXFLV;
                case _videoWEBM:
                    return HttpContentType.VideoWEBM;

                default:
                    return null;
            }
#nullable disable
        }
    }
}