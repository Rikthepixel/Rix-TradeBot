using SteamKit2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rix_Bot
{
    class MessageHandler
    {
        private Setup setup;
        public MessageHandler(Setup setup)
        {
            this.setup = setup;
        }

        public void OnMessageRecieved(SteamFriends.FriendMsgCallback callback)
        {
            var senderID = callback.Sender;
            string Testmessage;
            string Message = callback.Message;
            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);

            Testmessage = "This bot is still work in progress and does not have any responses yet";

            if (Message.Length != 0)
            {
                Console.WriteLine($"Sent {Testmessage} to {senderName}");
                Console.WriteLine(Message);
                setup.steamFriends.SendChatMessage(senderID, EChatEntryType.ChatMsg, Testmessage);

            }
        }
    }
}
