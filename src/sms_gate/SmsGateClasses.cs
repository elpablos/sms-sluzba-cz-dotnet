using System;

namespace sms_sluzba_cz.sms_gate
{
    /// <summary>
    /// Prihlasovaci informace
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// Prihlasovaci jmeno
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Heslo
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Identifikator affiliate programu
        /// </summary>
        public string Affiliate { get; set; }
    }

    /// <summary>
    /// Odchozi SMS zprava
    /// </summary>
    public class OutgoingSms
    {
        /// <summary>
        /// ID odchozi zpravy
        /// </summary>
        public string OutgoingSmsID { get; set; }

        /// <summary>
        /// Text odchozi zpravy
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Telefonni cislo prijemce - v mezinarodnim formatu (+420xxxxxxxxx)
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Cas odeslani zpravy - pokud neni zadan, odesle se ihned
        /// </summary>
        public DateTime? SendAt { get; set; }

        /// <summary>
        /// Priznak, zda se ma vyzadovat dorucenka (true = Ano, false = Ne, null = ridi se nastaveni uzivatelskeho uctu na sms.sluzba.cz)
        /// </summary>
        public bool? RequireDeliveryReport { get; set; }
    }

    /// <summary>
    /// Prichozi SMS zprava
    /// </summary>
    public class IncomingSms
    {
        /// <summary>
        /// ID prichozi zpravy
        /// </summary>
        public string IncomingSmsID { get; set; }

        /// <summary>
        /// ID odchozi zpravy, na kterou se odpovida
        /// </summary>
        public string OutgoingSmsID { get; set; }

        /// <summary>
        /// Text odchozi zpravy
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Cas odeslani zpravy
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Telefonni cislo odesilatele
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Telefonni cislo prijemce
        /// </summary>
        public string Recipient { get; set; }
    }

    /// <summary>
    /// Dorucenka
    /// </summary>
    public class DeliveryReport
    {
        /// <summary>
        /// ID odchozi zpravy
        /// </summary>
        public string IncomingSmsID { get; set; }

        /// <summary>
        /// Stav delivery reportu
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Cas delivery reportu
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Popis delivery reportu
        /// </summary>
        public string Description { get; set; }
    }
}
