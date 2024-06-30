using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Common.Helper
{
    public static class HelperMethods
    {
        public static string GenerateOrderDetailsMessageForCustomer(Order order)
        {

            // Construct the order details message here
            // Ensure each line ends with Environment.NewLine or \n for new lines
            return $"Order ID: {order.OrderID}{Environment.NewLine}" +
                   $"Provider: {order.OrderDetails.Provider.FirstName}{Environment.NewLine}" +
                   $"Service: {string.Join(", ", order.OrderDetails.RequestedServices.Select(s => s.Service.ServiceName))}{Environment.NewLine}" +
                   $"Requested Day: {order.OrderDetails.RequestedSlot.RequestedDay}{Environment.NewLine}" +
                   $"Day Of Week: {order.OrderDetails.RequestedSlot.DayOfWeek}{Environment.NewLine}" +
                   $"Requested Slot: {order.OrderDetails.RequestedSlot.StartTime}{Environment.NewLine}" +
                   $"Order Status: {order.OrderStatus}{Environment.NewLine}" +
                   $"Price: {order.OrderDetails.Price:C}";

        }
        public static string GenerateOrderDetailsMessageForProvider(Order order)
        {

            // Construct the order details message here
            // Ensure each line ends with Environment.NewLine or \n for new lines
            return $"Order ID: {order.OrderID}{Environment.NewLine}" +
                   $"Customer: {order.Customer.FirstName}{Environment.NewLine}" +
                   $"Address: {order.Customer.Address}{Environment.NewLine}" +
                   $"District: {order.OrderDetails.providerDistrict.District.DistrictName}{Environment.NewLine}" +
                   $"Service: {string.Join(", ", order.OrderDetails.RequestedServices.Select(s => s.Service.ServiceName))}{Environment.NewLine}" +
                   $"Requested Day: {order.OrderDetails.RequestedSlot.RequestedDay}{Environment.NewLine}" +
                   $"Day Of Week: {order.OrderDetails.RequestedSlot.DayOfWeek}{Environment.NewLine}" +
                   $"Requested Slot: {order.OrderDetails.RequestedSlot.StartTime}{Environment.NewLine}" +
                   $"Order Status: {order.OrderStatus}{Environment.NewLine}" +
                   $"Price: {order.OrderDetails.Price:C}";

        }
    }
}
