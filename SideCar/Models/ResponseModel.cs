using Microsoft.AspNetCore.Http;

namespace SideCar.Models
{
    public class ResponseModel
    {
        public MetaData ProcessedRequest {get; set;}
        public MetaData ProcessedResponse {get; set;}        
    }

    public class MetaData 
    {
        public IHeaderDictionary Headers {get; set;}
        public string StatusCode {get; set;} = "n/a";
        public string Body { get; set; } = string.Empty;
    }
}