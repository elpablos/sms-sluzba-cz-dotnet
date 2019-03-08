using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Net;

namespace sms_sluzba_cz
{
    internal class Communicator
    {
        public static string CallUrl(string url, string data, string method, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;

            if (string.Equals("POST", method, StringComparison.OrdinalIgnoreCase))
            {
                byte[] postBuffer = Encoding.UTF8.GetBytes(data);
                request.ContentLength = postBuffer.Length;

                using (var postData = request.GetRequestStream())
                {
                    postData.Write(postBuffer, 0, postBuffer.Length);
                    postData.Close();
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return stream.ReadToEnd();
                }
            }
        }
    }

    internal class StringWriterUtf8 : StringWriter
    {
        public StringWriterUtf8(StringBuilder sb)
            : base(sb)
        {
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
