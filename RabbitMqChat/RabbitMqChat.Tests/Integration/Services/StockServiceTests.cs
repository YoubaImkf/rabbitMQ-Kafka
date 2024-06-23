using RabbitMqChat.Services;

namespace RabbitMqChat.Tests.Integration.Services
{
    [TestClass]
    public class StockServiceTests
    {
        public IStockService? _stockService;

        [TestInitialize]
        public void SetupService()
        {
            HttpClient httpClient = new HttpClient();   
            _stockService = new StockService(httpClient);
        }

        [TestMethod]
        public async Task GetStockCodeSuccesfully()
        {
            //Arrange
            string stockCode = "aapl.us";
            string expectedResponseFormat = $"{stockCode.ToUpper()} quote is";
            
            //Act
            var response = await _stockService.GetStockInfo(stockCode);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains(expectedResponseFormat));

        }

        [TestMethod]
        public async Task GetStockNotValidCode()
        {
            //Arrange
            string stockCode = "XYZ";

            //Act
            var response = await _stockService.GetStockInfo(stockCode);

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(response));
        }
    }
}
