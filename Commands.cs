using System.Linq;
using Twilio;


namespace DiscordBot_2
{
    class Commands
    {
        //SMS Operations
        private TwilioRestClient twilio = new TwilioRestClient(Program.twilioSID, Program.twilioToken);

        //Format a string into an acceptable SMS with greeting.
        public string formatSms(Discord.MessageEventArgs dmsg, string msg)
        {
            return string.Format(Program.smsGreeting, dmsg.User.Name, dmsg.Channel.Name, msg);
        }

        //Send the SMS to the number using Twilio.
        public void SendFormattedSms(string msg, string to, Discord.MessageEventArgs dmsg)
        {
            twilio.SendMessage(Program.twilioFrom, to, formatSms(dmsg, msg));
        }

        //Get the phone number for a given contact in the contact book
        public string getNumberFromContact(string contact)
        {
            string number;
            if (Program.contactBook.TryGetValue(contact.ToLower().Trim(), out number))
                return number;
            else
                return null;
        }

        public string getContactFromNumber(string number)
        {
            return Program.contactBook.FirstOrDefault(x => x.Value == number).Key;
        }

    }
}
