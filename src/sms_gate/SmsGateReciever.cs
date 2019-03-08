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
    /// Trida pro odesilani SMS zprav
    /// </summary>
    public class SmsGateReciever
    {
        internal const string ApiXml20RecieverUrl = "https://smsgateapi.sms-sluzba.cz/apixml20/receiver?login=[login]&password=[password]&affiliate=[affiliate]";

        public enum SendSmsResultStatus : int
        {
            OK = 200,
            InvalidMsisdn = 4001,
            InvalidText = 4002,
            InvalidAction = 400,
            InvalidLogin = 401,
            LowCredit = 402,
            GatewayError = 503
        }

        /// <summary>
        /// Vysledek odeslani SMS zpravy
        /// </summary>
        public class SendSmsResult
        {
            /// <summary>
            /// Stav odeslani
            /// </summary>
            public SendSmsResultStatus Status;

            /// <summary>
            /// Popis vysledku odesilani
            /// </summary>
            public string StatusDescription { get; set; }

            /// <summary>
            /// Odchozi zprava
            /// </summary>
            public OutgoingSms Message { get; set; }

            /// <summary>
            /// Pocet fragmentu zpravy
            /// </summary>
            public int Parts { get; set; }

            /// <summary>
            /// Cena odeslane zpravy
            /// </summary>
            public decimal Price { get; set; }

            /// <summary>
            /// Aktualni stav kreditu
            /// </summary>
            public decimal Credit { get; set; }
        }

        /// <summary>
        /// Odeslani SMS zpravy
        /// </summary>
        public static SendSmsResult SendSms(OutgoingSms sms, LoginInfo loginInfo)
        {
            if (sms == null)
                throw new ArgumentNullException("message");
            if (loginInfo == null)
                throw new ArgumentNullException("loginInfo");

            #region pozadavek
            var doc = new XDocument();
            var rootEl = new XElement("outgoing_message");
            doc.Add(rootEl);

            if (sms.RequireDeliveryReport.HasValue)
            {
                if (sms.RequireDeliveryReport == true)
                    rootEl.Add(new XElement("dr_request", "20"));
                else
                    rootEl.Add(new XElement("dr_request", "0"));
            }

            rootEl.Add(new XElement("text", new XCData(sms.Text)));
            rootEl.Add(new XElement("recipient", sms.Recipient));

            if (sms.SendAt.HasValue)
            {
                rootEl.Add(new XElement("send_at", sms.SendAt.Value.ToString("yyyyMMddHHmmss")));
            }

            var sb = new StringBuilder();
            using (var sw = new StringWriterUtf8(sb))
            {
                doc.Save(sw);
            }
            #endregion

            string url = ApiXml20RecieverUrl
                .Replace("[login]", HttpUtility.UrlEncodeUnicode(loginInfo.Login))
                .Replace("[password]", HttpUtility.UrlEncodeUnicode(loginInfo.Password))
                .Replace("[affiliate]", loginInfo.Affiliate);

            // odeslani
            string resultString = Communicator.CallUrl(url, sb.ToString(), "POST", "text/xml");

            #region zpracovani odpovedi
            var result = new SendSmsResult() { Status = SendSmsResultStatus.OK, StatusDescription = "OK" };
            result.Message = sms;

            doc = XDocument.Parse(resultString);
            // chyba
            if (string.Equals(doc.Root.Name.LocalName, "messages", StringComparison.OrdinalIgnoreCase))
            {
                var messageEl = doc.Root.Elements("message").First();

                result.Message.OutgoingSmsID = messageEl.Elements("id").FirstOrDefault().Value;
                result.Message.Text = messageEl.Elements("text").FirstOrDefault().Value;
                result.Message.SendAt = DateTime.ParseExact(messageEl.Elements("send_at").FirstOrDefault().Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                result.Message.Recipient = messageEl.Elements("recipient").FirstOrDefault().Value;

                result.Parts = int.Parse(messageEl.Elements("parts").FirstOrDefault().Value);
                result.Price = decimal.Parse(messageEl.Elements("price").FirstOrDefault().Value, CultureInfo.GetCultureInfo(2057));
                result.Credit = decimal.Parse(doc.Root.Elements("credit").First().Value, CultureInfo.GetCultureInfo(2057));
            }
            else
            {
                result.StatusDescription = doc.Root.Elements("message").First().Value;
                switch (doc.Root.Elements("id").First().Value)
                {
                    case "400;1":
                        result.Status = SendSmsResultStatus.InvalidMsisdn;
                        break;
                    case "400;2":
                        result.Status = SendSmsResultStatus.InvalidText;
                        break;
                    case "400":
                        result.Status = SendSmsResultStatus.InvalidAction;
                        break;
                    case "401":
                        result.Status = SendSmsResultStatus.InvalidLogin;
                        break;
                    case "402":
                        result.Status = SendSmsResultStatus.LowCredit;
                        break;
                    case "500":
                        result.Status = SendSmsResultStatus.GatewayError;
                        break;
                    default:
                        break;
                }
            }
            #endregion

            return result;
        }
    }
}
