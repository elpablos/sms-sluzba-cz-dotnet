using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace sms_sluzba_cz.sms_gate
{
    /// <summary>
    /// Trida pro stahovani seznamu SMS zprav
    /// </summary>
    public class SmsGateSender
    {
        internal const string ApiXml20SenderUrl = "https://smsgateapi.sluzba.cz/apixml20/sender?login=[login]&password=[password]&count=[count]&[query]=1";
        internal const string ApiXml20SenderDeliveryReportUrl = "https://smsgateapi.sluzba.cz/apixml20/sender?login=[login]&password=[password]&id=[id]&act=get_delivery_report";

        /// <summary>
        /// Stazeni seznamu odeslanych SMS zprav
        /// </summary>
        public static List<OutgoingSms> GetOutgoingSmsList(int limit, LoginInfo loginInfo)
        {
            if (loginInfo == null)
                throw new ArgumentNullException("loginInfo");
            if (limit <= 0 || limit > 30)
                throw new ArgumentOutOfRangeException("limit", "Hodnota musi byt vetsi nez 0 a mensi nebo rovna 30.");

            string url = ApiXml20SenderUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[count]", limit.ToString())
                .Replace("[query]", "query_outgoing");

            // odeslani
            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
            var doc = XDocument.Parse(resultString);

            var result = new List<OutgoingSms>(limit);
            if (string.Equals(doc.Root.Name.LocalName, "messages", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var messageEl in doc.Root.Elements("message"))
                {
                    var message = new OutgoingSms();
                    message.OutgoingSmsID = messageEl.Elements("id").FirstOrDefault().Value;
                    message.Text = messageEl.Elements("text").FirstOrDefault().Value;
                    message.SendAt = DateTime.ParseExact(messageEl.Elements("send_at").FirstOrDefault().Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    message.Recipient = messageEl.Elements("recipient").FirstOrDefault().Value;

                    result.Add(message);
                }
            }

            return result;
        }

        /// <summary>
        /// Stazeni seznamu prijatych SMS zprav
        /// </summary>
        public static List<IncomingSms> GetIncomingSmsList(int limit, LoginInfo loginInfo)
        {
            if (loginInfo == null)
                throw new ArgumentNullException("loginInfo");
            if (limit <= 0 || limit > 30)
                throw new ArgumentOutOfRangeException("limit", "Hodnota musi byt vetsi nez 0 a mensi nebo rovna 30.");

            string url = ApiXml20SenderUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[count]", limit.ToString())
                .Replace("[query]", "query_incoming");

            // odeslani
            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
            var doc = XDocument.Parse(resultString);

            var result = new List<IncomingSms>(limit);
            if (string.Equals(doc.Root.Name.LocalName, "messages", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var messageEl in doc.Root.Elements("message"))
                {
                    var message = new IncomingSms();
                    message.IncomingSmsID = messageEl.Elements( "id" ).FirstOrDefault().Value;
                    message.Text = messageEl.Elements("text").FirstOrDefault().Value;
                    message.SentAt = DateTime.ParseExact(messageEl.Elements("sent_at").FirstOrDefault().Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    message.OutgoingSmsID = messageEl.Elements( "in_reply_to" ).FirstOrDefault() != null ? messageEl.Elements( "in_reply_to" ).FirstOrDefault().Value : string.Empty;
                    message.Sender = messageEl.Elements("sender").FirstOrDefault().Value;
                    message.Recipient = messageEl.Elements("recipient").FirstOrDefault().Value;

                    result.Add(message);
                }
            }

            return result;
        }

        /// <summary>
        /// Stazeni seznamu dorucenek
        /// </summary>
        public static List<DeliveryReport> GetDeliveryReportList(int limit, LoginInfo loginInfo)
        {
            if (loginInfo == null)
                throw new ArgumentNullException("loginInfo");
            if (limit <= 0 || limit > 30)
                throw new ArgumentOutOfRangeException("limit", "Hodnota musi byt vetsi nez 0 a mensi nebo rovna 30.");

            string url = ApiXml20SenderUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[count]", limit.ToString())
                .Replace("[query]", "query_delivery_report");

            // odeslani
            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
            var doc = XDocument.Parse(resultString);

            var result = new List<DeliveryReport>(limit);
            if (string.Equals(doc.Root.Name.LocalName, "messages", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var messageEl in doc.Root.Elements("message"))
                {
                    var message = new DeliveryReport();
                    message.IncomingSmsID = messageEl.Elements("id").FirstOrDefault().Value;
                    message.Timestamp = DateTime.ParseExact(messageEl.Elements("delivery_timestamp").FirstOrDefault().Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    message.Status = int.Parse(messageEl.Elements("status").FirstOrDefault().Value);
                    message.Description = messageEl.Elements("description").FirstOrDefault().Value;

                    result.Add(message);
                }
            }

            return result;
        }

        /// <summary>
        /// Stazeni dorucenky pro odchozi zpravu
        /// </summary>
        public static DeliveryReport GetDeliveryReport(string outgoingSmsID, LoginInfo loginInfo)
        {
            if (loginInfo == null)
                throw new ArgumentNullException("loginInfo");
            if (string.IsNullOrEmpty(outgoingSmsID))
                throw new ArgumentNullException("outgoingSmsID");

            string url = ApiXml20SenderDeliveryReportUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[id]", outgoingSmsID);

            // odeslani
            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
            var doc = XDocument.Parse(resultString);

            DeliveryReport result = null;
            if (string.Equals(doc.Root.Name.LocalName, "messages", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var messageEl in doc.Root.Elements("message"))
                {
                    result = new DeliveryReport();
                    result.IncomingSmsID = messageEl.Elements("id").FirstOrDefault().Value;
                    result.Timestamp = DateTime.ParseExact(messageEl.Elements("delivery_timestamp").FirstOrDefault().Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    result.Status = int.Parse(messageEl.Elements("status").FirstOrDefault().Value);
                    result.Description = messageEl.Elements("description").FirstOrDefault().Value;
                    break;
                }
            }
            return result;
        }
    }
}
