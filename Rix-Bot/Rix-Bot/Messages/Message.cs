﻿using System;
using System.Collections.Generic;
using System.Text;
using SteamKit2;

namespace Rix_Bot.Messages
{
    class Message
    {

        private readonly Setup setup;
        public Message(Setup setup)
        {
            this.setup = setup;
        }
        private MessageHandling MH;
        private MessageFunctions MF;
        public Message()
        {
            MH = new MessageHandling(MF);
            MF = new MessageFunctions(MH);
        }
        public void OnMessageRecieved(SteamFriends.FriendMsgCallback callback) //OnMessageRecieved fires when a message is sent and when the user is typing
        {
            
            MH.SetupKeywords();

            SteamID senderID = callback.Sender; //Gets the Sender His/Her SteamID
            string Message = callback.Message;  //Gets the message sent by the sender
            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);
            string AnswerMessage = MH.MessageHandler(Message, senderID);
            bool MSGCond = false;

            // This if statement is essential, as otherwise the bot will send a message if the user is still typing
            if (Message.Length != 0)
            {

                if (MH.CAny)
                {
                    MSGCond = true;
                }
                //If the Message we generate is not of a valid length
                if (AnswerMessage.Length == 0)
                {
                    MSGCond = false;
                }
            }
            if (MSGCond)
            {
                string Q = "\"";

                //Report the message and the response into the Console
                Console.WriteLine(Message);
                Console.WriteLine($"Sent {Q}{AnswerMessage}{Q} to => {senderName}");

                //Sends the Message calculated in the Message Handler to the Sender
                setup.steamFriends.SendChatMessage(senderID, EChatEntryType.ChatMsg, AnswerMessage);
            }

            // Reset essential variables
            MSGCond = false;
            MH.CAny = false;
        }

    }
}
