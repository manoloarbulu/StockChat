using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace UnitTestProject
{
    [TestClass]
    public class ApiTest
    {
        private const string apiUrl = "https://localhost:44394/api/stock";

        [TestMethod]
        public void TestGetValidStock()
        {
            RestClient client = new RestClient(apiUrl);
            RestRequest request = new RestRequest("/abc.us", Method.GET);

            IRestResponse response = client.Execute(request);

            //Checking on RabbitMQ message arrival
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void TestGetInvalidStock()
        {
            RestClient client = new RestClient(apiUrl);
            RestRequest request = new RestRequest("/xxxxxx", Method.GET);

            IRestResponse response = client.Execute(request);

            //Checking on RabbitMQ message arrival
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void TestGetEmptyStock()
        {
            RestClient client = new RestClient(apiUrl);
            RestRequest request = new RestRequest("/", Method.GET);

            IRestResponse response = client.Execute(request);

            //Without ID should be NotFound
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
