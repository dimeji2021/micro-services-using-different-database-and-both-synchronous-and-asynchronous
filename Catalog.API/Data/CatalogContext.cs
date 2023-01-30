using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IConfiguration configuration)
        {

            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString")); //Connect to Mongo client
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));// creating a database if there is none

            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
            CatalogContextSeed.SeedData(Products);
        }
        public IMongoCollection<Product> Products { get; }
    }
}

