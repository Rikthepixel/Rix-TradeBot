using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
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
        string UserName;
        string PassWord;
        string TFAcode;


        //
        // Login
        //

        public void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected succesfully");

            setup.steamUser.LogOn(new SteamUser.LogOnDetails
            {
                //Setup the LoginDetails
                Username = UserName,
                Password = PassWord,
                TwoFactorCode = TFAcode
            });
        }


        //On Logged on function
        public void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            //if failed to loggon
            if (callback.Result != EResult.OK)
            {
                // steamguard protection
                if (callback.Result == EResult.AccountLogonDenied)
                {
                    Console.WriteLine("Account is SteamGuard protected.");
                    setup.isRunning = false;
                    return;
                }

                Console.WriteLine($"Unabble to logon: {callback.Result}, {callback.ExtendedResult}.");
                setup.isRunning = false;
            }

            if (callback.Result == EResult.OK)
            {
                // Blank out the Login variables for security
                UserName = "";
                PassWord = "";
                TFAcode = "";

                Console.WriteLine("Logged on succesfully.");
            }
        }

        public void LoginDetails(Program.LoginType loginType, Program.AuthType AuthentictionType)
        {
            string AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
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
            }
        }

        // 
        // END Login
        //


        //
        // Disconnecting
        //

        //on Disconnect function
        public void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            Console.WriteLine("Disconnected succesfully");

            setup.isRunning = false;
        }
        //on Logg off function
        public void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Console.WriteLine($"Logged off succesfully {callback.Result}");
        }

        //
        // Local Persona
        //

        public void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            EPersonaState PersonaState = setup.steamFriends.GetPersonaState();
            if (PersonaState == EPersonaState.Offline)
            {
                Console.WriteLine("Setting personastate to Online");
                setup.steamFriends.SetPersonaState(EPersonaState.Online);
            }
        }

        //
        // END Disconnect
        //
    }
}
