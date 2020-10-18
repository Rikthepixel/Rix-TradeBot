using System;
using System.Collections.Generic;
using System.Text;

namespace Rix_Bot.Messages
{
    class MessageFunctions
    {
        private MessageHandling MH;

        public MessageFunctions(MessageHandling mH)
        {
            MH = mH;
        }

        //
        // Usefull Functions
        //

        #region Contains
        /// <summary> Checks if the message contains specified string value. If it does it will return true </summary>
        public bool Contains(string Message, string Word)
        {
            Message = Message.ToLower(); //ToLower to make sure that it is case insensitive.
            Word = Word.ToLower();
            bool Output = Message.Contains(Word);
            return Output;
        }

        /// <summary> Checks if the message contains the string values from a specified array. If any of the string values are contained in the message, then it will return true</summary>
        public bool Contains(string Message, string[] keywords)
        {
            bool Output = false;
            foreach (string Keyword in keywords)
            {
                if (Contains(Message, Keyword))
                {
                    Output = true;
                }
            }

            return Output;
        }

        /// <summary> Checks if the message contains both of the asked for keywords. If they are both in the Message it will return true </summary>
        public bool Contains(string Message, string keyword1, string keyword2)

        {
            bool Output = false;
            int Counter = 0;
            if (Contains(Message, keyword1))
            {
                Counter++;
            }
            if (Contains(Message, keyword2))
            {
                Counter++;
            }
            if (Counter == 2)
            {
                Output = true;
            }
            return Output;
        }
        #endregion Contains

        #region RandomResponse
        /// <summary> Chooses a Random response out of a specified KeyTypes Keywords</summary>
        /// <returns>Returns a randomly selected string value</returns>
        public string RandomResponse(KeyTypes Keyword)
        {
            int index = 0;

            string Output = "";
            Random random = new Random();
            index = random.Next(0, Keyword.Response.Length);
            Output = Keyword.Response[index];

            return Output;
        }
        /// <summary> Chooses a Random response out of a specified string array </summary>
        /// <returns>Returns a randomly selected string value</returns>
        public string RandomResponse(string[] Keywords)
        {
            int index = 0;

            string Output;
            Random random = new Random();
            index = random.Next(0, Keywords.Length);
            Output = Keywords[index];

            return Output;
        }
        #endregion RandomResponse

        #region Occurance
        /// <summary> Checks if the "ShouldOccurFirst" string is found earlier in the message then "ShouldOccurLater" </summary>
        /// <returns>Returns true if "ShouldOccurFirst" has a lower index then "ShouldOccurLater"</returns>
        public bool OccuringFirst(string Message, string ShouldOccurFirst, string ShouldOccurLater)
        {
            bool OccursFirst = false;
            ShouldOccurFirst = ShouldOccurFirst.ToLower();
            ShouldOccurLater = ShouldOccurLater.ToLower();
            int SOF = Message.IndexOf(ShouldOccurFirst);
            int SOL = Message.IndexOf(ShouldOccurLater);

            if (SOF < SOL)
            {
                OccursFirst = true;
            }
            else
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

        /// <summary> Checks if a Message contains both of the strings, if it does then it checks if the "ShouldOccurFirst" string is found earlier in the message then "ShouldOccurLater"</summary>
        /// <returns>Returns true if both of the strings are conta "ShouldOccurFirst" has a lower index then "ShouldOccurLater" </returns>
        public bool ContainsOccuringFirst(string Message, string ShouldOccurFirst, string ShouldOccurLater)
        {
            bool Output = false;
            bool Contain = false;
            //Contains
            if (Contains(Message, ShouldOccurFirst, ShouldOccurLater))
            {
                Contain = true;
            }
            //Occuring First
            if (Contain == true)
            {
                Output = OccuringFirst(Message, ShouldOccurFirst, ShouldOccurLater);
            }

            return Output;
        }
        #endregion Occurance
        /// <summary> Divides the message into seperate sentences </summary>
        /// <returns>Returns a array of strings, each string is a separate sentence</returns>
        public string[] DivideIntoSentences(string Message)
        {
            string[] Output = null;

            return Output;
        }

        /// <summary> Checks how many Keywords there are in the Message. </summary>
        /// <returns>Returns true if there is only one keyword</returns>
        public bool OnlyKeyword()
        {
            bool Output = false;
            if (MH.Keywordcount == 1)
            {
                Output = true;
            }
            else
            {
                Output = false;
            }
            return Output;
        }
    }
}
