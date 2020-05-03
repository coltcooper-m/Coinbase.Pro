﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Pro.Models;
using Flurl;
using Flurl.Http;

namespace Coinbase.Pro
{
   public interface IOrdersEndpoint
   {
      /// <summary>
      /// List your current open orders. Only open or un-settled orders are returned.
      /// As soon as an order is no longer open and settled, it will no longer
      /// appear in the default request.
      /// </summary>
      /// <param name="status">Status: ['open', 'pending', 'active', 'all']. Limit list of orders to these
      /// statuses. Passing 'all' returns orders of all statuses. To specify multiple statuses,
      /// supply a comma delimited list of statuses. IE: 'open, pending'.</param>
      /// <param name="productId">The coinbase specific product id. IE: 'BTC-USD', 'ETH-USD', etc.</param>
      /// <param name="limit">Number of results per request. Maximum 100. (default 100)</param>
      /// <param name="before">Request page before (newer) this pagination id.</param>
      /// <param name="after">Request page after (older) this pagination id.</param>
      /// <param name="cancellationToken"></param>
      Task<PagedResponse<Order>> GetAllOrdersAsync(
         string status = "all",
         string productId = null,
         int? limit = null, long? before = null, long? after = null,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// Get a single order by order id.
      /// </summary>
      Task<Order> GetOrderAsync(string orderId, CancellationToken cancellationToken = default);

      /// <summary>
      /// With best effort, cancel all open orders. The response is a list of ids of the canceled orders.
      /// </summary>
      /// <param name="productId">When this parameter is null, all orders across all order books are canceled.</param>
      /// <param name="cancellationToken"></param>
      Task<List<Guid>> CancelAllOrdersAsync(string productId = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Cancel a previously placed order.
      /// If the order had no matches during its lifetime its record may be purged.
      /// </summary>
      Task<List<Guid>> CancelOrderById(string orderId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Market orders differ from limit orders in that they provide no pricing
      /// guarantees. They however do provide a way to buy or sell specific
      /// amounts of bitcoin or fiat without having to specify the price. Market
      /// orders execute immediately and no part of the market order will go on
      /// the open order book. Market orders are always considered takers and incur
      /// taker fees. When placing a market order you can specify funds
      /// and/or size. Funds will limit how much of your quote currency
      /// account balance is used and size will limit the bitcoin amount transacted.
      /// </summary>
      /// <param name="side">Buy side or Sell side</param>
      /// <param name="productId">Product Id. IE: 'BTC-USD', 'ETH-USD', etc.</param>
      /// <param name="amount">The amount, size or funds to buy or sell.</param>
      /// <param name="amountType">When placing a market order you can specify funds and/or size.
      ///    Funds will limit how much of your quote currency account balance
      ///    is used and size will limit the cryptocurrency amount transacted.</param>
      /// <param name="clientOid"> The optional client_oid field must be a
      ///    UUID generated by your trading application. This field value
      ///    will be broadcast in the public feed for received messages. You can
      ///    use this field to identify your orders in the public feed. The client_oid
      ///    is different than the server-assigned order id. If you are consuming the
      ///    public feed and see a received message with your client_oid, you should
      ///    record the server-assigned order_id as it will be used for future order
      ///    status updates.The client_oid will NOT be used after the received message is sent.
      ///    The server-assigned order id is also returned as the id field to this
      ///    HTTP POST request.</param>
      /// <param name="cancellationToken"></param>
      Task<Order> PlaceMarketOrderAsync(
         OrderSide side,
         string productId, decimal amount, AmountType amountType = AmountType.UseSize,
         Guid? clientOid = null,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// A limit order will be filled at the price specified or better.
      /// A sell order can be filled at the specified price per bitcoin or
      /// a higher price per bitcoin and a buy order can be filled at
      /// the specified price or a lower price depending on market conditions.
      /// If market conditions cannot fill the limit order immediately,
      /// then the limit order will become part of the open order book until
      /// filled by another incoming order or canceled by the user.
      /// </summary>
      /// <param name="side">Buy side or Sell side</param>
      /// <param name="productId">Product Id. IE: 'BTC-USD', 'ETH-USD', etc.</param>
      /// <param name="size">Size of cryptocurrency to buy or sell when limit price is hit.</param>
      /// <param name="limitPrice">The price of the cryptocurrency to buy or sell at.</param>
      /// <param name="timeInForce">Time in force policies provide guarantees about the lifetime of an order.</param>
      /// <param name="postOnly">The post-only flag indicates that the order should only make
      ///    liquidity. If any part of the order results in taking liquidity, the
      ///    order will be rejected and no part of it will execute.</param>
      /// <param name="clientOid"> The optional client_oid field must be a
      ///    UUID generated by your trading application. This field value
      ///    will be broadcast in the public feed for received messages. You can
      ///    use this field to identify your orders in the public feed. The client_oid
      ///    is different than the server-assigned order id. If you are consuming the
      ///    public feed and see a received message with your client_oid, you should
      ///    record the server-assigned order_id as it will be used for future order
      ///    status updates.The client_oid will NOT be used after the received message is sent.
      ///    The server-assigned order id is also returned as the id field to this
      ///    HTTP POST request.</param>
      /// <param name="cancellationToken"></param>
      Task<Order> PlaceLimitOrderAsync(
         OrderSide side,
         string productId, decimal size, decimal limitPrice,
         TimeInForce timeInForce = TimeInForce.GoodTillCanceled,
         bool postOnly = true,
         Guid? clientOid = null,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// A limit order will be filled at the price specified or better.
      /// A sell order can be filled at the specified price per bitcoin or
      /// a higher price per bitcoin and a buy order can be filled at
      /// the specified price or a lower price depending on market conditions.
      /// If market conditions cannot fill the limit order immediately,
      /// then the limit order will become part of the open order book until
      /// filled by another incoming order or canceled by the user.
      /// </summary>
      /// <param name="side">Buy side or Sell side</param>
      /// <param name="productId">Product Id. IE: 'BTC-USD', 'ETH-USD', etc.</param>
      /// <param name="size">Size of cryptocurrency to buy or sell when limit price is hit.</param>
      /// <param name="limitPrice">The price of the cryptocurrency to buy or sell at.</param>
      /// <param name="cancelAfter">The order remains open on the book until canceled or the
      ///    allotted cancel_after is depleted on the matching engine.
      ///    Orders are guaranteed to cancel before any other order is processed after
      ///    the cancel_after timestamp which is returned by the API. A day is considered 24 hours.</param>
      /// <param name="postOnly">The post-only flag indicates that the order should only make
      ///    liquidity. If any part of the order results in taking liquidity, the
      ///    order will be rejected and no part of it will execute.</param>
      /// <param name="clientOid"> The optional client_oid field must be a
      ///    UUID generated by your trading application. This field value
      ///    will be broadcast in the public feed for received messages. You can
      ///    use this field to identify your orders in the public feed. The client_oid
      ///    is different than the server-assigned order id. If you are consuming the
      ///    public feed and see a received message with your client_oid, you should
      ///    record the server-assigned order_id as it will be used for future order
      ///    status updates.The client_oid will NOT be used after the received message is sent.
      ///    The server-assigned order id is also returned as the id field to this
      ///    HTTP POST request.</param>
      /// <param name="cancellationToken"></param>
      Task<Order> PlaceLimitOrderAsync(
         OrderSide side,
         string productId, decimal size, decimal limitPrice, GoodTillTime cancelAfter,
         bool postOnly = true,
         Guid? clientOid = null,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// Stop orders become active and wait to trigger based on the movement of the
      /// last trade price. There are two types of stop orders, stop loss and stop entry:
      /// 1. stop 'loss': Triggers when the last trade price changes to a value at or below the stop_price.
      /// 2. stop: 'entry': Triggers when the last trade price changes to a value at or above the stop_price.
      /// The last trade price is the last price at which an order was filled.
      /// </summary>
      /// <param name="side">Buy side or Sell side</param>
      /// <param name="productId">Product Id. IE: 'BTC-USD', 'ETH-USD', etc.</param>
      /// <param name="amount">The amount, size or funds to buy or sell.</param>
      /// <param name="amountType">When placing a market order you can specify funds and/or size.
      ///    Funds will limit how much of your quote currency account balance
      ///    is used and size will limit the cryptocurrency amount transacted.</param>
      /// <param name="stopPrice">The market price that will trigger this order to execute.</param>
      /// <param name="clientOid">The optional client_oid field must be a
      ///    UUID generated by your trading application. This field value
      ///    will be broadcast in the public feed for received messages. You can
      ///    use this field to identify your orders in the public feed. The client_oid
      ///    is different than the server-assigned order id. If you are consuming the
      ///    public feed and see a received message with your client_oid, you should
      ///    record the server-assigned order_id as it will be used for future order
      ///    status updates.The client_oid will NOT be used after the received message is sent.
      ///    The server-assigned order id is also returned as the id field to this
      ///    HTTP POST request.</param>
      /// <param name="cancellationToken"></param>
      Task<Order> PlaceStopOrderAsync(
         OrderSide side,
         string productId, decimal amount, AmountType amountType, decimal stopPrice,
         Guid? clientOid = null,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// Create an order that executes when a specific target market price is hit
      /// with limits on the maximum or minimum price you're willing pay or sell
      /// respectively. When the market price reaches the stop price, this kind of
      /// 'stop limit order' will execute as a market order with the specified criteria.
      /// </summary>
      /// <param name="side">Buy side or Sell side</param>
      /// <param name="productId">Product Id. IE: 'BTC-USD', 'ETH-USD', etc.</param>
      /// <param name="size">Size of cryptocurrency to buy or sell when stop price is hit.</param>
      /// <param name="stopPrice">The market price that will trigger this order to execute.</param>
      /// <param name="limitPrice">The maximum price you're willing to pay when the order executes
      ///    or the minimum price you're willing to sell when the order executes.</param>
      /// <param name="clientOid"></param>
      /// <param name="cancellationToken"></param>
      Task<Order> PlaceStopLimitOrderAsync(
         OrderSide side,
         string productId, decimal size, decimal stopPrice, decimal limitPrice,
         Guid? clientOid = null,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// Advanced: Use this method if you want to create your own order with custom fields.
      /// </summary>
      Task<Order> PlaceOrderAsync(CreateOrder o, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseProClient : IOrdersEndpoint
   {
      public IOrdersEndpoint Orders => this;

      protected internal Url OrdersEndpoint => this.Config.ApiUrl.AppendPathSegment("orders");

      Task<PagedResponse<Order>> IOrdersEndpoint.GetAllOrdersAsync(
         string status, string productId,
         int? limit, long? before, long? after,
         CancellationToken cancellationToken)
      {
         var statuses = status.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);

         return this.OrdersEndpoint
            .WithClient(this)
            .SetQueryParam("status", statuses)
            .SetQueryParam("product_id", productId)
            .AsPagedRequest(limit, before, after)
            .GetPagedJsonAsync<Order>(cancellationToken);
      }

      Task<Order> IOrdersEndpoint.GetOrderAsync(
         string orderId, CancellationToken cancellationToken)
      {
         return this.OrdersEndpoint
            .WithClient(this)
            .AppendPathSegment(orderId)
            .GetJsonAsync<Order>(cancellationToken);
      }

      Task<List<Guid>> IOrdersEndpoint.CancelAllOrdersAsync(
         string productId, CancellationToken cancellationToken)
      {
         return this.OrdersEndpoint
            .WithClient(this)
            .SetQueryParam("product_id", productId)
            .DeleteAsync(cancellationToken)
            .ReceiveJson<List<Guid>>();
      }

      Task<List<Guid>> IOrdersEndpoint.CancelOrderById(
         string orderId,
         CancellationToken cancellationToken)
      {
         return this.OrdersEndpoint
            .WithClient(this)
            .AppendPathSegment(orderId)
            .DeleteAsync(cancellationToken)
            .ReceiveJson<List<Guid>>();
      }

      Task<Order> IOrdersEndpoint.PlaceMarketOrderAsync(
         OrderSide side,
         string productId, decimal amount, AmountType amountType,
         Guid? clientOid,
         CancellationToken cancellationToken)
      {
         var mo = new CreateMarketOrder()
            {
               Side = side,
               ProductId = productId,
               Type = OrderType.Market,
               ClientOid = clientOid,
            };

         if ( amountType == AmountType.UseSize )
            mo.Size = amount;
         else if( amountType == AmountType.UseFunds )
            mo.Funds = amount;

         return this.Orders.PlaceOrderAsync(mo, cancellationToken);
      }

      Task<Order> IOrdersEndpoint.PlaceLimitOrderAsync(OrderSide side, string productId, decimal size, decimal limitPrice, TimeInForce timeInForce, GoodTillTime goodTillTime, bool postOnly, Guid? clientOid, CancellationToken cancellationToken)
      {
         var lo = new CreateLimitOrder
         {
            Side = side,
            ProductId = productId,
            Type = OrderType.Limit,
            Price = limitPrice,
            Size = size,
            TimeInForce = timeInForce,
            CancelAfter = goodTillTime,
            PostOnly = postOnly,
            ClientOid = clientOid
         };

         return this.Orders.PlaceOrderAsync(lo, cancellationToken);
      }

      Task<Order> IOrdersEndpoint.PlaceLimitOrderAsync(
         OrderSide side,
         string productId, decimal size, decimal limitPrice,
         GoodTillTime cancelAfter,
         bool postOnly,
         Guid? clientOid,
         CancellationToken cancellationToken)
      {
         var lo = new CreateLimitOrder
            {
               Side = side,
               ProductId = productId,
               Type = OrderType.Limit,
               Price = limitPrice,
               Size = size,
               TimeInForce = TimeInForce.GoodTillTime,
               CancelAfter = cancelAfter,
               PostOnly = postOnly,
               ClientOid = clientOid
            };
         return this.Orders.PlaceOrderAsync(lo, cancellationToken);
      }

      Task<Order> IOrdersEndpoint.PlaceStopOrderAsync(
         OrderSide side,
         string productId, decimal amount, AmountType amountType, decimal stopPrice,
         Guid? clientOid,
         CancellationToken cancellationToken)
      {
         var mo = new CreateMarketOrder
            {
               Side = side,
               ProductId = productId,
               Type = OrderType.Market,
               StopPrice = stopPrice,
               ClientOid = clientOid
            };

         if (amountType == AmountType.UseFunds)
            mo.Funds = amount;
         else if (amountType == AmountType.UseSize)
            mo.Size = amount;

         if (side == OrderSide.Buy)
            mo.Stop = StopType.Entry;
         else if (side == OrderSide.Sell)
            mo.Stop = StopType.Loss;

         return this.Orders.PlaceOrderAsync(mo, cancellationToken);
      }

      Task<Order> IOrdersEndpoint.PlaceStopLimitOrderAsync(OrderSide side, string productId, decimal size, decimal stopPrice, decimal limitPrice, Guid? clientOid, CancellationToken cancellationToken)
      {
         var lo = new CreateLimitOrder
         {
            Side = side,
            ProductId = productId,
            Type = OrderType.Limit,
            Size = size,
            StopPrice = stopPrice,
            Price = limitPrice,
            ClientOid = clientOid
         };

         if (side == OrderSide.Buy)
            lo.Stop = StopType.Entry;
         else if (side == OrderSide.Sell)
            lo.Stop = StopType.Loss;

         return this.Orders.PlaceOrderAsync(lo, cancellationToken);
      }

      Task<Order> IOrdersEndpoint.PlaceOrderAsync(CreateOrder o, CancellationToken cancellationToken)
      {
         return this.OrdersEndpoint
            .WithClient(this)
            .PostJsonAsync(o, cancellationToken)
            .ReceiveJson<Order>();
      }
   }
}
