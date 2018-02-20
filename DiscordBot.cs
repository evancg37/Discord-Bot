using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DiscordBot_2
{
    class DiscordBot
    {

        public Commands com = new Commands();


        public void ProcessMessage(Object user, Discord.MessageEventArgs msg)
        {
            //Incoming Discord Message.
            //If it starts with !, it's treated as a command.
            //Check the list of commands to see if the command exists.
            //The message is then split into a command and the arguments, 
            //and then passed to be run as a command to ProcessCommand().

            if (msg.Message.Text.StartsWith("!"))
            {
                List<string> words = msg.Message.Text.Split(' ').ToList();
                string command = words.First().Trim('!');
                List<string> args = words.ToList(); args.RemoveAt(0);

                if (Program.commands.Contains(command))
                    ProcessCommand(command, args, msg);
                else
                    SendMessage(msg.Channel, "No such command: \"" + command + "\".");
            }
            
        }

        //Take a command and it's arguments and perform the task.
        public void ProcessCommand(string command, List<string> args, Discord.MessageEventArgs dmsg)
        {

            //Text command. Sends an SMS to a phone number using Twilio.
            if (command == "text")
            {
                string smsTo = args.First(); args.RemoveAt(0); //The to number should be the first argument.
                string smsMessage = string.Join(" ", args.ToArray()); //The rest of the arguments form the body of the message.
                
                //This first part handles the SMS if the contact is a phone number.
                float temp;
                if (float.TryParse(smsTo, out temp))
                {
                    if (isAdmin(dmsg.User))
                    {
                        com.SendFormattedSms(smsMessage, smsTo, dmsg);
                        SendMessage(dmsg.Channel, string.Format(Program.smsSent, smsTo, smsMessage));
                    }
                    else
                        SendMessage(dmsg.Channel, string.Format(Program.smsNotAllowed, dmsg.User.Name));
                }
                //Otherwise send the text to the contact if they're in the contact book.
                else
                {
                    string number = com.getNumberFromContact(smsTo);
                    smsTo = smsTo.First().ToString().ToUpper() + smsTo.Substring(1);

                    if (!string.IsNullOrEmpty(number))
                    {
                        com.SendFormattedSms(smsMessage, number, dmsg);
                        SendMessage(dmsg.Channel, string.Format(Program.smsSent, smsTo, smsMessage));
                    }
                    else
                        SendMessage(dmsg.Channel, string.Format(Program.contactNotFound, smsTo));

                }

            }else if (command == "setchannel")
            {
                Program.growChannel = dmsg.Channel;
                SendMessage(dmsg.Channel, "The channel has been set.");
                SendMessage(dmsg.Channel, (string.Format("The channel name is {0}", Program.growChannel.Name)));


            }
        }

        public bool isAdmin(Discord.User user)
        {
            return Program.admins.Contains(user.Id);
        }

        async public void SendMessage(Discord.Channel channel, string msg)
        {
            if (channel == null)
                channel = Program.growChannel;

            await channel.SendMessage(msg);
        }

        async public void NotifySms(string from, string sms, string smsImageUrls)
        {
            string msg;
            if (string.IsNullOrEmpty(sms) & !string.IsNullOrEmpty(smsImageUrls))
                msg = string.Format(Program.incomingMms, from, smsImageUrls);
            else if (!string.IsNullOrEmpty(sms))
            {
                msg = string.Format(Program.incomingSms, from, sms);
                if (!string.IsNullOrEmpty(smsImageUrls))
                    msg = msg + "\n\nWith attachment:\n\n" + smsImageUrls;
            }else
                msg = string.Format(Program.incomingSms, from, "*Blank message*");

            await Program.growChannel.SendMessage(msg);
        }
        
    }
}
