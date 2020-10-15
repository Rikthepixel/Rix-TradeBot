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
        public string[] RandomAddition;
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
    public class references
    {
        public KeyTypes Sender;
        public KeyTypes Bot;
    }

    class MessageHandling
    {
        bool CAny = false;

        private Setup setup;
        public MessageHandling(Setup setup)
        {
            this.setup = setup;
        }

        references Refer = new references();
        help Help = new help();

        KeyTypes Greet;

        //Keywords
        void CreateKeywords()
        {
            Greet = new KeyTypes();
            Help.AskingFor = new KeyTypes();
            Help.BotOfferingHelp = new KeyTypes();
            Refer.Sender = new KeyTypes();
            Refer.Bot = new KeyTypes();

            //Greeting
            string[] greetings = { "Hello", "Hi", "Hai", "Hoi", "Howdy", "Hey", "Hoy", "Ahoy", "Hallo", "Excuse me" };
            string[] greetingsResponse = { "Hello", "Hey", "Hi", "Greetings"};
            Greet.KeyWords = greetings;
            Greet.Response = greetingsResponse;


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

            //
            // References
            //

            //Sender
            string[] sender = { "Me", "My", "Mine", "I" };
            Refer.Sender.KeyWords = sender;

            //Bot
            string[] bot = { $"{setup.steamFriends.GetPersonaName()}", "You", "Your", "Yours" };
            string[] BotNameRandomAdd = { "", "there ", "", "" };
            Refer.Bot.KeyWords = bot;
            Refer.Bot.RandomAddition = BotNameRandomAdd;
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
            string OutputMSG = null;

            Message = Message.ToLower();

            //List of Keywords it has to listen for
            List<KeyTypes> TypesList = new List<KeyTypes>()
            {
                Greet,
                Refer.Sender,
                Refer.Bot
            };

            //Check if the keywords are inside of the message
            for (int i = 0; i < TypesList.Count; i++)
            {
                TypesList[i].InMSG = Contains(Message, TypesList[i].KeyWords);
                if (TypesList[i].InMSG)
                {
                    Keywordcount++;
                }
            }

            //Setup the message condition
            if (Keywordcount >= 1)
            {
                CAny = true;
            }

            //Reactions to keywords
            //If the sender greets the bot, it will send a greeting back
            if (Greet.InMSG)
            {
                
                if (Keywordcount >= 2)
                {
                    if (Greet.InMSG)
                    {
                        string Resp = $"{RandomResponse(Greet)} ";
                        OutputMSG = $"{Resp}{OutputMSG}";
                    }
                }
                else if (Keywordcount == 1)
                {
                    string Resp = $"{RandomResponse(Greet)} {senderName}";
                    OutputMSG = $"{Resp}";
                }
            }


            //If a referal of the bot is used in the sentence
            if (Refer.Bot.InMSG)
            {
                string Resp;

                //if only a referal to the bot is used in the sentence
                if (Keywordcount == 1)
                {
                    if (Contains(Message, Refer.Bot.KeyWords[0]))
                    {
                        Resp = $"{RandomResponse(Greet)} {senderName}, {RandomResponse(Help.BotOfferingHelp)}";
                        OutputMSG = $"{Resp}";
                    }

                    if (Contains(Message, Refer.Bot.KeyWords[1]))
                    {
                        if (Contains(Message, Refer.Bot.KeyWords[0]))
                        {
                            Resp = $"Uhm, yes {senderName}, me?";
                            OutputMSG = $"{Resp}";

                            if (OccuringFirst(Message, Refer.Bot.KeyWords[1], Refer.Bot.KeyWords[0]))
                            {
                                if (Contains(Message, "?"))
                                Resp = $"Yes {senderName}, that is me";
                                OutputMSG = $"{Resp}";
                            }
                        }
                    }
                }

                if (Keywordcount >= 2)
                {
                    //If the sender sends the bot name
                    if (Contains(Message, Refer.Bot.KeyWords[0]))
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
                            Resp = $"{RandomResponse(Refer.Bot.RandomAddition)}{senderName}";
                            OutputMSG = $"{OutputMSG}{Resp}";
                        }
                    }
                }
            }
            return OutputMSG;
        }


        //
        // Usefull Functions
        //

        //Check if the message contains a specified word
        bool Contains(string Message, string Word)
        {
            Message = Message.ToLower(); //ToLower to make sure that it is case insensitive.
            Word = Word.ToLower();
            bool Contains = Message.Contains(Word);
            return Contains;
        }
        //Check if a Message contains a keyword out of an array
        bool Contains(string Message, string[] keywords)
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
        //Checks if the 1st word is earlier in the message then the 2nd word
        bool OccuringFirst(string Message, string ShouldOccurFirst, string ShouldOccurLater )
        ///Checks if ShouldOccurFirst Occurs earlier in the sentence then ShouldOccurLater does. If ShouldOccurFirst has a lower index then ShouldOccurLater, then this returns true
        {
            bool OccursFirst = false;
            ShouldOccurFirst = ShouldOccurFirst.ToLower();
            ShouldOccurLater = ShouldOccurLater.ToLower();
            int SOF = Message.IndexOf(ShouldOccurFirst);
            int SOL = Message.IndexOf(ShouldOccurLater);

            if (SOF < SOL)
            {
                OccursFirst = true;
            } else
            {
                OccursFirst = false;
            }
            Console.WriteLine(SOF);
            Console.WriteLine(ShouldOccurFirst);
            Console.WriteLine(SOL);
            Console.WriteLine(ShouldOccurLater);
            Console.WriteLine(OccursFirst);

            return OccursFirst;
        }

        //Checks for start of the sentence and the 
        string[] Divide(string Message)
        {
            string[] Output = null;

            return Output;
        }

        //Chooses a random response
        string RandomResponse(KeyTypes Keyword)
        {
            int index = 0;

            string Output = "";
            Random random = new Random();
            index = random.Next(0, Keyword.Response.Length);
            Output = Keyword.Response[index];

            return Output;
        }
        string RandomResponse(string[] Keywords)
        {
            int index = 0;

            string Output = "";
            Random random = new Random();
            index = random.Next(0, Keywords.Length);
            Output = Keywords[index];

            return Output;
        }

    }
}
