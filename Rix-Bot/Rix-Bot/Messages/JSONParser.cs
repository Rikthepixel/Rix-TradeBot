using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2.GC.Artifact.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rix_Bot.Messages
{
    class JSONParser
    {
        MessageFunctions MF = new MessageFunctions();

        #region General

        public void CreateFile(string FilePath)
        {
            using FileStream fs = File.Create(FilePath);
            fs.Close();
        }

        public void WriteToFile(string FilePath, string Text)
        {
            using StreamWriter writer = new StreamWriter(FilePath);
            writer.WriteAsync(Text);
            writer.Close();
        }

        public bool FileExists(string FilePath)
        {
            bool Output = false;

            if (File.Exists(FilePath))
            {
                Output = true;
            }

            return Output;
        }

        public bool WriteToNewFile(string FilePath, string Text)
        {
            bool Ouput;
            if (FileExists(FilePath))
            {
                Ouput = true;
            }
            else
            {
                CreateFile(FilePath);
                WriteToFile(FilePath, Text);

                Ouput = true;
            }

            return Ouput;
        }

        public string GetExecutableLocation()
        {
            string Output = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Output;
        }

        public string CombinePath(string PrimairyPath, string SecondairyPath)
        {
            string Output = Path.Combine(PrimairyPath, SecondairyPath);
            return Output;
        }
        #endregion

        #region JSONFunctions
        public string ReadJSONData(string Path)
        {
            string Data = string.Empty;

            using (StreamReader r = new StreamReader(Path))
            {
                Data = r.ReadToEnd();
                r.Close();
            }

            return Data;
        }
        public bool JSONContainsKeyMessages(string Message, string JSONdata, string Botname, MessageHandling MH)
        {
            bool Output = false;
            bool temp = false;
            
            var Data = JsonConvert.DeserializeObject<dynamic>(JSONdata);

            //Main recognition loop
            for (int i = 0; i < Data.Count; i++)
            {
                //Secondairy recognition loop
                for (int j = 0; j < Data[i].Message.Count; j++)
                {
                    #region Flexible "Contains" recognition
                    string TEMPstring = Data[i].Message[j];

                    #region JSON template
                    //Replace JSON message Template

                    TEMPstring = MF.ContainsReplace(TEMPstring, "BOTNAME", Botname);


                    #endregion JSON template

                    #region Message
                    //Replace Items in Message
                    Message = MF.ContainsReplace(Message, MH.Refer.Iam.KeyWords, "IAMFLEX");
                    Message = MF.ContainsReplace(Message, MH.Refer.YouAre.KeyWords, "YOUAREFLEX");
                    Message = MF.ContainsReplace(Message, MH.Refer.AreYou.KeyWords, "AREYOUFLEX");
                    Message = MF.ContainsReplace(Message, MH.Greet.KeyWords, "USERGREETSBOT");
                    #endregion Message

                    #endregion Flexible "Contains" recognition
                    temp = MF.Contains(Message, TEMPstring);
                    if(temp == true)
                    {
                        Output = true;
                    }
                }
            }
            return Output;
        }
        #endregion

        #region Response

        public string GetResponse(string Message, string FilePath, string Botname, string SenderName, MessageHandling MH)
        {
            string Output = string.Empty;

            bool ResponseChosen = false;


            var JSONdata = ReadJSONData(FilePath);
            var Data = JsonConvert.DeserializeObject<dynamic>(JSONdata);

            //Main recognition loop
            for (int i = 0; i < Data.Count; i++)
            {
                //Secondairy recognition loop
                for (int j = 0; j < Data[i].Message.Count; j++)
                {
                    #region Flexible "Contains" recognition
                    string TEMPstring = Data[i].Message[j];
                    
                    #region JSON template
                    //Replace JSON message Template

                    TEMPstring = MF.ContainsReplace(TEMPstring, "Botname", Botname);

                    #endregion JSON template

                    #region Message
                    //Replace Items in Message

                    Message = MF.ContainsReplace(Message, MH.Refer.Iam.KeyWords, "IAMFLEX");
                    Message = MF.ContainsReplace(Message, MH.Refer.YouAre.KeyWords, "YOUAREFLEX");
                    Message = MF.ContainsReplace(Message, MH.Refer.AreYou.KeyWords, "AREYOUFLEX");
                    Message = MF.ContainsReplace(Message, MH.Greet.KeyWords, "USERGREETSBOT");

                    #endregion Message

                    #endregion Flexible "Contains" recognition

                    //If the message contains the TEMPstring and the response is not Chosen yet
                    if (MF.Contains(Message, TEMPstring) && !ResponseChosen)
                    {
                        //Create a Temporairy Array Because you cant feed Data[i].Response into an array directly
                        string[] TEMPRespArray = new string[Data[i].Response.Count];
                        for (int R = 0; R < Data[i].Response.Count; R++)
                        {
                            TEMPRespArray[R] = Data[i].Response[R];
                        }

                        //Grab a response out of the TempRespArray
                        string resp = MF.RandomResponse(TEMPRespArray);
                        //Replace JSON response Template
                        resp = MF.ContainsReplace(resp, "USER", SenderName);
                        resp = MF.ContainsReplace(resp, "RANDOMGREET", MF.RandomResponse(MH.Greet.Response));
                        
                        //RandomAdditions
                        if (MF.Contains(resp, "RANDOMADD"))
                        {
                            //Try and Add a RandomAddition to the Response
                            try
                            {
                                string[] TEMPRandArray = new string[Data[i].RandomAdd.Count];
                                for (int T = 0; T < Data[i].RandomAdd.Count; T++)
                                {
                                    TEMPRandArray[T] = Data[i].RandomAdd[T];
                                }
                                //Generate a random number
                                Random random = new Random();
                                int Index = random.Next(0, 100);

                                if (Index >= 80)
                                {
                                    resp = MF.ContainsReplace(resp, "RandomAdd", MF.RandomResponse(TEMPRandArray));
                                }
                                else
                                {
                                    resp = MF.ContainsReplace(resp, "RandomAdd", "");
                                }
                            }
                            catch (Exception)
                            {
                                //Makes sure that if the "MessResp" doesnt have a RandomAdd array
                                //That it wont crash

                                //Replace the RandomAdd so that the User doesnt see the RandomAdd
                                resp = MF.ContainsReplace(resp, "RandomAdd", "");
                            }
                        }

                        Output = resp;

                        //Reset Chosen response to false
                        ResponseChosen = true;
                    }
                }
            }

            return Output;
        }

        #endregion
    }
}
