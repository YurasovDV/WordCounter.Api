using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly IMessageSender _messageSender;
        private readonly ICountersService _countersService;

        public ArticlesController(ILogger<ArticlesController> logger,  IMessageSender messageSender, ICountersService countersService)
        {
            _logger = logger;
            _messageSender = messageSender;
            _countersService = countersService;
        }

        [HttpGet("{id}")]
        public IActionResult GetStatus(Guid id)
        {
            _logger.LogInformation($"GetStatus: got request {id}");

            if (id == Guid.Empty)
            {
                throw new Exception("empty guid");
            }
            var row = _countersService.GetResultByCorrelationId(id);
            if (row == null)
            {
                return StatusCode(202);
            }

            return Ok(row.WordCount);
        }

        [HttpGet]
        public IActionResult SendTestMessage()
        {
            try
            {
                var msg = new BusinessMessage()
                {
                    Content = "some Content",
                    CorrelationId = Guid.Parse(Constants.HelloWorldMessage)
                };

                _logger.LogInformation($"SendTestMessage: created message {msg.CorrelationId}");

                var id = _countersService.CreateRequest(new CountRequest() { Content = msg.Content, CorrelationId = msg.CorrelationId });

                _logger.LogInformation($"SendTestMessage: stored in db {id}");

                _messageSender.Send(msg);

                _logger.LogInformation($"SendTestMessage: sent {id}");

                return Content(msg.CorrelationId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Dto dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException("dto");
                }

                if (string.IsNullOrWhiteSpace(dto.Article))
                {
                    throw new ArgumentNullException("dto.Article");
                }

                var msg = new BusinessMessage() { Content = dto.Article, CorrelationId = Guid.NewGuid() };

                _logger.LogInformation($"Post: created message {msg.CorrelationId} content='{msg.Content}'");

                var id = _countersService.CreateRequest(new CountRequest() { Content = msg.Content, CorrelationId = msg.CorrelationId });

                _logger.LogInformation($"Post: stored in db {id}");

                _messageSender.Send(msg);

                _logger.LogInformation($"Post: sent {id}");

                return Ok(msg.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
