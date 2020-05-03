﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Pro;
using Coinbase.Pro.Models;
using FluentAssertions;
using Flurl;
using NUnit.Framework;

namespace Coinbase.Tests.EndpointTests
{
   [TestFixture]
   public class MarketDataTests : Test
   {
      private CoinbaseProClient client;

      public override void BeforeEachTest()
      {
         base.BeforeEachTest();
         client = new CoinbaseProClient();
      }

      [Test]
      public async Task Can_Get_Currencies()
      {
         server.RespondWith(Examples.Currencies);

         var r = await client.MarketData.GetCurrenciesAsync();

         r.Dump();

         var c = r.First();
         c.Id.Should().Be("BTC");

         server.ShouldHaveCalledSomePathAndQuery("/currencies")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task Can_Get_Stats()
      {
         server.RespondWith(Examples.StatsJson);

         var r = await client.MarketData.GetStatsAsync("BTC-USD");

         r.Dump();

         r.Volume30Day.Should().Be(447043.36757766m);

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/stats")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task Can_Get_Candles()
      {
         //http.ExpectCall(HttpMethod.Get, "/products/BTC-USD/candles")
         //   .RespondJson(HttpStatusCode.OK, Examples.HistoricRatesJson);

         server.RespondWith(Examples.HistoricRatesJson);
         var start = DateTime.Now.AddMinutes(-5);
         var end = DateTime.Now;

         var r = await client.MarketData.GetHistoricRatesAsync("BTC-USD", start, end, 60);

         r.Dump();

         var candle = r.First();

         candle.Low.Value.Should().Be(3820.0m);
         candle.Volume.Value.Should().Be(2.36m);

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/candles?" +
                                    $"start={Url.Encode(start.ToString("o"))}&" +
                                    $"end={Url.Encode(end.ToString("o"))}&" +
                                    $"granularity=60")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task Can_Get_Trades()
      {
         //http.ExpectCall(HttpMethod.Get, "/products/BTC-USD/trades")
         //   .RespondJson(HttpStatusCode.OK, Examples.TradesJson);
         SetupServerPagedResponse(Examples.TradesJson);

         var r = await client.MarketData.GetTradesAsync("BTC-USD");

         r.Dump();

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/trades")
            .WithVerb(HttpMethod.Get);

         var t = r.Data.First();

         t.Price.Should().Be(4013.64000000m);
         t.Side.Should().Be(OrderSide.Sell);
         t.Size.Should().Be(0.00842855m);
         t.Time.Should().Be(DateTimeOffset.Parse("2018-11-26T07:23:53.243Z"));
         t.TradeId.Should().Be(54706228);

         r.Data.Count.Should().BeGreaterOrEqualTo(3);
         r.Before.Should().Be(54870014);
         r.After.Should().Be(54870113);
      }

      [Test]
      public async Task Can_Make_Trades_Paged()
      {
         SetupServerPagedResponse(Examples.TradesJson);

         var r = await client.MarketData.GetTradesAsync("BTC-USD", limit: 50, before: 27, after: 28);

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/trades?" +
                                    "limit=50&" +
                                    "before=27&" +
                                    "after=28")
            .WithVerb(HttpMethod.Get);

         r.Before.Should().NotBeNull();
         r.After.Should().NotBeNull();
      }

      [Test]
      public async Task Can_Get_Ticker()
      {
         server.RespondWith(Examples.TickerJson);

         var r = await client.MarketData.GetTickerAsync("BTC-USD");

         r.Dump();

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/ticker")
            .WithVerb(HttpMethod.Get);

         r.Ask.Should().Be(4027.95m);
         r.Bid.Should().Be(4027.82m);
         r.Price.Should().Be(4029.70000000m);
         r.Size.Should().Be(0.00100000m);
         r.Time.Should().Be(DateTimeOffset.Parse("2018-11-26T07:13:57.089000Z"));
         r.TradeId.Should().Be(54705877);
         r.Volume.Should().Be(32218.97638837m);
      }

      [Test]
      public async Task Can_Get_Orderbook_l3()
      {
         server.RespondWith(Examples.OrderBookLevel3Json);

         var r = await client.MarketData.GetOrderBookAsync("ETC-BTC", 3);

         r.Dump();

         server.ShouldHaveCalledSomePathAndQuery("/products/ETC-BTC/book?level=3")
            .WithVerb(HttpMethod.Get);

         r.Bids[0].Price.Should().Be(0.00117m);
         r.Bids[0].Size.Should().Be(5.5m);
         r.Bids[0].OrderId.Should().Be("88588a7f-5d24-4131-b270-394dd05a1353");

      }

      [Test]
      public async Task Can_Get_Orderbook_l2()
      {
         server.RespondWith(Examples.OrderBookLevel2Json);

         var r = await client.MarketData.GetOrderBookAsync("BTC-USD", 2);

         r.Dump();

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/book?level=2")
            .WithVerb(HttpMethod.Get);

         r.Asks[0].Price.Should().Be(3931.11m);
         r.Asks[0].Size.Should().Be(0.96328664m);
         r.Asks[0].OrderCount.Should().Be(1);
      }

      [Test]
      public async Task Can_Get_Orderbook()
      {
         server.RespondWith(Examples.OrderBookLevel1Json);

         var r = await client.MarketData.GetOrderBookAsync("BTC-USD", 1);

         r.Dump();

         server.ShouldHaveCalledSomePathAndQuery("/products/BTC-USD/book?level=1")
            .WithVerb(HttpMethod.Get);

         r.Sequence.Should().Be(7416643032);
         r.Bids[0].Price.Should().Be(3967.55m);
         r.Bids[0].Size.Should().Be(57.6603824m);
         r.Bids[0].OrderCount.Should().Be(12);
         r.Bids.Length.Should().Be(1);

         r.Asks[0].Price.Should().Be(3967.56m);
         r.Asks[0].Size.Should().Be(0.001m);
         r.Asks[0].OrderCount.Should().Be(1);
         r.Asks.Length.Should().Be(1);
      }

      [Test]
      public async Task Can_Get_Products()
      {
         server.RespondWith(Examples.ProductsJson);
        
         var r = await client.MarketData.GetProductsAsync();

         server.ShouldHaveCalledSomePathAndQuery("/products")
            .WithVerb(HttpMethod.Get);

         r.Count.Should().BeGreaterOrEqualTo(3);

         var p = r.First();
         p.Id.Should().Be("BCH-USD");
         p.BaseCurrency.Should().Be("BCH");
         p.QuoteCurrency.Should().Be("USD");
         p.BaseMinSize.Should().Be(0.01m);
         p.BaseMaxSize.Should().Be(350.0m);
         p.QuoteIncrement.Should().Be(0.01m);
         p.CancelOnly.Should().Be(false);
         p.DisplayName.Should().Be("BCH/USD");
         p.LimitOnly.Should().Be(true);
         p.MaxMarketFunds.Should().Be(1000000);
         p.MinMarketFunds.Should().Be(10);
         p.PostOnly.Should().Be(false);
         p.Status.Should().Be("online");
         p.StatusMessage.Should().BeNull();
      }
   }


   public class AuthenticatedApiTest : Test
   {
      protected CoinbaseProClient client;

      [SetUp]
      public new void BeforeEachTest()
      {
         client = new CoinbaseProClient(new Config{ ApiKey = "key", Secret = "secret", Passphrase = "satoshi", UseTimeApi = false});
      }

      protected override void EnsureEveryRequestHasCorrectHeaders()
      {
         base.EnsureEveryRequestHasCorrectHeaders();

         server.ShouldHaveMadeACall()
            .WithHeader(HeaderNames.AccessKey, "key")
            .WithHeader(HeaderNames.AccessSign)
            .WithHeader(HeaderNames.AccessPassphrase, "satoshi")
            .WithHeader(HeaderNames.AccessTimestamp);
      }
   }

   //public static class TestExtensions
   //{
   //   public static HttpTest RespondJson(this HttpTest source, HttpStatusCode statusCode, string content)
   //   {
   //      source.RespondWith(content, status: (int)statusCode,  )
   //      return source.Respond(statusCode, Constants.Json, content);
   //   }
   //}

   public static class Constants
   {
      public const string Json = "application/json";
   }
}
