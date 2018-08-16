using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public abstract class MessageDto
    {
        public string QueueName { get; set; }
        public MessageDto(string queueName)
        {
            QueueName = queueName;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class CountryResearchRequest : MessageDto
    {
        public int RequestedUserId { get; set; }
        public DateTime RequestedDateTime { get; set; }
        public string RequestedCountryCode { get; set; }

        public CountryResearchRequest(int requestedUserId, string requestedCountryCode) 
            : base("CountryResearchRequest")
        {
            RequestedUserId = requestedUserId;
            RequestedCountryCode = requestedCountryCode;
            RequestedDateTime = DateTime.Now;
        }      
    }

    public class CountryDto : MessageDto
    {
        public string Name { get; set; }
        public string Alpha3Code { get; set; }
        public string Capital { get; set; }
        public double Population { get; set; }
        public double Gini { get; set; }

        public CountryDto() : base("CountryDto")
        {

        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
