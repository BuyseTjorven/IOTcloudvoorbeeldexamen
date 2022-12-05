using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace MCT.Function
{
    public static class School
    {
        [FunctionName("getKind")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getall")] HttpRequest req,
            ILogger log)
        {
            CosmosClientOptions options = new CosmosClientOptions();
            options.ConnectionMode = ConnectionMode.Gateway;
            CosmosClient client = new CosmosClient(Environment.GetEnvironmentVariable("connectionstring"), options);


            //connect to database
            //get container
            Container container = client.GetContainer("voorbeeldexamen", "Kind");

            //Creating query
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            //Creating list to put items in that will be returned
            List<object> items = new List<object>();
            using (FeedIterator<object> resultSet = container.GetItemQueryIterator<object>(
                queryDefinition: query))
            {
                while (resultSet.HasMoreResults)
                {
                    //Get the items and put them in the list
                    FeedResponse<object> response = await resultSet.ReadNextAsync();
                    items.AddRange(response);
                }
            }
            //return the list
            return new OkObjectResult(items);
        }

        [FunctionName("addKind")]
        public static async Task<IActionResult> addKind(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "addchild")] HttpRequest req,
                ILogger log)
        {
            string json = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic document2object = JsonConvert.DeserializeObject<JObject>(json);
            var id = Guid.NewGuid().ToString();
            document2object.id = id;

            //Create Cosmos client
            CosmosClientOptions options = new CosmosClientOptions();
            options.ConnectionMode = ConnectionMode.Gateway;
            //connect to database
            CosmosClient client = new CosmosClient(Environment.GetEnvironmentVariable("connectionstring"), options);
            //get container in a database
            Container container = client.GetContainer("voorbeeldexamen", "Kind");



            //Get the response
            //await container.CreateItemAsync(document2object, new PartitionKey(id.ToString()));
            var response = await container.CreateItemAsync(document2object, new PartitionKey(id.ToString()));
            return new OkObjectResult("");
        }
        [FunctionName("getMaaltijdenKind")]
        public static async Task<IActionResult> getMaaltijdenKind(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            CosmosClientOptions options = new CosmosClientOptions();
            options.ConnectionMode = ConnectionMode.Gateway;
            CosmosClient client = new CosmosClient(Environment.GetEnvironmentVariable("connectionstring"), options);


            //connect to database
            //get container
            Container container = client.GetContainer("voorbeeldexamen", "Maaltijd");

            //Creating query
            QueryDefinition query = new QueryDefinition($"SELECT * FROM c where c.stamboeknummer = \"{id}\"");

            //Creating list to put items in that will be returned
            List<object> items = new List<object>();
            using (FeedIterator<object> resultSet = container.GetItemQueryIterator<object>(
                queryDefinition: query))
            {
                while (resultSet.HasMoreResults)
                {
                    //Get the items and put them in the list
                    FeedResponse<object> response = await resultSet.ReadNextAsync();
                    items.AddRange(response);
                }
            }
            //return the list
            return new OkObjectResult(items);
        }

        [FunctionName("addMaaltijd")]
        public static async Task<IActionResult> addMaaltijd(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "addmaaltijd")] HttpRequest req,
                ILogger log)
        {
            string json = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic document2object = JsonConvert.DeserializeObject<JObject>(json);
            var id = Guid.NewGuid().ToString();
            document2object.id = id;

            //Create Cosmos client
            CosmosClientOptions options = new CosmosClientOptions();
            options.ConnectionMode = ConnectionMode.Gateway;
            //connect to database
            CosmosClient client = new CosmosClient(Environment.GetEnvironmentVariable("connectionstring"), options);
            //get container in a database
            Container container = client.GetContainer("voorbeeldexamen", "Maaltijd");



            //Get the response
            //await container.CreateItemAsync(document2object, new PartitionKey(id.ToString()));
            var response = await container.CreateItemAsync(document2object, new PartitionKey(id.ToString()));
            return new OkObjectResult("");
        }

        [FunctionName("getMaaltijdenKlas")]
        public static async Task<IActionResult> getMaaltijdenKlas(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "klas/{klas}")] HttpRequest req,
            string klas,
            ILogger log)
        {
            CosmosClientOptions options = new CosmosClientOptions();
            options.ConnectionMode = ConnectionMode.Gateway;
            CosmosClient client = new CosmosClient(Environment.GetEnvironmentVariable("connectionstring"), options);


            //connect to database
            //get container
            Container container = client.GetContainer("voorbeeldexamen", "Kind");

            //Creating query
            QueryDefinition query = new QueryDefinition($"SELECT * FROM c where c.klas = \"{klas}\"");



            //Creating list to put items in that will be returned

            List<string> stamboeknummers = new List<string>();
            using (FeedIterator<leerling> resultSet = container.GetItemQueryIterator<leerling>(
                queryDefinition: query))
            {
                while (resultSet.HasMoreResults)
                {
                    //Get the items and put them in the list
                    FeedResponse<leerling> response = await resultSet.ReadNextAsync();
                    foreach (var item in response)
                    {
                        stamboeknummers.Add(item.stamboeknummer);
                    }
                }
            }

            List<object> items = new List<object>();
            Container container2 = client.GetContainer("voorbeeldexamen", "Maaltijd");
            foreach (var stamboeknummer in stamboeknummers)
            {
                QueryDefinition query2 = new QueryDefinition($"SELECT * FROM c where c.stamboeknummer = \"{stamboeknummer}\"");
                using (FeedIterator<object> resultSet = container2.GetItemQueryIterator<object>(
                queryDefinition: query2))
                {
                    while (resultSet.HasMoreResults)
                    {
                        //Get the items and put them in the list
                        FeedResponse<object> response = await resultSet.ReadNextAsync();
                        items.AddRange(response);
                    }
                }
            }
            return new OkObjectResult(items);
        }
    }
}
