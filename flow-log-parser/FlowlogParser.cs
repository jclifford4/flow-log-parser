using System.IO;

namespace FlowLog
{
    public static class FlowLogParser
    {
        /// <summary>
        /// Reads a flow log file and writes dstport and protocol type to console
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="protocolDict"></param>
        public static void ReadFlowLog(string filepath, Dictionary<string, string> protocolDict)
        {
            var portProtocolBucket = new List<Tuple<string, string>>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    GetDSTPortAndProtocol(line, portProtocolBucket, protocolDict);
                }
            }

            WriteDSTPortAndProtocol(portProtocolBucket);
        }

        /// <summary>
        /// Initializes protocol number : keyword dictionary for lookup
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="protocolDict"></param>
        public static void InitializeProtocolLookup(string filepath, Dictionary<string, string> protocolDict)
        {
            using (var reader = new StreamReader(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    char delimiter = ',';
                    string[] lineItems = line.Split(delimiter);

                    string protocolDecimal = lineItems[0];

                    if (string.IsNullOrWhiteSpace(lineItems[1]))
                    {

                        protocolDict.TryAdd(protocolDecimal, "N/A");
                    }
                    else
                    {
                        protocolDict.TryAdd(protocolDecimal, lineItems[1]);
                    }

                }
            }
        }


        /// <summary>
        /// Grabs the dstport and protocol number from the flow log
        /// </summary>
        /// <param name="line"></param>
        /// <param name="portProtocol"></param>
        /// <param name="protocolDict"></param>
        /// <returns>bool</returns>
        private static bool GetDSTPortAndProtocol(string line, List<Tuple<string, string>> portProtocol, Dictionary<string, string> protocolDict)
        {
            char delimiter = ' ';
            foreach (var item in line)
            {
                string[] lineItems = line.Split(delimiter);

                if (lineItems.Length >= 8)
                {
                    string dstport = lineItems[6];
                    string protocolNumber = lineItems[7];

                    string protocolKeyword = GetProtocolKeyword(protocolNumber, protocolDict);

                    portProtocol.Add(Tuple.Create(dstport, protocolKeyword));

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts protocol number to protocol keyword
        /// </summary>
        /// <param name="protocolNumber"></param>
        /// <param name="protocolDict"></param>
        /// <returns>string</returns>
        private static string GetProtocolKeyword(string protocolNumber, Dictionary<string, string> protocolDict)
        {
            return protocolDict.ContainsKey(protocolNumber) ? protocolDict[protocolNumber].ToLower() : "n/a";
        }

        private static void WriteDSTPortAndProtocol(List<Tuple<string, string>> portProtocolBucket)
        {
            foreach (var item in portProtocolBucket)
            {
                Console.WriteLine(item.Item1 + "," + item.Item2);
            }
        }

        /// <summary>
        /// Reads and converts protocol decimal to its associated Keyword
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="protocolNumber"></param>
        private static void ReadProtocolNumbers(string filepath, int protocolNumber)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                try
                {
                    string line = GetLineFromFile(filepath, protocolNumber + 1);
                    if (line != null)
                    {
                        Console.WriteLine(line);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }

            }
        }


        /// <summary>
        /// Grabs specific line matching protocol number and returns the Keyword
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string GetLineFromFile(string filepath, int lineNumber)
        {
            if (lineNumber < 1)
            {
                throw new ArgumentException("Line number is less than 1!");
            }

            using (var reader = new StreamReader(filepath))
            {
                for (int i = 1; i < lineNumber; i++)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        return null;
                    }

                    if (i == lineNumber)
                    {
                        char delimiter = ',';
                        string[] lineItems = line.Split(delimiter);
                        return lineItems[1];

                    }
                }
            }

            return null;
        }
    }

}
