using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public class Job
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("created_at")]
        public string Created_At { get; set; }
        [JsonPropertyName("company")]
        public string Company { get; set; }
        [JsonPropertyName("company_url")]
        public string Company_Url { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("how_to_apply")]
        public string How_To_Apply { get; set; }
        [JsonPropertyName("company_logo")]
        public string Company_Logo { get; set; }
        public User User { get; set; }
    }
}
