using Ordering.Domain.Common;

namespace Ordering.Domain.Entities
{
    public class Order : EntityBase, IAggregateRoot
    {
        public Order()
        {
        }

        public Order(string userName, decimal totalPrice, int paymentMethod)
        {
            UserName = userName;
            TotalPrice = totalPrice;
            PaymentMethod = paymentMethod;
        }

        public string? UserName { get; set; }
        public decimal TotalPrice { get; set; }

        // BillingAddress
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? AddressLine { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

        // Payment
        public string? CardName { get; set; }

        public string? CardNumber { get; set; }
        public string? Expiration { get; set; }
        public string? CVV { get; set; }
        public int PaymentMethod { get; set; }

        public void SetPaymentMethod(int paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method Order.AddOrderItem() which includes behavior.
        //private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        // Using List<>.AsReadOnly()
        // This will create a read only wrapper around the private list so is protected against "external updates".
        // It's much cheaper than .ToList() because it will not have to copy all items in a new collection. (Just one heap alloc for the wrapper instance)
        //https://msdn.microsoft.com/en-us/library/e78dcd75(v=vs.110).aspx
        //public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        //public decimal Total()
        //{
        //    var total = 0m;
        //    foreach (var item in _orderItems)
        //    {
        //        total += item.UnitPrice * item.Units;
        //    }
        //    return total;
        //}

        //Value Objects
        //public Address Address { get; set; }
    }
}