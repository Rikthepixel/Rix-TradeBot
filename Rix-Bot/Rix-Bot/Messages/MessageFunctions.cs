using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rix_Bot.Messages
{
    class MessageFunctions
    {

        //
        // Usefull Functions
        //

        #region Contains
        /// <summary> Checks if the message contains specified string value. If it does it will return true </summary>
        public bool Contains(string Container, string Word)
        {
            Container = Container.ToLower(); //ToLower to make sure that it is case insensitive.
            Word = Word.ToLower();
            bool Output = Container.Contains(Word);
            return Output;
        }

        /// <summary> Checks if the message contains the string values from a specified array. If any of the string values are contained in the message, then it will return true</summary>
        public bool Contains(string Container, string[] keywords)
        {
            bool Output = false;
            foreach (string Keyword in keywords)
            {
                if (Contains(Container, Keyword))
                {
                    Output = true;
                }
            }

            return Output;
        }

        /// <summary> Checks if the message contains both of the asked for keywords. If they are both in the Message it will return true </summary>
        public bool Contains(string Container, string keyword1, string keyword2)

        {
            bool Output = false;
            int Counter = 0;
            if (Contains(Container, keyword1))
            {
                Counter++;
            }
            if (Contains(Container, keyword2))
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
            int index;
            Random random = new Random();
            index = random.Next(0, Keyword.Response.Length);
            string Output = Keyword.Response[index];

            return Output;
        }
        /// <summary> Chooses a Random response out of a specified string array </summary>
        /// <returns>Returns a randomly selected string value</returns>
        public string RandomResponse(string[] Keywords)
        {
            int index;
            Random random = new Random();
            index = random.Next(0, Keywords.Length);
            string Output = Keywords[index];

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
            if (MessageHandling.Keywordcount == 1)
            {
                Output = true;
            }
            else
            {
                Output = false;
            }
            return Output;
        }

        #region Salts
        /// <summary> Method generates a randomly generated number, this number is converted to a string. This string can be used for Keywords of a length that is too short</summary>
        /// <returns> Returns a randomly generated number in the form of a string variable </returns>
        public string GenerateSalt()
        {
            string Salt = null;
            Random random = new Random();

            //Adding spices
            Salt = Convert.ToString(random.Next(0, 4687325));
            Salt += Convert.ToString(random.Next(0, 4885948));
            Salt += Convert.ToString(random.Next(0, 8374393));

            return Salt;
        }

        public string AddSaltMSG(string Message, string[] Salt)
        {
            int count = 0;
            int index;
            bool HadToChange = true;
            bool Ignore = false;
            Message = $"{Message} ";

            //Make a loop that continues if the message had to change
            while (HadToChange)
            {
                //resets the variables
                HadToChange = false;
                index = 0;

                //Make a loop that checks each character
                foreach (char C in Message)
                {
                    //Add 1 to the index number
                    index++;
                    //if the character is a letter then we'll add 1 to the count
                    if (Char.IsLetterOrDigit(C))
                    {
                        count++;
                        if (Char.IsDigit(C))
                        {
                            Ignore = true;
                        }
                        if (Char.IsLetter(C))
                        {
                            Ignore = false;
                        }
                    }

                    //If there is a space
                    if (Char.IsWhiteSpace(C))
                    {
                        //if the single character string is not a digit we set Ignore to false.
                        if (!Ignore)
                        {
                            switch (count)
                            {
                                case 1:
                                    Message = Message.Insert(index - 1, Salt[0]);
                                    HadToChange = true;
                                    break;
                                case 2:
                                    Message = Message.Insert(index - 1, Salt[1]);
                                    HadToChange = true;
                                    break;
                                case 3:
                                    Message = Message.Insert(index - 1, Salt[2]);
                                    HadToChange = true;
                                    break;
                            }
                            if (HadToChange)
                            {
                                break;
                            }
                        }
                        //if there has only been one to three letters

                        //reset the count
                        count = 0;
                    }
                }
            }
            return Message;
        }
        public string AddSalt(string Keyword, string[] Salt)
        {
            int count = 0; 

            foreach (char c in Keyword)
            {
            count++;
            }

            switch (count)
            {
                case 1:
                    Keyword = $"{Keyword}{Salt[0]}";
                    break;
                case 2:
                    Keyword = $"{Keyword}{Salt[1]}";
                    break;
                case 3:
                    Keyword = $"{Keyword}{Salt[2]}";
                    break;
            }
            return Keyword;
        }
        #endregion Salts
    }
}
