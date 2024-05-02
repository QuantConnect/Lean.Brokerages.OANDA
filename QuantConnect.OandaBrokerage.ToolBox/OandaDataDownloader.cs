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
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Configuration;
using QuantConnect.Brokerages.Oanda;
using QuantConnect.Util;
using Environment = QuantConnect.Brokerages.Oanda.Environment;

namespace QuantConnect.ToolBox.OandaDownloader
{
    /// <summary>
    /// Oanda Data Downloader class
    /// </summary>
    public class OandaDataDownloader : IDataDownloader
    {
        private readonly OandaBrokerage _brokerage;
        private readonly OandaSymbolMapper _symbolMapper = new OandaSymbolMapper();

        public OandaDataDownloader() : this(Config.Get("oanda-access-token"), Config.Get("oanda-account-id"))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OandaDataDownloader"/> class
        /// </summary>
        public OandaDataDownloader(string accessToken, string accountId)
        {
            // Set Oanda account credentials
            _brokerage = new OandaBrokerage(null,
                null,
                null,
                Environment.Practice,
                accessToken,
                accountId);
        }

        /// <summary>
        /// Checks if downloader can get the data for the Lean symbol
        /// </summary>
        /// <param name="symbol">The Lean symbol</param>
        /// <returns>Returns true if the symbol is available</returns>
        public bool HasSymbol(string symbol)
        {
            return _symbolMapper.IsKnownLeanSymbol(Symbol.Create(symbol, GetSecurityType(symbol), Market.Oanda));
        }

        /// <summary>
        /// Gets the security type for the specified Lean symbol
        /// </summary>
        /// <param name="symbol">The Lean symbol</param>
        /// <returns>The security type</returns>
        public SecurityType GetSecurityType(string symbol)
        {
            return _symbolMapper.GetLeanSecurityType(symbol);
        }

        /// <summary>
        /// Get historical data enumerable for a single symbol, type and resolution given this start and end time (in UTC).
        /// </summary>
        /// <param name="parameters">model class for passing in parameters for historical data</param>
        /// <returns>Enumerable of base data for this symbol</returns>
        public IEnumerable<BaseData> Get(DataDownloaderGetParameters parameters)
        {
            if (!_brokerage.IsValidHistoryRequest(parameters.Symbol, parameters.StartUtc, parameters.EndUtc, parameters.Resolution, parameters.TickType))
            {
                return null;
            }

            var symbol = parameters.Symbol;
            var resolution = parameters.Resolution;
            var startUtc = parameters.StartUtc;
            var endUtc = parameters.EndUtc;

            var barsTotalInPeriod = new List<QuoteBar>();
            var barsToSave = new List<QuoteBar>();

            // set the starting date/time
            var date = startUtc;
            var startDateTime = date;

            // loop until last date
            while (startDateTime <= endUtc.AddDays(1))
            {
                // request blocks of 5-second bars with a starting date/time
                var bars = _brokerage.DownloadQuoteBars(symbol, startDateTime, endUtc.AddDays(1), Resolution.Second, DateTimeZone.Utc).ToList();
                if (bars.Count == 0)
                    break;

                var groupedBars = GroupBarsByDate(bars);

                if (groupedBars.Count > 1)
                {
                    // we received more than one day, so we save the completed days and continue
                    while (groupedBars.Count > 1)
                    {
                        var currentDate = groupedBars.Keys.First();
                        if (currentDate > endUtc)
                            break;

                        barsToSave.AddRange(groupedBars[currentDate]);

                        barsTotalInPeriod.AddRange(barsToSave);

                        barsToSave.Clear();

                        // remove the completed date
                        groupedBars.Remove(currentDate);
                    }

                    // update the current date
                    date = groupedBars.Keys.First();

                    if (date <= endUtc)
                    {
                        barsToSave.AddRange(groupedBars[date]);
                    }
                }
                else
                {
                    var currentDate = groupedBars.Keys.First();
                    if (currentDate > endUtc)
                        break;

                    // update the current date
                    date = currentDate;

                    barsToSave.AddRange(groupedBars[date]);
                }

                // calculate the next request datetime (next 5-sec bar time)
                startDateTime = bars[bars.Count - 1].Time.AddSeconds(5);
            }

            if (barsToSave.Count > 0)
            {
                barsTotalInPeriod.AddRange(barsToSave);
            }

            return LeanData.AggregateQuoteBars(barsTotalInPeriod, symbol, resolution.ToTimeSpan());
        }

        /// <summary>
        /// Groups a list of bars into a dictionary keyed by date
        /// </summary>
        /// <param name="bars"></param>
        /// <returns></returns>
        private static SortedDictionary<DateTime, List<QuoteBar>> GroupBarsByDate(IEnumerable<QuoteBar> bars)
        {
            var groupedBars = new SortedDictionary<DateTime, List<QuoteBar>>();

            foreach (var bar in bars)
            {
                var date = bar.Time.Date;

                if (!groupedBars.ContainsKey(date))
                    groupedBars[date] = new List<QuoteBar>();

                groupedBars[date].Add(bar);
            }

            return groupedBars;
        }
    }
}
