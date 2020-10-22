using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public bool JSONContainsKeyMessages(string Message, string JSONdata, string Botname)
        {
            bool Output = false;
            bool temp = false;
            var Data = JsonConvert.DeserializeObject<dynamic>(JSONdata);
            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = 0; j < Data[i].Message.Count; j++)
                {
                    string TEMPstring = Data[i].Message[j];
                    if (TEMPstring.Contains("Botname"))
                    {
                        TEMPstring = TEMPstring.Replace("Botname", Botname);
                    }
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

        public string GetResponse(string Message, string FilePath, string Botname, string SenderName, KeyTypes Greeting)
        {
            string Output = string.Empty;
            bool ResponseChosen = false;
            var JSONdata = ReadJSONData(FilePath);
            var Data = JsonConvert.DeserializeObject<dynamic>(JSONdata);
            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = 0; j < Data[i].Message.Count; j++)
                {
                    string TEMPstring = Data[i].Message[j];
                    if (TEMPstring.Contains("Botname"))
                    {
                        TEMPstring = TEMPstring.Replace("Botname", Botname);
                    }
                    if (MF.Contains(Message, TEMPstring) && !ResponseChosen)
                    {
                        //Create a Temporairy Array Because you cant feed Data[i].Response into an array directly
                        string[] TEMPArray = new string[Data[i].Response.Count];
                        for (int R = 0; R < Data[i].Response.Count; R++)
                        {
                            TEMPArray[R] = Data[i].Response[R];
                        }


                        string resp = MF.RandomResponse(TEMPArray);

                        if (resp.Contains("User"))
                        {
                            resp = resp.Replace("User", SenderName);
                        }

                        if (MF.Contains(resp, "RandomGreet")){
                            resp = resp.Replace("RandomGreet", MF.RandomResponse(Greeting.Response));
                        }

                        if (MF.Contains(resp, "RandomAdd"))
                        {
                            string[] TEMPRandArray = new string[Data[i].RandomAdd.Count];
                            for (int T = 0; T < Data[i].RandomAdd.Count; T++)
                            {
                                TEMPRandArray[T] = Data[i].RandomAdd[T];
                            }
                            Random random = new Random();
                            int Index = random.Next(0, 100);
                            if (Index >= 80)
                            {
                                resp = resp.Replace("RandomAdd", MF.RandomResponse(TEMPRandArray));
                            } else
                            {
                                resp = resp.Replace("RandomAdd", "");
                            }
                        }

                        Output = resp;

                        ResponseChosen = true;
                    }
                }
            }

            return Output;
        }

        #endregion
    }
}
