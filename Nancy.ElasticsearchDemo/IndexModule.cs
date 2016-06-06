namespace Nancy.ElasticsearchDemo
{
    using System.Collections.Generic;
    using System.Linq;

    using Nancy;
    using Nancy.ElasticsearchDemo.Models;
    using Nancy.ElasticsearchDemo.ViewModels;

    using Nest;

    public class IndexModule : NancyModule
    {
        private readonly IElasticClient client;

        public IndexModule(IElasticClient client)
        {
            this.client = client;

            this.Get["/"] = parameters =>
                {
                    string query = this.Request.Query["q"];
                    if (string.IsNullOrEmpty(query))
                    {
                        return this.View["Index", new List<CountryVm>()];
                    }

                    var countries = this.GetCountries(query);
                    return this.View["Index", countries];
                };

            this.Get["/countries"] = _ =>
                {
                    string query = this.Request.Query["q"];
                    if (string.IsNullOrEmpty(query))
                    {
                        return this.View["Partials/_Results", new List<CountryVm>()];
                    }

                    var countries = this.GetCountries(query);
                    return this.View["Partials/_Results", countries];
                };
        }

        private List<CountryVm> GetCountries(string query)
        {
            var response = this.client.Search<Country>(s => s
                        .Size(15)
                        .Index("demo")
                        .Type("countries")
                        .Query(q => q.QueryString(qs => qs.Query(string.Format("*{0}*", query))))
                        .Highlight(h => h
                            .Fields(f => f.Field("*").RequireFieldMatch(false))
                            .PreTags("<b>")
                            .PostTags("</b>")));

            var countries = response.Hits.Select(
                h =>
                    new CountryVm
                    {
                        Name = h.Source.Name,
                        FlagUrl = string.Format("http://www.geonames.org/flags/x/{0}.gif", h.Source.Alpha2Code.ToLower()),
                        Hightlights = string.Join(" ... ", h.Highlights.Select(hi => string.Concat(hi.Key, ": ", string.Join(" ... ", hi.Value.Highlights.ToArray()))))
                    }).ToList();

            return countries;
        }
    }
}