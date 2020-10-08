using SteamKit2;
using SteamKit2.GC.TF2.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace Rix_Bot
{
    public struct KeyTypes
    {
        public string[] KeyWords;
        public bool InMSG;
    }

    class MessageHandling
    {
        bool CAny = false;
        KeyTypes Greet;
        KeyTypes BotName;

        //Keywords
        void CreateKeywords()
        {
            //Greeting
            string[] greetings = { "Hello", "Hi", "Hai", "Hoi", "Howdy", "Hey", "Hoy", "Ahoy" };
            Greet.KeyWords = greetings;

            //Botname
            string[] Botname = { setup.steamFriends.GetPersonaName() };
            BotName.KeyWords = Botname;
        }

        private Setup setup;

        public MessageHandling(Setup setup)
        {
            this.setup = setup;
        }

        public void OnMessageRecieved(SteamFriends.FriendMsgCallback callback) //OnMessageRecieved fires when a message is sent and when the user is typing
        {
            CreateKeywords();

            SteamID senderID = callback.Sender; //Gets the Sender His/Her SteamID
            string Message = callback.Message;  //Gets the message sent by the sender
            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);
            string AnswerMessage = MessageHandler(Message, senderID); 
            bool MSGCond = false;
            
            //Handles the Message send condition, if it is false, then no messages will be sent
            if (Message.Length != 0) //prevents messages from being send when the Sender is still typing
            {
                if (CAny)
                {
                    MSGCond = true;
                }
            }
            Console.WriteLine(MSGCond);
            if (MSGCond)
            {
                string Q = "\"";

                //Report the message and the response into the Console
                Console.WriteLine(Message);
                Console.WriteLine($"Sent {Q}{AnswerMessage}{Q} to {senderName}");
                setup.steamFriends.SendChatMessage(senderID, EChatEntryType.ChatMsg, AnswerMessage);
            }
            MSGCond = false;
            CAny = false;
        }


        //MessageHandler: This generates a response based on the message that was sent by the Sender
        private string MessageHandler(string Message, SteamID senderID)
        {
            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);
            string OutputMSG = "";
            Message = Message.ToLower();

            //Setup MSG Contains
            bool CTest = Contains(Message, "Test");
            BotName.InMSG = ArrContains(Message, BotName.KeyWords);
            Greet.InMSG = ArrContains(Message, Greet.KeyWords);

            //Reactions to Keywords
            if (CTest)
            {
                CAny = true;

                string Resp = "This bot is still work in progress and does not have any responses yet";
                OutputMSG = $"{OutputMSG}{Resp}";
            }
            
            //Greeting
            if (Greet.InMSG)
            {
                if (CAny)
                {
                    string Resp = "Hello, ";
                    OutputMSG = $"{Resp}{OutputMSG}";
                } else if (BotName.InMSG) {
                    string Resp = "Hello";
                    OutputMSG = $"{Resp}{OutputMSG}";
                } else if (CAny == false){
                    string Resp = $"Hello {senderName}";
                    OutputMSG = $"{Resp}";
                }

                CAny = true;
            }

            //Botname
            if (BotName.InMSG)
            {
                string Resp;
                if (CAny == false)
                {
                    Resp = $"Yes {senderName}, That is me. What's up? :D";
                    OutputMSG = $"{Resp}";
                } else if(Greet.InMSG)
                {
                    Resp = $" there {senderName}";
                    OutputMSG = $"{OutputMSG}{Resp}";
                }

                CAny = true;
            }

            return OutputMSG;
        }

        bool Contains(string Message, string Word)
        {
            Message = Message.ToLower(); //ToLower to make sure that it is case insensitive.
            Word = Word.ToLower();
            bool Contains = Message.Contains(Word);
            return Contains;
        }

        bool ArrContains(string Message, string[] KeywordsArray)
        {
            bool TContains = false;
            bool ArrContains = false;

            foreach (string Keywords in KeywordsArray)
            {
                TContains = Contains(Message, Keywords);
                if (TContains)
                {
                    ArrContains = true;
                }
            }

            return ArrContains;
        }


    }
}
