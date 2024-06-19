using Moq;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.CartSpecifications;
using Sarvicny.Application.Services.Specifications.CustomerSpecification;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;

namespace Sarvicny.Application.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<IServiceProviderRepository> _mockProviderRepository =
            new Mock<IServiceProviderRepository>();

        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<IServiceRepository> _mockServiceRepository = new Mock<IServiceRepository>();
        private readonly Mock<ICustomerRepository> _mockCustomerRepository = new Mock<ICustomerRepository>();
        private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
        private readonly Mock<IOrderRepository> _mockOrderRepository = new Mock<IOrderRepository>();
        private readonly Mock<ICartRepository> _mockCartRepository = new Mock<ICartRepository>();
        private readonly Mock<IOrderService> _mockOrderService = new Mock<IOrderService>();

        private readonly Mock<IServiceProviderService>
            _mockServiceProviderService = new Mock<IServiceProviderService>();

        private readonly Mock<IPaymentService> _mockPaymentService = new Mock<IPaymentService>();
        private readonly Mock<IDistrictRepository> _mockDistrictRepository = new Mock<IDistrictRepository>();

        private CustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerService = new CustomerService(
                _mockProviderRepository.Object,
                _mockUnitOfWork.Object,
                _mockServiceRepository.Object,
                _mockCustomerRepository.Object,
                _mockUserRepository.Object,
                _mockOrderRepository.Object,
                _mockCartRepository.Object,
                _mockOrderService.Object,
                _mockServiceProviderService.Object,
                _mockPaymentService.Object,
                _mockDistrictRepository.Object);
        }

        [Fact]
        public async Task GetCustomerCart_CustomerExists_ReturnsCartDetails()
        {
            // Arrange
            string testCustomerId = "testCustomerId";
            var expectedServiceRequests = new List<dynamic> // Use 'object' here to allow various types
            {
                new // Simulating expected service request details
                {
                    ServiceRequestID = "exampleServiceRequestId",
                    providerId = "exampleProviderId",
                    FirstName = (string?)null, // Explicitly casting to nullable string
                    LastName = (string?)null, // Explicitly casting to nullable string
                    ServiceID = "exampleServiceId",
                    ServiceName = "exampleServiceName",
                    ParentServiceID = (string?)null, // If this could be null
                    parentServiceName = (string?)null, // If this could be null
                    CriteriaID = (string?)null, // Explicitly making string nullable
                    CriteriaName = (string?)null, // Explicitly making string nullable
                    SlotID = "exampleSlotId",
                    StartTime = (DateTime?)DateTime.Now, // Use nullable DateTime if StartTime could be null
                    DistrictID = "exampleDistrictId",
                    DistrictName = "exampleDistrictName",
                    Price = 100m, // Decimal is a value type, but it's not expected to be null here
                    ProblemDescription = (string?)null // If ProblemDescription could be null
                }
                // Add more mock service requests if necessary for testing
            };

            var cart = new Cart
            {
                CartID = "testCartId",
                ServiceRequests = new List<CartServiceRequest>()
            };
            var testCustomer = new Customer { Id = testCustomerId, Cart = cart };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerById(It.IsAny<CustomerWithCartSpecification>()))
                .ReturnsAsync(testCustomer);
            _mockCartRepository.Setup(repo => repo.GetCart(It.IsAny<CartWithServiceRequestsSpecification>()))
                .ReturnsAsync(cart);

            // Act
            var result = await _customerService.GetCustomerCart(testCustomerId);

            // Assert
             
            Assert.False(result.isError);
            Assert.NotNull(result.Payload); // First, ensure the payload itself is not null.
           

            // Cast the payload to a dynamic type before accessing properties
            dynamic actualPayload = result.Payload;

            string actualCartId = actualPayload.CartID.ToString();
           
            // Now, assert that actualPayload indeed contains 'CartID'.
            // But before that, ensure actualPayload actually has the structure we expect.
            // If you're not sure, you can convert result.Payload to a dictionary or examine it under a debugger.
            Assert.NotNull(actualPayload.CartID); // This will throw an exception if actualPayload doesn't have 'CartID'.
            Assert.Equal(cart.CartID, actualPayload.CartID.ToString());
            
        }


        [Fact]
        public async Task GetCustomerCart_CustomerDoesNotExist_ReturnsError()
        {
            // Arrange
            string testCustomerId = "nonExistingCustomerId";
            _mockCustomerRepository.Setup(repo => repo.GetCustomerById(It.IsAny<CustomerWithCartSpecification>()))
                .ReturnsAsync((Customer)null); // Simulate no customer found

            // Act
            var result = await _customerService.GetCustomerCart(testCustomerId);

            // Assert
            Assert.True(result.isError);
            Assert.Null(result.Payload);
            Assert.Equal("Customer is not found", result.Message);
            // You can also verify that errors contain "Error with customer"
        }
    }
}