using SteamKit2;
using SteamKit2.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Rix_Bot
{
    class UserActivities
    {
        private readonly Setup setup;
        public UserActivities(Setup setup)
        {
            this.setup = setup;
        }
        //Login details
        string UserName;
        string PassWord;
        string TFAcode, authCode;

        //Reconnection
        bool ReconnectAttempt;
        int ReconnectAttemptCount = 0;


        bool TFAConnect = false;
        bool FirstLogon = true;
        //Application path
        string AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        //
        // Login
        //

        //Connecting
        public void OnConnected(SteamClient.ConnectedCallback callback)
        {
            byte[] sentryHash = null;
            string SentryPath = Path.Combine(AppPath, "sentry.bin");
            string LoginKey;
            bool ShouldRememberPassword = false;

            //Automatically enable the ShouldRememberPassword because if you dont do that the program will crash
            if (Program.AuthenticationType == Program.AuthType.AutomaticAuthcode)
            {
                ShouldRememberPassword = true;
            }

            using (StreamReader reader = File.OpenText("LoginKey.txt"))
            {
                LoginKey = reader.ReadLine();
            }
            Console.WriteLine(File.Exists("sentry.bin"));

            if (File.Exists("sentry.bin"))
            {
                // if we have a saved sentry file, read and sha-1 hash it
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }

            if (Program.AuthenticationType == Program.AuthType.AutomaticAuthcode)
            {
                if (FirstLogon)
                {
                    TFAConnect = true;
                } 
                if (!FirstLogon) //Dont Show the Succesfull connection if it is a first Two factor authentication login
                {
                    Console.WriteLine("Connected succesfully");
                }
            }

            if (Program.AuthenticationType != Program.AuthType.AutomaticAuthcode)
            {
                Console.WriteLine("Connected succesfully");
            }

            //Call the Login Details Function to gather the correct login Details
            LoginDetails(Program.LoginDetailType, Program.AuthenticationType);

            if (ShouldRememberPassword == true)
            {
                setup.steamUser.LogOn(new SteamUser.LogOnDetails
                {
                    //Setup the LoginDetails
                    Username = UserName,

                    //Set the TwoFactorCode to the User entered value
                    TwoFactorCode = TFAcode,

                    //Set Authcode supplied by the user
                    AuthCode = authCode,

                    //LoginKey, this is for 1 time entering of the SMA code
                    ShouldRememberPassword = ShouldRememberPassword,

                    LoginKey = LoginKey,


                    //This SentryHash is proof that the current user owns the account
                    SentryFileHash = sentryHash,
                });
            } 

            if (ShouldRememberPassword == false)
            {
                setup.steamUser.LogOn(new SteamUser.LogOnDetails
                {
                    //Setup the LoginDetails
                    Username = UserName,
                    Password = PassWord,

                    //Set the TwoFactorCode to the User entered value
                    TwoFactorCode = TFAcode,

                    //Set Authcode supplied by the user
                    AuthCode = authCode,

                    //This SentryHash is proof that the current user owns the account
                    SentryFileHash = sentryHash,
                });
            }
            
            LoginKey = "";
           
            if (ReconnectAttempt)
            {
                ReconnectAttemptCount++;
            }
        }
        
        //On Logged on function
        public void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            bool isSteamGuard = callback.Result == EResult.AccountLogonDenied;
            bool is2FA = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

            Console.WriteLine(callback.GetHashCode());
            Console.WriteLine(callback.ExtendedResult);
            // steamguard protection
            if (isSteamGuard || is2FA)
            {
                if (is2FA)
                {
                    if (Program.AuthenticationType == Program.AuthType.AutomaticAuthcode)
                    {
                        AutomaticTFA(Program.AuthenticationType);
                    }
                    else
                    {
                        Console.Write("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain);
                        authCode = Console.ReadLine();
                    }
                    return;
                }
            }

            //if failed to loggon
            if (callback.Result != EResult.OK)
            {
                string Result = LoginCodeMSG(callback.Result);

                Console.WriteLine($"Unable to logon: {Result}, {callback.ExtendedResult}.");
                setup.isRunning = false;
            }

            if (callback.Result == EResult.OK)
            {
                // Blank out the Login variables for security

                UserName = "";
                PassWord = "";
                TFAcode = "";

                FirstLogon = false;

                if (TFAConnect)
                {
                    TFAConnect = false;
                }

                Console.WriteLine("Logged on succesfully.");
                Console.WriteLine("______________________"); //Blank line for better looks
                Console.WriteLine("");
            }
        }

        // Login Denied Response handler
        private string LoginCodeMSG(EResult result) 
        {
            EResult Code = result;
            string Response = "It is so empty in here";
            switch (Code)
            {
                case EResult.Invalid:
                    Response = "Login was Invalid, due to an unknown Reason";
                    break;
                case EResult.OK:
                    Response = "Login succesful";
                    break;
                case EResult.Fail:
                    Response = "Login Failed, due to an unknown reasons";
                    break;
                case EResult.NoConnection:
                    Response = "No connection was able to be made to the Steam servers";
                    break;
                case EResult.InvalidPassword:
                    Response = "You have entered the wrong Password";
                    break;
                case EResult.InvalidName:
                    Response = "You have entered the wrong Username";
                    break;
                case EResult.AccessDenied:
                    Response = "Access to your account has been denied";
                    break;
                case EResult.LimitExceeded:
                    Response = "Too many login attempts on the same account. Wait about an hour and try again";
                    break;
                case EResult.ConnectFailed:
                    Response = "Connection to the Steam servers failed";
                    break;
                case EResult.AccountDisabled:
                    Response = "This Steam account is Disabled";
                    break;
                case EResult.PasswordUnset:
                    Response = "Please enter a password next time, restart the program to try again";
                    break;
                case EResult.IllegalPassword:
                    Response = "The Password you have entered is not allowed";
                    break;
                case EResult.AccountLogonDenied:
                    Response = "Your account logon was denied";
                    break;
                case EResult.CannotUseOldPassword:
                    Response = "You have entered an Old passowrd";
                    break;
                case EResult.InvalidLoginAuthCode:
                    Response = "Your Authentication code was invalid";
                    break;
                case EResult.BadResponse:
                    break;
                case EResult.RequirePasswordReEntry:
                    Response = "Please Re-Enter your password";
                    break;
                case EResult.RateLimitExceeded:
                    Response = "There are too many login attempts for this account, wait an about an hour and then try again";
                    break;
                case EResult.AccountLoginDeniedNeedTwoFactor:
                    Response = "The login attempt was denied, because no (Valid) Two Factor Authentication code was entered";
                    break;
                case EResult.TwoFactorCodeMismatch:
                    Response = "The login attempt was denied, because the Two Factor Authentication code did not match up with the SMA code";
                    break;
                case EResult.TwoFactorActivationCodeMismatch:
                    Response = "The login attempt was denied, because the Two Factor Authentication code did not match up with the SMA code";
                    break;
            }

            return Response;
        }

        //
        // Login Details
        //
        public void LoginDetails(Program.LoginType loginType, Program.AuthType AuthentictionType)
        {
            string LoginDetailsPath = Path.Combine(AppPath, "Login Details.txt");

            BuiltinLogin(loginType, AuthentictionType);

            OneTimeManualLogin(loginType, LoginDetailsPath, AuthentictionType);

            ManualLogin(loginType, AuthentictionType);

        }

        // Login types
        private void BuiltinLogin(Program.LoginType loginType, Program.AuthType AuthentictionType)
        {
            if (loginType == Program.LoginType.BuiltinLoginDetails)
            {
                UserName = Program.Username;
                PassWord = Program.Password;

                ManualTFA(AuthentictionType);
            }
        }
        private void OneTimeManualLogin(Program.LoginType loginType, string LoginDetailsPath, Program.AuthType AuthentictionType)
        {

            if (loginType == Program.LoginType.OneTimeManualDetails)
            {
                String[] details;

                Console.WriteLine(">> One-Time Manual login <<");
                Console.WriteLine("Please enter Username");
                UserName = Console.ReadLine();
                Console.WriteLine("Please enter Password");
                PassWord = Console.ReadLine();

                try
                {
                    File.Decrypt(LoginDetailsPath);
                }
                catch
                {
                    Console.WriteLine("Login Details file was not Encrypted");
                }
                details = File.ReadAllLines(LoginDetailsPath);
                File.Encrypt(LoginDetailsPath);

                ManualTFA(AuthentictionType);

            }
        }
        private void ManualLogin(Program.LoginType loginType, Program.AuthType AuthentictionType)
        {

            if (loginType == Program.LoginType.ManualLoginDetails)
            {
                Console.WriteLine(">> Manual Login <<");
                Console.WriteLine("Please enter Username");
                UserName = Console.ReadLine();
                Console.WriteLine("Please enter Password");
                PassWord = Console.ReadLine();

                //Just creates a blank line for a cleaner program look
                Console.WriteLine("");

                ManualTFA(AuthentictionType);
            }
        }

        // Authentication Types
        private void ManualTFA(Program.AuthType AuthentictionType)
        {
            if (AuthentictionType == Program.AuthType.ManualAuthcode)
            {
                Console.WriteLine(">> Manual Authentication <<");
                Console.WriteLine("Please enter Steam Guard code");
                TFAcode = Console.ReadLine().ToUpper();
                Console.WriteLine("");
            }
        }
        private void AutomaticTFA(Program.AuthType AuthentictionType)
        {
            if (Program.AuthenticationType == Program.AuthType.AutomaticAuthcode)
            {
                Console.WriteLine(">> Automatic Authentication <<");
                Console.WriteLine("Please enter Steam Guard code");
                TFAcode = Console.ReadLine().ToUpper();
                Console.WriteLine("");
                FirstLogon = false;
            }
        }

        public void MachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            {
                Console.WriteLine("Updating sentryfile...");

                // write out our sentry file
                // ideally we'd want to write to the filename specified in the callback
                // but then this sample would require more code to find the correct sentry file to read during logon
                // for the sake of simplicity, we'll just use "sentry.bin"

                int fileSize;
                byte[] sentryHash;
                using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Seek(callback.Offset, SeekOrigin.Begin);
                    fs.Write(callback.Data, 0, callback.BytesToWrite);
                    fileSize = (int)fs.Length;

                    fs.Seek(0, SeekOrigin.Begin);
                    using (var sha = SHA1.Create())
                    {
                        sentryHash = sha.ComputeHash(fs);
                    }
                }
                Console.WriteLine(callback.OneTimePassword);
                // inform the steam servers that we're accepting this sentry file
                setup.steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
                {
                    JobID = callback.JobID,

                    FileName = callback.FileName,

                    BytesWritten = callback.BytesToWrite,
                    FileSize = fileSize,
                    Offset = callback.Offset,

                    Result = EResult.OK,
                    LastError = 0,

                    OneTimePassword = callback.OneTimePassword,

                    SentryFileHash = sentryHash,
                });

                Console.WriteLine("Done!");
            }
        }

        public void OnLoginKey(SteamUser.LoginKeyCallback callback)
        {
            string loginKey = callback.LoginKey;
            Console.WriteLine(loginKey);

            if (File.Exists("LoginKey.txt"))
            {
                using (StreamWriter writer = File.CreateText("LoginKey.txt"))
                {
                    writer.WriteLineAsync(loginKey);
                    writer.Close();
                }
            }

            if (!File.Exists("LoginKey.txt"))
            {
                using (StreamWriter writer = File.CreateText("LoginKey.txt"))
                {
                    writer.WriteLineAsync(loginKey);
                    writer.Close();
                }
            }

            setup.steamUser.AcceptNewLoginKey(callback);
        }

        //
        // Disconnecting
        //

        //on Disconnect function
        public void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            if (TFAConnect == false)
            {
                Console.WriteLine("Bot was Disconnected from Steam");
            }

            if (Program.ReconnectAfterDisconnect == false)
            {
                setup.isRunning = false;
            } else if (Program.ReconnectAfterDisconnect == true)
            {
                ReconnectAttempt = true;
                Reconnecting();
            }
        }

        //Reconnection function
        private void Reconnecting()
        {
            if (TFAConnect)
            {
                setup.Client.Connect();
                ReconnectAttemptCount -= 1;
                return;
            }
            //If the Reconnection counter exceeds the Allowed reconnection attempts then give up
            if (ReconnectAttemptCount >= (Program.ReconnectionAttempts + 1))
            {
                Console.WriteLine("Failed to Reconnect to Steam");
                setup.isRunning = false;
                return;
            }

            if (ReconnectAttemptCount != (Program.ReconnectionAttempts + 1))
            {
                string Counter = "Hello";
                switch (ReconnectAttemptCount)
                {
                    case 1:
                        Counter = "2nd";
                        break;
                    case 2:
                        Counter = "3rd";
                        break;
                    case 3:
                        Counter = "4th";
                        break;

                    default:
                        bool chosen = false;
                        if (ReconnectAttemptCount == 21)
                        {
                            Counter = $"21st";
                            chosen = true;
                        }
                        if (ReconnectAttemptCount == 22)
                        {
                            Counter = $"22nd";
                            chosen = true;
                        }
                        if (ReconnectAttemptCount == 23)
                        {
                            Counter = $"23rd";
                            chosen = true;
                        }
                        if (ReconnectAttemptCount > 33 && chosen == false)
                        {
                            if (ReconnectAttemptCount.ToString().Contains('1') && chosen == false)
                            {
                                Counter = $"{ReconnectAttemptCount}st";
                                chosen = true;
                            }
                            if (ReconnectAttemptCount.ToString().Contains('2') && chosen == false)
                            {
                                Counter = $"{ReconnectAttemptCount}nd";
                                chosen = true;
                            }
                            if (ReconnectAttemptCount.ToString().Contains('3') && chosen == false)
                            {
                                Counter = $"{ReconnectAttemptCount}rd";
                                chosen = true;
                            }
                        }
                        if (ReconnectAttemptCount > 3 && chosen == false)
                        {
                            Counter = $"{ReconnectAttemptCount}th";

                        }
                        break;
                } //counting grammar :P

                if (ReconnectAttemptCount == 0)
                {
                    Console.WriteLine($"Attempting to reconnect in 5 seconds");
                } else
                {
                    Console.WriteLine($"{Counter} Attempt to reconnect in 5 seconds");
                    Console.WriteLine($"There are: {Program.ReconnectionAttempts - ReconnectAttemptCount} Reconnection attempts remaining");
                }

                for (int i = 5; i > 0; i--) //Count down to 0
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Console.WriteLine($">> {i} seconds");
                }

                Console.WriteLine("Attempting reconnection to Steam");
                Console.WriteLine(""); //Blank line for cosmetic purposes

                setup.Client.Connect();
            }
        }

        //on Logg off function
        public void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine($"Logged off succesfully {callback.Result}");
        }

        //
        // Local Persona
        //

        // Set the Bot it's state
        public void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            EPersonaState PersonaState = setup.steamFriends.GetPersonaState();
            if (PersonaState == EPersonaState.Offline)
            {
                Console.WriteLine("Setting personastate to Online");
                setup.steamFriends.SetPersonaState(EPersonaState.Online);
                Console.WriteLine("______________________________"); //Blank line for cosmetic purposes
                Console.WriteLine("");
            }
        }
    }
}
