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

    public class help
    {
        public KeyTypes AskingFor;
        public KeyTypes BotOfferingHelp;
    }

    class MessageHandling
    {
        bool CAny = false;
        bool MessageHandeled = false;

        help Help = new help();
        KeyTypes Greet;
        KeyTypes BotNameUse;

        //Keywords
        void CreateKeywords()
        {
            Greet = new KeyTypes();
            BotNameUse = new KeyTypes();
            Help.AskingFor = new KeyTypes();
            Help.BotOfferingHelp = new KeyTypes();


            //Greeting
            string[] greetings = { "Hello", "Hi", "Hai", "Hoi", "Howdy", "Hey", "Hoy", "Ahoy", "Hallo", "Excuse me" };
            string[] greetingsResponse = { "Hello", "Hey", "Hi, Greetings"};
            Greet.KeyWords = greetings;
            Greet.Response = greetingsResponse;

            //BotnameUse
            string[] Botname = { setup.steamFriends.GetPersonaName() };
            string[] BotnameResponse = { $"That is me. What is up?" };
            BotNameUse.KeyWords = Botname;
            BotNameUse.Response = BotnameResponse;

            //
            // Help
            // 

            //Asking for help
            string[] AFH = { "Can you help me?", "Could you help me?", "I Need help", "Help me", "IM IN NEED OF SOME MEDICAL ATTENTION"};
            string[] AFHResponse = { "" };

            //How Do I Work?
            string[] HDIW = { "How to chat", "How to chat with you" };

            //Bot offering his help
            string[] BOHHResponse = { "How can I Help you?", "How can I be of service?"};
            Help.BotOfferingHelp.Response = BOHHResponse;
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

            // This if statement is essential, as otherwise the bot will send a message if the user is still typing
            if (Message.Length != 0)
            {

                if (CAny)
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
            CAny = false;
        }


        //MessageHandler: This generates a response based on the message that was sent by the Sender
        private string MessageHandler(string Message, SteamID senderID)
        {
            int Keywordcount = 0;

            string senderName = setup.steamFriends.GetFriendPersonaName(senderID);
            string OutputMSG = "";

            Message = Message.ToLower();

            //List of Keywords it has to listen for
            List<KeyTypes> TypesList = new List<KeyTypes>()
            {
                Greet,
                BotNameUse
            };

            //Check if the keywords are inside of the message
            for (int i = 0; i < TypesList.Count; i++)
            {
                TypesList[i].InMSG = ArrContains(Message, TypesList[i].KeyWords);
                if (TypesList[i].InMSG)
                {
                    Keywordcount++;
                }
            }


            if (Keywordcount >= 1)
            {
                CAny = true;
            }

            //Reactions to keywords
            
            //If the sender greets the bot, it will send a greeting back
            if (Greet.InMSG)
            {
                
                if (Keywordcount > 1)
                {
                    string Resp = $"Hello, ";
                    OutputMSG = $"{Resp}{OutputMSG}";
                }
                else if (Keywordcount == 1)
                {
                    string Resp = $"{RandomResponse(Greet)} {senderName}";
                    OutputMSG = $"{Resp}";
                }
            }

            //If the botname is used in the sentence
            if (BotNameUse.InMSG)
            {
                string Resp;

                //if only the BotName is used in the sentence
                if (Keywordcount == 1)
                {
                    Resp = $"{RandomResponse(Greet)} {senderName}, {RandomResponse(Help.BotOfferingHelp)}";
                    OutputMSG = $"{Resp}";
                }

                if (Keywordcount >= 2)
                {
                    //if the sender doesn't greet the bot
                    if (!Greet.InMSG)
                    {
                        Resp = $"Hello there {setup.steamFriends.GetFriendPersonaName(senderID)}, {RandomResponse(Help.BotOfferingHelp)}";
                        OutputMSG = $"{Resp}";
                    }
                    //if the Sender greets the bot
                    if (Greet.InMSG)
                    {
                        Resp = $" there {senderName}";
                        OutputMSG = $"{OutputMSG}{Resp}";
                    }
                }


            }
            MessageHandeled = true;
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
            int index = 0;

            string Output = "";
            Random random = new Random();
            index = random.Next(0, Keyword.Response.Length);
            Console.WriteLine(index);
            Output = Keyword.Response[index];

            return Output;
        }


    }
}
