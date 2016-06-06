namespace Nancy.ElasticsearchDemo.Models
{
    using System.Collections.Generic;

    public class Country
    {
        public string Name { get; set; }

        public string Capital { get; set; }

        public List<string> AltSpellings { get; set; }

        public string Relevance { get; set; }

        public string Region { get; set; }

        public string Subregion { get; set; }

        public Translations Translations { get; set; }

        public int Population { get; set; }

        public List<double> Latlng { get; set; }

        public string Demonym { get; set; }

        public double? Area { get; set; }

        public double? Gini { get; set; }

        public List<string> Timezones { get; set; }

        public List<string> Borders { get; set; }

        public string NativeName { get; set; }

        public List<string> CallingCodes { get; set; }

        public List<string> TopLevelDomain { get; set; }

        public string Alpha2Code { get; set; }

        public string Alpha3Code { get; set; }

        public List<string> Currencies { get; set; }

        public List<string> Languages { get; set; }
    }
}