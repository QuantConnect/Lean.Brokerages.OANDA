/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Linq;
using NodaTime;
using NUnit.Framework;
using QuantConnect.Brokerages.Oanda;
using QuantConnect.Configuration;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Logging;
using QuantConnect.Securities;
using Environment = QuantConnect.Brokerages.Oanda.Environment;

namespace QuantConnect.Tests.Brokerages.Oanda
{
    [TestFixture]
    public class OandaBrokerageHistoryProviderTests
    {
        private static TestCaseData[] TestParameters
        {
            get
            {
                TestGlobals.Initialize();
                var eurusd = Symbol.Create("EURUSD", SecurityType.Forex, Market.Oanda);

                return new[]
                {
                    // valid parameters
                    new TestCaseData(eurusd, Resolution.Second, Time.OneMinute, TickType.Quote, false),
                    new TestCaseData(eurusd, Resolution.Minute, Time.OneHour, TickType.Quote, false),
                    new TestCaseData(eurusd, Resolution.Hour, Time.OneDay, TickType.Quote, false),
                    new TestCaseData(eurusd, Resolution.Daily, TimeSpan.FromDays(15), TickType.Quote, false),

                    // invalid resolution, null result
                    new TestCaseData(eurusd, Resolution.Tick, TimeSpan.FromSeconds(15), TickType.Quote, true),

                    // invalid period, null result
                    new TestCaseData(eurusd, Resolution.Daily, TimeSpan.FromDays(-15), TickType.Quote, true),

                    // invalid symbol, null result
                    new TestCaseData(Symbol.Create("XYZ", SecurityType.Forex, Market.FXCM), Resolution.Daily, TimeSpan.FromDays(15), TickType.Quote, true),

                    // invalid security type, null result
                    new TestCaseData(Symbols.AAPL, Resolution.Daily, TimeSpan.FromDays(15), TickType.Quote, true),

                    // invalid market, null result
                    new TestCaseData(Symbol.Create("EURUSD", SecurityType.Forex, Market.USA), Resolution.Daily, TimeSpan.FromDays(15), TickType.Quote, true),

                    // invalid tick type, null result
                    new TestCaseData(eurusd, Resolution.Daily, TimeSpan.FromDays(15), TickType.Trade, true),
                    new TestCaseData(eurusd, Resolution.Daily, TimeSpan.FromDays(15), TickType.OpenInterest, true),
                };
            }
        }

        [Test, TestCaseSource(nameof(TestParameters))]
        public void GetsHistory(Symbol symbol, Resolution resolution, TimeSpan period, TickType tickType, bool unsupported)
        {
            var environment = Config.Get("oanda-environment").ConvertTo<Environment>();
            var accessToken = Config.Get("oanda-access-token");
            var accountId = Config.Get("oanda-account-id");

            var brokerage = new OandaBrokerage(null, null, null, environment, accessToken, accountId);

            var now = DateTime.UtcNow;
            var request = new HistoryRequest(now.Add(-period),
                now,
                tickType == TickType.Quote ? typeof(QuoteBar) : typeof(TradeBar),
                symbol,
                resolution,
                SecurityExchangeHours.AlwaysOpen(TimeZones.EasternStandard),
                DateTimeZone.Utc,
                Resolution.Minute,
                false,
                false,
                DataNormalizationMode.Adjusted,
                tickType);

            var history = brokerage.GetHistory(request)?.ToList();

            if (unsupported)
            {
                Assert.IsNull(history);
                return;
            }

            Assert.IsNotNull(history);

            foreach (var bar in history.Cast<QuoteBar>())
            {
                Log.Trace("{0}: {1} - O={2}, H={3}, L={4}, C={5}", bar.Time, bar.Symbol, bar.Open, bar.High, bar.Low, bar.Close);
            }

            Log.Trace("Data points retrieved: " + history.Count);
        }
    }
}