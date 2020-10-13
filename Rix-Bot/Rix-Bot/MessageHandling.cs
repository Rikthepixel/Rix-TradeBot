using SteamKit2;
using SteamKit2.GC.TF2.Internal;
using SteamKit2.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace Rix_Bot
{

    public class KeyTypes
    {
        public string[] KeyWords;
        public string[] Response;
        public bool InMSG;

        public KeyTypes()
        {
            KeyWords = null;
            Response = null;
            InMSG = false;
        }
    }

    class MessageHandling
    {
        
        KeyTypes Greet;
        KeyTypes BotNameUse;



        bool CAny = false;


        //Keywords
        void CreateKeywords()
        {
            Greet = new KeyTypes();
            BotNameUse = new KeyTypes();
            //Greeting
            string[] greetings = { "Hello", "Hi", "Hai", "Hoi", "Howdy", "Hey", "Hoy", "Ahoy", "Hallo" };
            string[] greetingsResponse = { "Hello", "Hey", "Hi"};
            Greet.KeyWords = greetings;
            Greet.Response = greetingsResponse;

            //BotnameUse
            string[] Botname = { setup.steamFriends.GetPersonaName() };
            string[] BotnameResponse = { $"That is me. What is up?" };
            BotNameUse.KeyWords = Botname;
            BotNameUse.Response = BotnameResponse;
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
                if (AnswerMessage.Length == 0)
                {
                    MSGCond = false;
                }
            }
            Console.WriteLine(AnswerMessage);
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
            string OutputMSG = "IDK WHAT JUST HAPPENED";
            Message = Message.ToLower();

            //Setup MSG Contains
            List<KeyTypes> TypesList = new List<KeyTypes>()
            {
                Greet,
                BotNameUse
            };

            int Keywordcount = 0;

            for (int i = 0; i < TypesList.Count; i++)
            {
                TypesList[i].InMSG = ArrContains(Message, TypesList[i].KeyWords);
            }


            if (Keywordcount >= 1)
            {
                CAny = true;
            }
            bool CTest = Contains(Message, "Test");

            Console.WriteLine(Keywordcount);
            Console.WriteLine(TypesList[0].InMSG);
            if (Keywordcount == 1)
            {


            }
            //Reactions to Keywords
            if (CTest)
            {
                CAny = true;

                string Resp = "I am still a work in progress, but since 8-10-2020 I do have responces!";
                OutputMSG = $"{OutputMSG}{Resp}";
            }
            
            //If the sender greets the bot, it will send a greeting back
            if (Greet.InMSG)
            {
                
                if (Keywordcount > 1)
                {
                    Console.WriteLine("yes");
                    string Resp = $"Hello, ";
                    OutputMSG = $"{Resp}{OutputMSG}";
                }
                else if (Keywordcount == 1)
                {
                    Console.WriteLine("yes");
                    string Resp = $"{RandomResponse(Greet)} {senderName},";
                    OutputMSG = $"{Resp}";
                }
            }

            //If the botname is used in the sentence
            if (BotNameUse.InMSG)
            {
                string Resp;

                if (Keywordcount == 1)
                {
                    Console.WriteLine("yes");
                    Resp = $"{RandomResponse(Greet)} {senderName}";
                    OutputMSG = $"{Resp}";
                }
                else if (Keywordcount >= 2)
                {
                    Console.WriteLine("yes");
                    Resp = $"There {setup.steamFriends.GetFriendPersonaName(senderID)}";
                    OutputMSG = $"{Resp}";
                }

                if (Greet.InMSG)
                {
                    Console.WriteLine("yes");
                    Resp = $" there {senderName}";
                    OutputMSG = $"{OutputMSG}{Resp}";
                }
            }

            return OutputMSG;
        }

        //Check if the message contains a specified word
        bool Contains(string Message, string Word)
        {
            Message = Message.ToLower(); //ToLower to make sure that it is case insensitive.
            Word = Word.ToLower();
            bool Contains = Message.Contains(Word);
            return Contains;
        }

        //Check if a Message contains a keyword out of an array
        bool ArrContains(string Message, string[] keywords)
        {
            bool ArrContains = false;

            foreach (string Keyword in keywords)
            {
                if (Contains(Message, Keyword))
                {
                    ArrContains = true;
                }
            }

            return ArrContains;
        }

        //Chooses one of the Struct responses
        string RandomResponse(KeyTypes Keyword)
        {
            Random random = new Random();
            string Output = "";
            int index = random.Next(0, Keyword.KeyWords.Length);

            Output = Keyword.Response[index];

            return Output;
        }
    }
}
