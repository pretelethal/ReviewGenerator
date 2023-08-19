using Microsoft.AspNetCore.Mvc;

namespace ReviewGenerator.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("API/generate")]
    public class ReviewGeneratorController : ControllerBase
    {
        private readonly ILogger<ReviewGeneratorController> _logger;
        private ITextGenerator _textGenerator;

        public ReviewGeneratorController(ILogger<ReviewGeneratorController> logger, ITextGenerator textGenerator)
        {
            _logger = logger;
            _textGenerator = textGenerator;
        }

        [HttpGet]
        public GeneratedReview Get()
        {
            return new GeneratedReview
            {
                ReviewText = _textGenerator.GenerateRandomString(),
                StarRating = Random.Shared.Next(1, 6)
            };
        }
    }
}
