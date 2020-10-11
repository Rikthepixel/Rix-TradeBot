using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        string AuthCode;

        //
        // Connecting to steam
        //

        public void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Console.WriteLine("Connected succesfully");

            setup.steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = UserName,
                Password = PassWord,
            });
        }
        //on Logg on function
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
                Console.WriteLine("Logged on succesfully.");
            }
        }

        public void LoginDetails(Program.LoginType loginType)
        {
            string AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string LoginDetailsPath = Path.Combine(AppPath, "Login Details.txt");

            BuiltinLogin(loginType);

            OneTimeManualLogin(loginType, LoginDetailsPath);

            ManualLogin(loginType);
        }
        private void BuiltinLogin(Program.LoginType loginType)
        {
            if (loginType == Program.LoginType.BuiltinLoginDetails)
            {
                UserName = Program.Username;
                PassWord = Program.Password;
               
            }
        }
        private void OneTimeManualLogin(Program.LoginType loginType, string LoginDetailsPath)
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
            }
        }
        private void ManualLogin(Program.LoginType loginType)
        {
            if (loginType == Program.LoginType.ManualLoginDetails)
            {
                Console.WriteLine(">> Manual Login <<");
                Console.WriteLine("Please enter Username");
                UserName = Console.ReadLine();
                Console.WriteLine("Please enter Password");
                PassWord = Console.ReadLine();
            }
        }



        //
        // Disconnecting from steam
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
    }
}
