using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

namespace ReviewGenerator.Tests
{
    public class TextGeneratorTests
    {
        IConfiguration _config;

        ILogger<TextGenerator> _logger;

        TextGenerator _tg;

        List<string> singleLine;

        List<string> multipleLines;

        List<string> singleLineEmptyText;

        List<string> singleLineMissingText;

        List<string> singleLineMalformedText;

        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>().Object;
            _logger = new Mock<ILogger<TextGenerator>>().Object;
            _tg = new TextGenerator(_config, _logger);

            singleLine = new List<string>()
            {
                "{\"reviewText\": \"Test review text 1 with some gibberish added\"}"
            };

            multipleLines = new List<string>()
            {
                "{\"reviewText\": \"Test review text 2 with some more gibberish added\"}"
            };
            multipleLines = multipleLines.Concat(singleLine).ToList();

            singleLineEmptyText = new List<string>()
            {
                "{\"reviewText\": \"\"}"
            };

            singleLineMissingText = new List<string>()
            {
                "{}"
            };

            singleLineMalformedText = new List<string>()
            {
                "{}}"
            };
        }

        [Test]
        public void TextGeneratorProcessDataMultipleTest()
        {
            var words = _tg.ProcessData(multipleLines);
            Assert.That(words.Count, Is.EqualTo(17));
        }

        [Test]
        public void TextGeneratorProcessDataSingleTest()
        {
            var words = _tg.ProcessData(singleLine);
            Assert.That(words.Count, Is.EqualTo(8));
        }

        [Test]
        public void TextGeneratorProcessDataEmptyTest()
        {
            var words = _tg.ProcessData(singleLineEmptyText);
            Assert.That(words.Count, Is.EqualTo(1));
        }

        [Test]
        public void TextGeneratorProcessDataMissingTest()
        {
            var words = _tg.ProcessData(singleLineMissingText);
            Assert.That(words.Count, Is.EqualTo(0));
        }

        [Test]
        public void TextGeneratorProcessDataMalformedTest()
        {
            var words = _tg.ProcessData(singleLineMalformedText);
            Assert.That(words.Count, Is.EqualTo(0));
        }

        [Test]
        public void TextGeneratorTrainDataTest()
        {
            var words = _tg.ProcessData(multipleLines);
            _tg.TrainData(words);

            Assert.That(_tg.dict.ContainsKey("Test review text"));
            Assert.That(_tg.dict.ContainsKey("with some more"));

            Assert.That(_tg.dict["Test review text"].Count, Is.EqualTo(2));
            Assert.That(_tg.dict["with some more"].Count, Is.EqualTo(1));
        }

        [Test]
        public void TextGeneratorGenerateRandomStringTest()
        {
            var words = _tg.ProcessData(multipleLines);
            _tg.TrainData(words);
            var str = _tg.GenerateRandomString();

            //kind of hard to test the output of a randomized value but we can at least check if it is not an empty output
            Assert.IsNotEmpty(str);
        }

        [Test]
        public void TextGeneratorOutputSizeTest()
        {
            try
            {
                _tg.keySize = 5;
            }
            catch (ArgumentException)
            {
                Assert.Fail();
            }

            Assert.That(_tg.keySize, Is.EqualTo(5));
        }

        [Test]
        public void TextGeneratorOutputSizeInvalidTest()
        {
            try
            {
                _tg.outputSize = 0;
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

            Assert.That(_tg.outputSize, Is.EqualTo(10));
        }

        [Test]
        public void TextGeneratorKeySizeTest()
        {
            try
            {
                _tg.outputSize = 25;
            }
            catch (ArgumentException)
            {
                Assert.Fail();
            }

            Assert.That(_tg.outputSize, Is.EqualTo(25));
        }

        [Test]
        public void TextGeneratorKeySizeInvalidTest()
        {
            try
            {
                _tg.keySize = 0;
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

            Assert.That(_tg.keySize, Is.EqualTo(3));
        }

    }
}