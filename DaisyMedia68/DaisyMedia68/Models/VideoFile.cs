using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DaisyMvc.Models
{
    public class VideoFile
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Title { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("FileStream")]
        public byte[] FileStream { get; set; }
    }
}