using SteamKit2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rix_Bot
{
    class MessageHandling
    {
        private Setup setup;
        public MessageHandling(Setup setup)
        {
            this.setup = setup;
        }

        public void OnMessageRecieved(SteamFriends.FriendMsgCallback callback)
        {
            SteamID senderID = callback.Sender;
            string Message = callback.Message;
            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);

            if (Message.Length != 0)
            {
                
                string AnswerMessage = MessageHandler(Message, senderID);
                Console.WriteLine(Message);
                setup.steamFriends.SendChatMessage(senderID, EChatEntryType.ChatMsg, AnswerMessage);
                Console.WriteLine($"Sent {AnswerMessage} to {senderName}");
            }
        }

        //MessageHandler: This generates a response 
        private string MessageHandler(string Message, SteamID senderID)
        {
            string OutputMSG;
            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);
            OutputMSG = "This bot is still work in progress and does not have any responses yet";
            return OutputMSG;
        }
    }
}
