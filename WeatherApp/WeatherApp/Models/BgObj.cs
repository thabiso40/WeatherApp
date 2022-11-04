using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Models
{
    public class BackgroundInfo
    {
        public int total_results { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public photo[] photos { get; set; }
        public string next_page { get; set; }
    }

    public class photo
    {
        public int id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
        public string photographer { get; set; }
        public string photographer_url { get; set; }
        public int photographer_id { get; set; }

        public Src src { get; set; }
        public bool liked { get; set; }

    }

    public class Src
    {
        public string origional { get; set; }
        public string large2x { get; set; }
        public string large { get; set; }
        public string medium { get; set; }

        public string small { get; set; }
        public string portrate { get; set; }
        public string landscape { get;set; }
        public string tiny { get; set; }
    }
}
