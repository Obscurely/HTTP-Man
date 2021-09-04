namespace HTTPMan
{
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

    public static class HttpContentTypeString
    {
        // Fields
        private static readonly string _textPlain = "text/plain";
        private static readonly string _applicationJson = "application/json";
        private static readonly string _applicationXML = "application/xml";
        private static readonly string _textXML = "text/xml";
        private static readonly string _textHTML = "text/html";
        private static readonly string _multipartFormData = "multipart/form-data";
        private static readonly string _applicationXWWWFormUrlEncoded = "application/x-www-form-urlencoded";
        private static readonly string _applicationJavaScript = "application/javascript";
        private static readonly string _applicationPDF = "application/pdf";
        private static readonly string _applicationZIP = "application/zip";
        private static readonly string _audioMPEG = "audio/mpeg";
        private static readonly string _audioXWAV = "audio/x-wav";
        private static readonly string _imageGIF = "image/gif";
        private static readonly string _imageJPEG = "image/jpeg";
        private static readonly string _imagePNG = "image/png";
        private static readonly string _imageTIFF = "image/tiff";
        private static readonly string _imageXIcon = "image/x-icon";
        private static readonly string _imageSVGAndXML = "image/svg+xml";
        private static readonly string _multipartMixed = "multipart/mixed";
        private static readonly string _textCSS = "text/css";
        private static readonly string _textCSV = "text/csv";
        private static readonly string _videoMPEG = "video/mpeg";
        private static readonly string _videoMP4 = "video/mp4";
        private static readonly string _videoXFLV = "video/x-flv";
        private static readonly string _videoWEBM = "video/webm";

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
        /// This function returns a Http Body Content Type based on the id passed.
        /// The id matches with the enum HttpContentType so you can do this:
        /// HttpContentTypeString.Get(HttpContentType.TextPlain);
        /// </summary>
        /// <param name="id">the id of the Content Type works best with the enum HttpContentType.</param>
        /// <returns>Returns a string contating the Content Type for the id passed.</returns>
        public static string Get(int id)
        {
            switch (id)
            {
                case 0:
                    return TextPlain;
                case 1:
                    return ApplicationJson;
                case 2:
                    return ApplicationXML;
                case 3:
                    return TextXML;
                case 4:
                    return TextHTML;
                case 5:
                    return MultipartFormData;
                case 6:
                    return ApplicationXWWWFormUrlEncoded;
                case 7:
                    return ApplicationJavaScript;
                case 8:
                    return ApplicationPDF;
                case 9:
                    return ApplicationZIP;
                case 10:
                    return AudioMPEG;
                case 11:
                    return AudioXWAV;
                case 12:
                    return ImageGIF;
                case 13:
                    return ImageJPEG;
                case 14:
                    return ImagePNG;
                case 15:
                    return ImageTIFF;
                case 16:
                    return ImageXIcon;
                case 17:
                    return ImageSVGAndXML;
                case 18:
                    return MultipartMixed;
                case 19:
                    return TextCSS;
                case 20:
                    return TextCSV;
                case 21:
                    return VideoMPEG;
                case 22:
                    return VideoMP4;
                case 23:
                    return VideoXFLV;
                case 24:
                    return VideoWEBM;
                default:
                    return TextPlain;
            }
        }
    }
}