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
using NUnit.Framework;
using QuantConnect.Brokerages.Oanda;
using QuantConnect.Configuration;
using QuantConnect.Lean.Engine.DataFeeds;
using Environment = QuantConnect.Brokerages.Oanda.Environment;

namespace QuantConnect.Tests.Brokerages.Oanda
{
    [TestFixture]
    public class OandaBrokerageAdditionalTests
    {
        [Test]
        [Explicit("This test will only pass if the Practice account at least one open order all of them are of a supported type")]
        public void GetsOpenOrders()
        {
            var brokerage = CreateBrokerage();
            var openOrders = brokerage.GetOpenOrders();
            Assert.That(openOrders, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Explicit("This test will only pass if the Practice account has an open order of an unsupported type, like STOP_LOSS")]
        public void ThrowsOnUnsupportedOpenOrders()
        {
            var brokerage = CreateBrokerage();
            Assert.Throws<NotSupportedException>(() => brokerage.GetOpenOrders());
        }

        private OandaBrokerage CreateBrokerage()
        {
            var environment = Config.Get("oanda-environment").ConvertTo<Environment>();
            var accessToken = Config.Get("oanda-access-token");
            var accountId = Config.Get("oanda-account-id");
            var aggregator = new AggregationManager();

            var brokerage =new OandaBrokerage(new OrderProvider(), new SecurityProvider(), aggregator, environment, accessToken, accountId);

            brokerage.Connect();
            Assert.IsTrue(brokerage.IsConnected);

            return brokerage;
        }
    }
}
