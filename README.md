![header-cheetah](https://user-images.githubusercontent.com/79997186/184224088-de4f3003-0c22-4a17-8cc7-b341b8e5b55d.png)

&nbsp;
&nbsp;
&nbsp;

## Introduction

This repository hosts the OANDA Brokerage Plugin Integration with the QuantConnect LEAN Algorithmic Trading Engine. LEAN is a brokerage agnostic operating system for quantitative finance. Thanks to open-source plugins such as this [LEAN](https://github.com/QuantConnect/Lean) can route strategies to almost any market.

[LEAN](https://github.com/QuantConnect/Lean) is maintained primarily by [QuantConnect](https://www.quantconnect.com), a US based technology company hosting a cloud algorithmic trading platform. QuantConnect has successfully hosted more than 200,000 live algorithms since 2015, and trades more than $1B volume per month.

### About OANDA

<p align="center">
<picture >
  <source media="(prefers-color-scheme: dark)" srcset="https://cdn.quantconnect.com/i/tu/oanda-logo-darkm.svg">
  <source media="(prefers-color-scheme: light)" srcset="https://cdn.quantconnect.com/i/tu/oanda-logo.svg">
  <img alt="introduction" width="40%">
</picture>
<p>

[OANDA](https://www.oanda.com/) was co-founded by Dr. Stumm, a computer scientist and Dr. Olsen, an economist, in 1997. The company was born out of the belief that the Internet and technology would open up the markets for both currency data and trading. OANDA uses innovative computer and financial technology to provide Internet-based forex trading and currency information services to everyone, from individuals to large corporations, from portfolio managers to financial institutions. OANDA is a market maker and a trusted source for currency data. It has access to one of the world's largest historical, high-frequency, filtered currency databases.

For more information about the OANDA brokerage, see the [QuantConnect-OANDA Integration Page](https://www.quantconnect.com/docs/v2/cloud-platform/live-trading/brokerages/cfd-and-forex-brokerages).


## Using the Brokerage Plugin
  
### Deploying Oanda with VSCode User Interace

  You can deploy using a visual interface in the QuantConnect cloud. For instructions, [QuantConnect-OANDA Integration Page](https://www.quantconnect.com/brokerages/oanda). 
  
  ![deploy-oanda](https://user-images.githubusercontent.com/38889814/188247050-5333413b-b0a0-4fe6-b89d-b1393ab85062.gif)
  
  In the QuantConnect Cloud Platform, you can harness the QuantConnect Live Data Feed. For most users, this is substantially cheaper and easier than self-hosting.
  
### Deploying OANDA with LEAN CLI

To deploy live algorithms with OANDA through the LEAN CLI, see [CFD and FOREX Brokerages](https://www.quantconnect.com/docs/v2/lean-cli/live-trading/brokerages/cfd-and-forex-brokerages) in the LEAN CLI documentation.

## Account Types

OANDA supports cash and margin accounts.

## Order Types and Asset Classes


OANDA supports trading Forex and CFDs with the following order types:

- Market Order
- Limit Order
- Stop-Market Order


## Downloading Data

For local deployment, the algorithm needs to download the following dataset:

[FOREX Data](https://www.quantconnect.com/datasets/oanda-forex)
[CFD Data](https://www.quantconnect.com/datasets/oanda-cfd-data)


## Brokerage Model

Lean models the brokerage behavior for backtesting purposes. The margin model is used in live trading to avoid placing orders that will be rejected due to insufficient buying power.

You can set the Brokerage Model with the following statements
```
SetBrokerageModel(BrokerageName.OandaBrokerage, AccountType.Cash);
SetBrokerageModel(BrokerageName.OandaBrokerage, AccountType.Margin);
```
[Read Documentation](https://www.quantconnect.com/docs/v2/cloud-platform/live-trading/brokerages/cfd-and-forex-brokerages)

### Fees

We model the order fees of OANDA, which are $0.

### Margin

We model buying power and margin calls to ensure your algorithm stays within the margin requirements.

[Read Documentation](https://www.quantconnect.com/docs/v2/cloud-platform/live-trading/brokerages/cfd-and-forex-brokerages)

#### Buying Power

OANDA allows up to 50x leverage for margin accounts.

#### Margin Calls

Regulation T margin rules apply. When the amount of margin remaining in your portfolio drops below 5% of the total portfolio value, you receive a [warning](https://www.quantconnect.com/docs/v2/writing-algorithms/reality-modeling/margin-calls#08-Monitor-Margin-Call-Events). When the amount of margin remaining in your portfolio drops to zero or goes negative, the portfolio sorts the generated margin call orders by their unrealized profit and executes each order synchronously until your portfolio is within the margin requirements.

### Slippage

Orders through OANDA do not experience slippage in backtests. In paper trading and live trading, your orders may experience slippage.

### Fills

We fill market orders immediately and completely in backtests. In live trading, if the quantity of your market orders exceeds the quantity available at the top of the order book, your orders are filled according to what is available in the order book.

### Settlements

Trades settle immediately after the transaction.

### Deposits and Withdraws

You can deposit and withdraw cash from your brokerage account while you run an algorithm that's connected to the account. We sync the algorithm's cash holdings with the cash holdings in your brokerage account every day at 7:45 AM Eastern Time (ET).

&nbsp;
&nbsp;
&nbsp;

![whats-lean](https://user-images.githubusercontent.com/79997186/184042682-2264a534-74f7-479e-9b88-72531661e35d.png)

&nbsp;
&nbsp;
&nbsp;

LEAN Engine is an open-source algorithmic trading engine built for easy strategy research, backtesting, and live trading. We integrate with common data providers and brokerages, so you can quickly deploy algorithmic trading strategies.

The core of the LEAN Engine is written in C#, but it operates seamlessly on Linux, Mac and Windows operating systems. To use it, you can write algorithms in Python 3.8 or C#. QuantConnect maintains the LEAN project and uses it to drive the web-based algorithmic trading platform on the website.

## Contributions

Contributions are warmly very welcomed but we ask you to read the existing code to see how it is formatted, commented and ensure contributions match the existing style. All code submissions must include accompanying tests. Please see the [contributor guide lines](https://github.com/QuantConnect/Lean/blob/master/CONTRIBUTING.md).

## Code of Conduct

We ask that our users adhere to the community [code of conduct](https://www.quantconnect.com/codeofconduct) to ensure QuantConnect remains a safe, healthy environment for
high quality quantitative trading discussions.

## License Model

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You
may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language
governing permissions and limitations under the License.
