﻿using SteamKit2;
using SteamKit2.Internal;
using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;

namespace Rix_Bot
{
    public class Setup
    {

        //
        // Startup
        //

        private void StartupText()
        {
            //Write Bot version
            Console.Title = "RIX-v0.1";
            Console.WriteLine("RIX-v0.1");
            //Write exit instructions
            Console.WriteLine("Ctrl + C to exit");
        }

        //
        // Callbacks
        //

        public bool isRunning;
        public SteamFriends steamFriends;
        public SteamClient Client;
        public SteamUser steamUser;
        public SteamTrading steamTrading;
        

        private Friends Friend;
        private UserActivities UserActs;
        private MessageHandling MsgHandler;
        private TradeOffers Trade;
        private Program program;

        public Setup()
        {
            StartupText();
            
            UserActs = new UserActivities(this);
            Friend = new Friends(this);
            MsgHandler = new MessageHandling(this);
            Trade = new TradeOffers(this);
            program = new Program(this);

            this.isRunning = false;
        }
        
        public void SetupListeners()
        {

            Client = new SteamClient();
            CallbackManager callbackManager = new CallbackManager(Client);
            steamUser = Client.GetHandler<SteamUser>();
            steamFriends = Client.GetHandler<SteamFriends>();
            steamTrading = Client.GetHandler<SteamTrading>();
           
            //
            //Callbacks
            //

            //Steam user activities
            callbackManager.Subscribe<SteamClient.ConnectedCallback>(UserActs.OnConnected);
            callbackManager.Subscribe<SteamClient.DisconnectedCallback>(UserActs.OnDisconnected);
            callbackManager.Subscribe<SteamUser.LoggedOnCallback>(UserActs.OnLoggedOn);
            callbackManager.Subscribe<SteamUser.LoggedOffCallback>(UserActs.OnLoggedOff);
            callbackManager.Subscribe<SteamUser.AccountInfoCallback>(UserActs.OnAccountInfo);

            callbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(UserActs.MachineAuth);
            callbackManager.Subscribe<SteamUser.LoginKeyCallback>(UserActs.OnLoginKey);

            //Steam friend activities
            callbackManager.Subscribe<SteamFriends.FriendsListCallback>(Friend.OnFriendsList);
            callbackManager.Subscribe<SteamFriends.PersonaStateCallback>(Friend.OnPersonaState);
            callbackManager.Subscribe<SteamFriends.FriendAddedCallback>(Friend.OnFriendAdded);
            callbackManager.Subscribe<SteamFriends.ProfileInfoCallback>(Friend.OnProfileInfo);

            //Steam Messages
            callbackManager.Subscribe<SteamFriends.FriendMsgCallback>(MsgHandler.OnMessageRecieved);

            //Trade
            callbackManager.Subscribe<SteamTrading.TradeProposedCallback>(Trade.OnRecieveTrade);
            callbackManager.Subscribe<SteamTrading.TradeResultCallback>(Trade.OnTradeResult);
            Client.Connect();

            isRunning = true;
            while (isRunning)
            {
                callbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }
    }
}
