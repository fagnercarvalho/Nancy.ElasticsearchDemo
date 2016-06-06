namespace ElasticsearchDemo
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;

    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.ElasticsearchDemo.Models;
    using Nancy.Hosting.Aspnet;
    using Nancy.TinyIoc;

    using Nest;

    using Newtonsoft.Json;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                return new AspNetRootPathProvider();
            }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            SeedDatabase();
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<IElasticClient>(GetElasticClient());
        }

        private static void SeedDatabase()
        {
            Console.WriteLine("Seeding database...");
            InsertCountries();
            Console.WriteLine("Database seeding finished.");
        }

        private static void InsertCountries()
        {
            var client = GetElasticClient();
            var search = client.Search<Country>(s => s.MatchAll());
            if (search.Hits.Count() != 0)
            {
                return;
            }

            var countries = GetCountries();
            client.IndexMany(countries, "demo", "countries");
        }

        private static IEnumerable<Country> GetCountries()
        {
            List<Country> countries = null;
            var client = new HttpClient();
            var task = client.GetAsync("https://restcountries.eu/rest/v1/all")
              .ContinueWith((taskwithresponse) =>
              {
                  var response = taskwithresponse.Result;
                  var jsonString = response.Content.ReadAsStringAsync();
                  jsonString.Wait();
                  countries = JsonConvert.DeserializeObject<List<Country>>(jsonString.Result);
              });
            task.Wait();

            return countries;
        }

        private static ElasticClient GetElasticClient()
        {
            var nodeUri = new Uri(ConfigurationManager.AppSettings["Elasticsearch"]);
            var settings = new ConnectionSettings(nodeUri);
            return new ElasticClient(settings);
        }
    }
}