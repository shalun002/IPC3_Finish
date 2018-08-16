using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REST
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageBus bus = new MessageBus();
            Logic logic = new Logic();

            Action<CountryResearchRequest> onReceive = logic.OnReceiveInRestService;

            bus.PullMessageFromQueue("CountryResearchRequest", onReceive);
        }
    }
}
