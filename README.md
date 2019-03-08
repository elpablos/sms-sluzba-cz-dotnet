# sms-sluzba-cz-dotnet
C# implementation of sms.sluzba.cz API.

 - Zdrojové kódy jsou staženy z [https://sms.sluzba.cz/sms_gate/doc](https://sms.sluzba.cz/sms_gate/doc)
 - Konkrétně pak [.NET knihovna pro komunikaci se SMS Gate API XML](https://sms.sluzba.cz/downloads/Priklad_napojeni_na_SMS_Gate_API_XML_DOTNET.zip)
 
 ## Změna domény a IP adresy

Ke dni 30.6.2019 dojde k technickému oddělení služby sms.sluzba.cz od ostatních služeb provozovaných pod doménou sluzba.cz. To s sebou přinese nutnost úpravy na vaší straně. Změní se doména, na které je provozováno API, změní se IP adresa a změní se SSL certifikát. Obě API budou fungovat souběžně až do 30.6.2019, přechod na nové API tak můžete udělat, kdykoliv vám to bude vyhovovat.
Nová doména druhého řádu je sms-sluzba.cz a všechny domény třetího řádu zůstanou zachovány. Doména API tak bude https://smsgateapi.sms-sluzba.cz.
