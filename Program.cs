using System;
using System.Collections.Generic;
using Discord;
using System.Threading;
using Twilio;

namespace DiscordBot_2
{
    class Program

    {
        //Discord Settings
        private static DiscordClient dclient;
        private static string discordToken = "DISCORD_TOKEN";
        //Twilio Settings
        public static string twilioFrom = "TWILIO_FROM";
        public static string twilioToken = "TWILIO_TOKEN";
        public static string twilioSID = "TWILIO_SID";
        public static string smsGreeting = "Discord message from {0} in #{1}: {2}";
        public static string smsNotAllowed = "Sorry {0}, you don't have permission to send texts to any number! Use the contacts list instead.";
        public static string smsSent = "Sent message to {0}:\n\n`{1}`";
        public static string contactNotFound = "Couldn't find {0} in the contacts list!";
        public static string incomingSms = "Incoming text message from {0}:\n\n`{1}`";
        public static string incomingMms = "Incoming picture message from {0}:\n\n{1}";
        public static Discord.Channel growChannel;
        //Commands
        public static List<string> commands = new List<string>(){ "commands", "text", "setchannel" };
        

        static void Main(string[] args)
        {
            Console.WriteLine("Before we start...");
                var twilio = new TwilioRestClient(twilioSID, twilioToken);

                var number = twilio.GetIncomingPhoneNumber("INCOMING_NUMBER");
                Console.WriteLine(number.PhoneNumber);

        Console.WriteLine("Initiating...");

            StartBot();

            Console.WriteLine("Done.");

            Console.ReadKey();
        }

        public static void StartBot()
        {
            dclient = new DiscordClient();
            DiscordBot bot = new DiscordBot();

            HttpProcessor processor = new HttpProcessor(bot);
            Thread httpThread = new Thread(new ThreadStart(processor.StartProcessor));
            httpThread.Start();

            dclient.MessageReceived += (sender, msg) =>
            {
                if (!msg.Message.IsAuthor)
                    bot.ProcessMessage(sender, msg);
            };

            dclient.Ready += (temp, idontknowwhattheseare) =>
            {
                Console.WriteLine("Ready!");
            };

            dclient.ExecuteAndWait(async () =>
            {
                Console.WriteLine("Starting Discord Bot...");
                await dclient.Connect(discordToken, TokenType.Bot);
            });

        }
    }
}
