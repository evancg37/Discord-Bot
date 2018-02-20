using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace DiscordBot_2
{
    class HttpProcessor
    {

        private HttpListener listener;
        private DiscordBot usingBot;
        private Commands com;

        public HttpProcessor(DiscordBot givenBot)
        {

            usingBot = givenBot;
            listener = new HttpListener();
            listener.Prefixes.Add("http://+:1337/");

        }

        public void StartProcessor()
        {
            com = new Commands();
            Console.WriteLine("Now listening...");
            listener.Start();
            while (listener.IsListening)
            {
                var context = listener.GetContext();
                Console.WriteLine("Incoming SMS.");

                string smsBody = context.Request.QueryString["Body"].Trim();
                string smsFrom = context.Request.QueryString["From"].Substring(2);

                List<string> smsImageUrls = new List<string>();
                string tempSmsImageUrl;


                for (int i = 0; i < 10; i++) //Process 10 possible MediaUrls
                {
                    tempSmsImageUrl = context.Request.QueryString["MediaUrl" + i.ToString()];
                    if (!string.IsNullOrEmpty(tempSmsImageUrl))
                    {
                        smsImageUrls.Add(tempSmsImageUrl);
                    }
                    else
                        break;
                }

                string smsImageUrlList = "";
                foreach (string url in smsImageUrls) {
                    smsImageUrlList = smsImageUrlList + url + " ";
                }
                smsImageUrlList.Trim();

                string contact = com.getContactFromNumber(smsFrom);

                if (!string.IsNullOrEmpty(contact))
                    smsFrom = contact.First().ToString().ToUpper() + contact.Substring(1); //Capitalize first letter.

                usingBot.NotifySms(smsFrom, smsBody, smsImageUrlList);

            }

            Console.WriteLine("Done listening.");

        }

    }
}
