using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetCore.CAP;

namespace test_cap.Controllers
{
    [Route("test")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        ICapPublisher bus;

        public ValuesController(ICapPublisher publisher)
        {
            bus = publisher;
        }

        // GET api/values/5
        [HttpGet("")]
        public ActionResult<string> Get(int id)
        {
            bus.PublishAsync("test-cap-as-event-bus", DateTime.Now);
            return "ok";
        }

        [NonAction]
        [CapSubscribe("test-cap-as-event-bus")]
        public void TestCap(DateTime now)
        {
            Console.WriteLine($"success at publish: " + now.ToLongTimeString());
            throw new Exception();
        }
    }
}
