using System;
using Microsoft.AspNetCore.Mvc;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        private readonly IMessageSender _messageSender;

        public ArticlesController(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [HttpGet]
        public IActionResult SendTestMessage()
        {
            try
            {
                var msg = new BusinessMessage()
                {
                    Content = "someContent",
                    CorrelationId = Guid.Parse(Constants.HelloWorldMessage)
                };

                _messageSender.Send(msg);

                return Content("Handled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody]string article)
        {
            try
            {
                var msg = new BusinessMessage() { Content = article, CorrelationId = Guid.NewGuid() };
                _messageSender.Send(msg);
                return Ok(msg.CorrelationId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
