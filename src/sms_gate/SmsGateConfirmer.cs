using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace sms_sluzba_cz.sms_gate
{
    /// <summary>
    /// Trida pro potvrzovani precteni SMS zprav
    /// </summary>
    public class SmsGateConfirmer
    {
        internal const string ApiXml20ConfirmUrl = "https://smsgateapi.sms-sluzba.cz/apixml20/confirm?login=[login]&password=[password]&id=[id]&type=[type]";

        /// <summary>
        /// Potvrzeni prijeti prichozi zpravy
        /// </summary>
        public static void ConfirmIncomingSms(string incomingSmsID, LoginInfo loginInfo)
        {
            if (string.IsNullOrEmpty(incomingSmsID))
                throw new ArgumentNullException( "incomingSmsID" );

            string url = ApiXml20ConfirmUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[affiliate]", loginInfo.Affiliate)
                .Replace("[id]", incomingSmsID)
                .Replace("[type]", "incoming_message");

            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
        }

        /// <summary>
        /// Potvrzeni precteni odchozi zpravy
        /// </summary>
        public static void ConfirmOutgoingSms(string outgoingSmsID, LoginInfo loginInfo)
        {
            if (string.IsNullOrEmpty(outgoingSmsID))
                throw new ArgumentNullException("outgoingSmsID");

            string url = ApiXml20ConfirmUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[affiliate]", loginInfo.Affiliate)
                .Replace("[id]", outgoingSmsID)
                .Replace("[type]", "outgoing_message");

            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
        }

        /// <summary>
        /// Potvrzeni prijeti dorucenky
        /// </summary>
        public static void ConfirmDeliveryReport(string incomingSmsID, LoginInfo loginInfo)
        {
            if (string.IsNullOrEmpty(incomingSmsID))
                throw new ArgumentNullException("incomingSmsID");

            string url = ApiXml20ConfirmUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[affiliate]", loginInfo.Affiliate)
                .Replace("[id]", incomingSmsID)
                .Replace("[type]", "delivery_report");

            string resultString = Communicator.CallUrl(url, string.Empty, "GET", "text/html");
        }
    }
}
