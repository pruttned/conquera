using System.Collections.Generic;

namespace Ale.Tools
{
    public static class CommandLineReader
    {
        private const string mParameterKeyStartChar = "-";

        public static Dictionary<string, string> ReadParameters(string[] parameters)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (null != parameters)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string key = parameters[i];

                    if (key.StartsWith(mParameterKeyStartChar))
                    {
                        string value = null;
                        if (i < parameters.Length - 1 && !parameters[i + 1].StartsWith(mParameterKeyStartChar))
                        {
                            value = parameters[i + 1];
                            i++;
                        }
                        dictionary.Add(key.Substring(1), value);
                    }
                }
            }
            return dictionary;
        }
    }
}
