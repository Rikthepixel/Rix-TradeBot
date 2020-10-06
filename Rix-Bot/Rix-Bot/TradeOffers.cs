using SteamKit2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rix_Bot
{
    class TradeOffers
    {
        private readonly Setup setup;
        public TradeOffers(Setup setup)
        {
            this.setup = setup;
        }

        public void OnRecieveTrade(SteamTrading.TradeProposedCallback callback)
        {
            //empty
        }

        public void OnTradeResult(SteamTrading.TradeResultCallback callback)
        {
            //empty
        }
    }
}
