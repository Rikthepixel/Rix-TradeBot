using Rix_Bot.Messages;
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

    internal class help
    {
        public KeyTypes AskingFor;
        public KeyTypes BotOfferingHelp;
    }
    internal class references
    {
        public KeyTypes Sender;
        public KeyTypes Bot;
    }

    class MessageHandling
    {
        private readonly MessageFunctions MF;

        public MessageHandling(MessageFunctions mF)
        {
            MF = mF;
        }
        private readonly Setup setup;
        public MessageHandling(Setup setup)
        {
            this.setup = setup;
        }

        //Keywords
        private KeyTypes Greet;
        private references Refer = new references();
        private help Help = new help();
        //Create a list
        List<KeyTypes> TypesList;


        //Variables
        public int Keywordcount;
        public bool CAny = false;

        //Bot
        string BotName;

        //Message specific
        string Message = null;
        string senderName = null;
        SteamID senderID = null;
        string OutputMSG = null;

        //Keywords
        public void SetupKeywords()
        {
            Greet = new KeyTypes();
            Help.AskingFor = new KeyTypes();
            Help.BotOfferingHelp = new KeyTypes();
            Refer.Sender = new KeyTypes();
            Refer.Bot = new KeyTypes();

            //List of Keywords it has to listen for
            TypesList = new List<KeyTypes>()
            {
                Greet,
                Refer.Sender,
                Refer.Bot
            };

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
            string[] BOHHResponse = { "How can I help you?", "How can I be of service?"};
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


        //MessageHandler: This generates a response based on the message that was sent by the Sender
        public string MessageHandler(string MSG, SteamID SID)
        {
            if (BotName != setup.steamFriends.GetPersonaName())
            {
                BotName = setup.steamFriends.GetPersonaName();
            }

            Keywordcount = 0;
            OutputMSG = null;

            //Sets and gets the sender his/her's identity
            senderID = SID;
            senderName = setup.steamFriends.GetFriendPersonaName(senderID);

            Message = MSG.ToLower();

            //Update the Keyword InMSG boolean variable
            UpdateKeywords(Message);



            //Reactions to keywords
            //If the sender greets the bot, it will send a greeting back

            if (Greet.InMSG)
            {
                Greetings();
            }

            if (Refer.Bot.InMSG)
            {
                Reference();
            }

            return OutputMSG;
        }

        #region Usefull functions


        /// <summary> Updates the list of Keywords thier InMSG boolean variable. Also Sets the CAny variable to True if there is at leased 1 keyword in the message. </summary>
        private void UpdateKeywords(string Message)
        {
            //Check if the keywords are inside of the message
            for (int i = 0; i < TypesList.Count; i++)
            {
                TypesList[i].InMSG = MF.Contains(Message, TypesList[i].KeyWords);
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
        }
        #endregion Usefull functions

        #region Keyword Reactions
        // Greet
        private void Greetings()
        {
            if (MF.OnlyKeyword())
            {
                string Resp = $"{MF.RandomResponse(Greet)} {senderName}";
                OutputMSG = $"{Resp}";
            }

            if (Keywordcount >= 2)
            {
                string Resp = $"{MF.RandomResponse(Greet)} ";
                OutputMSG = $"{Resp}{OutputMSG}";
            }
        }

        private void Reference()
        {
            //If a referal of the bot is used in the sentence
            if (Refer.Bot.InMSG)
            {
                string Resp;
                //if only a referal to the bot is used in the sentence
                if (MF.OnlyKeyword())
                {
                    if (MF.Contains(Message, BotName))
                    {
                        Resp = $"{MF.RandomResponse(Greet)} {senderName}, {MF.RandomResponse(Help.BotOfferingHelp)}";
                        OutputMSG = $"{Resp}";
                    }

                    if (MF.Contains(Message, Refer.Bot.KeyWords[1]))
                    {
                        Resp = $"Uhm, yes {senderName}, me?";
                        OutputMSG = $"{Resp}";
                    }

                    if (MF.ContainsOccuringFirst(Message, Refer.Bot.KeyWords[1], Refer.Bot.KeyWords[0]) && MF.Contains(Message, "?"))
                    {
                        Resp = $"Yes {senderName}, that is me";
                        OutputMSG = $"{Resp}";
                    }
                }

                if (Keywordcount >= 2)
                {
                    //If the sender sends the bot name
                    if (MF.Contains(Message, Refer.Bot.KeyWords[0]))
                    {
                        //if the sender doesn't greet the bot
                        if (!Greet.InMSG)
                        {
                            Resp = $"Hello there {setup.steamFriends.GetFriendPersonaName(senderID)}, {MF.RandomResponse(Help.BotOfferingHelp)}";
                            OutputMSG = $"{Resp}";
                        }
                        //if the Sender greets the bot
                        if (Greet.InMSG)
                        {
                            Resp = $"{MF.RandomResponse(Refer.Bot.RandomAddition)}{senderName}";
                            OutputMSG = $"{OutputMSG}{Resp}";
                        }
                    }
                }
            }
        }
        #endregion Keyword Reactions
    }
}
