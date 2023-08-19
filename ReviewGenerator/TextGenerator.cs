namespace ReviewGenerator
{
    public interface ITextGenerator
    {
        public List<string> ReadData();

        public List<string> ProcessData(List<string> lines);

        public void TrainData(List<string> words);

        public string GenerateRandomString();
    }

    public interface ITextGeneratorSingleton : ITextGenerator { }

    /// <summary>
    /// Class that generates random text via trained data
    /// </summary>
    public class TextGenerator : ITextGenerator
    {
        private ILogger<TextGenerator> _logger;
        private IConfiguration _config;

        //TODO perhaps use a database instead to trade some up front load time with lower ram usage
        //NOTE the larger the data set the more RAM this will consume
        public Dictionary<string, List<string>> dict { get; private set; } = new Dictionary<string, List<string>>();

        public string filePath { get; set; } = "";

        //NOTE the larger the keySize the longer the training will take
        private int _keySize = 3;
        public int keySize
        {
            get { return _keySize; }
            set
            {
                if (value < 1)
                {
                    _logger.LogError("Key size can't be less than 1");
                    throw new ArgumentException("Key size can't be less than 1");
                }
                else
                {
                    _keySize = value;
                }
            }
        }

        //NOTE this is something that can eventually be changed after training provided there is an endpoint created for this
        private int _outputSize = 10;
        public int outputSize
        {
            get { return _outputSize; }
            set
            {
                if (value < _keySize)
                {
                    _logger.LogError("Output size cannot be less than key size");
                    throw new ArgumentException("Output size cannot be less than key size");
                }
                else
                {
                    _outputSize = value;
                }
            }
        }

        public TextGenerator(IConfiguration configuration, ILogger<TextGenerator> logger)
        {
            _config = configuration;
            _logger = logger;
            LoadConfig();
        }

        /// <summary>
        /// Load config file data
        /// </summary>
        public void LoadConfig()
        {
            try
            {
                filePath = _config.GetRequiredSection("Config").GetValue<string>("DataFilePath");
                keySize = _config.GetRequiredSection("Config").GetValue<int>("MarkovKeySize");
                outputSize = _config.GetRequiredSection("Config").GetValue<int>("CharLimit");
            }
            catch (Exception)
            {
                _logger.LogInformation("Couldn't read application config. Please review config contents.");
            }
        }

        /// <summary>
        /// Open and read a file's contents by line
        /// </summary>
        /// <returns>List of strings</returns>
        public List<string> ReadData()
        {
            var lines = new List<string>();

            try
            {
                lines = File.ReadLines(filePath).ToList();
            }
            catch (Exception)
            {
                _logger.LogInformation("Couldn't read specified file " + filePath);
            }

            return lines;
        }

        /// <summary>
        /// Processes list of json strings into list of words
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>List of processed words</returns>
        public List<string> ProcessData(List<string> lines)
        {
            List<string> words = new List<string>();

            try
            {
                foreach (var line in lines)
                {
                    var review = Newtonsoft.Json.JsonConvert.DeserializeObject<Review>(line);
                    if (review?.ReviewText != null)
                        words.AddRange(review.ReviewText.Split());
                }
            }
            catch (Exception)
            {
                _logger.LogInformation("Json data in data set is malformed. Skipping entry.");
            }

            return words;
        }

        /// <summary>
        /// Train the data via key value pairs
        /// </summary>
        /// <param name="words"></param>
        public void TrainData(List<string> words)
        {
            for (int i = 0; i < words.Count - keySize; i++)
            {
                var key = words.Skip(i).Take(keySize).Aggregate(Join);
                string value;
                if (i + keySize < words.Count)
                    value = words[i + keySize];
                else
                    value = "";

                if (dict.ContainsKey(key))
                    dict[key].Add(value);
                else
                    dict.Add(key, new List<string>() { value });
            }
        }

        //TODO add rules to intelligently clean up some output, good enough for now though
        /// <summary>
        /// Generate a random text string based on trained data dictionary
        /// </summary>
        /// <returns>A randomized text output</returns>
        public string GenerateRandomString()
        {
            if (dict.Count < 1)
                return "";

            Random rand = new Random();
            List<string> output = new List<string>();
            int n = 0;
            int rn = rand.Next(dict.Count);
            string prefix = dict.Keys.Skip(rn).Take(1).Single();
            output.AddRange(prefix.Split());

            while (true)
            {
                if (!dict.ContainsKey(prefix))
                    return output.Aggregate(Join);

                var suffix = dict[prefix];
                if (suffix.Count == 1)
                {
                    if (suffix[0] == "")
                        return output.Aggregate(Join);
                    output.Add(suffix[0]);
                }
                else
                {
                    rn = rand.Next(suffix.Count);
                    output.Add(suffix[rn]);
                }
                if (output.Count >= outputSize)
                    return output.Take(outputSize).Aggregate(Join);
                n++;
                prefix = output.Skip(n).Take(keySize).Aggregate(Join);
            }
        }

        private string Join(string a, string b)
        {
            return a + " " + b;
        }
    }
}