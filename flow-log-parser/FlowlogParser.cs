using System.IO;
using System.Net.Sockets;

namespace FlowLog
{
    public static class FlowLogParser
    {
        /// <summary>
        /// Reads flow log and maps {protocol:keyword} from protocol.csv
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="protocolDict"></param>
        public static void ReadFlowLog(string filepath, Dictionary<string, string> protocolDict, Dictionary<Tuple<string, string>, string> tagDict, Dictionary<string, int> tagCounts, Dictionary<Tuple<string, string>, int> portProtCounts)
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


            Console.WriteLine();
            // Now check tagDict if the pair exists.
            foreach (var item in portProtocolBucket)
            {
                var pair = new Tuple<string, string>(item.Item1, item.Item2);
                CountFlowlogTags(pair, tagCounts, tagDict);
            }

            OutputFlowlogTagCountsToFile("Tag Counts", "Tag,Count", tagCounts);

            CountPortAndProtocolCombinations(portProtocolBucket, portProtCounts);
            OutputFlowlogPortAndProtocolCountsToFile("Port/Protocol Combination Counts", "Port,Protocol,Count", portProtCounts);

        }

        /// <summary>
        /// Outputs the summary counts from the flowlog
        /// </summary>
        /// <param name="countDict"></param>
        private static void OutputFlowlogTagCountsToFile(string title, string csv, Dictionary<string, int> countDict)
        {
            string filePath = "../output.csv";

            if (!File.Exists(filePath))
            {
                File.AppendAllText(filePath, title + ":\n");
                File.AppendAllText(filePath, csv + "\n");
            }

            foreach (var tag in countDict)
            {
                File.AppendAllText(filePath,
                tag.Key.Trim() +
                "," +
                tag.Value + "\n");

            }
        }

        /// <summary>
        /// Outputs the summary counts of Port,Protocol from flowlog file
        /// </summary>
        /// <param name="countDict"></param>
        private static void OutputFlowlogPortAndProtocolCountsToFile(string title, string csv, Dictionary<Tuple<string, string>, int> countDict)
        {
            string filePath = "../output.csv";

            File.AppendAllText(filePath, title + ":\n");
            File.AppendAllText(filePath, csv + "\n");


            foreach (var tag in countDict)
            {
                File.AppendAllText(filePath,
                tag.Key.Item1.Trim() + "," +
                tag.Key.Item2.Trim() + "," +
                tag.Value + "\n");
            }
        }

        /// <summary>
        /// Removes output file
        /// </summary>
        public static void RemoveOutputFile()
        {
            string filePath = "../output.csv";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
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
        /// Initialilzes lookup table for O(1) lookup time
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="tagDict"></param>
        public static void InitializeTagLookup(string filepath, Dictionary<Tuple<string, string>, string> tagDict)
        {
            using (var reader = new StreamReader(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    char delimiter = ',';
                    string[] lineItems = line.Split(delimiter);

                    string port = lineItems[0];
                    string protocol = lineItems[1];
                    string tag = lineItems[2];

                    tagDict.TryAdd(new Tuple<string, string>(port, protocol), tag);
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
        /// Outputs tags to a file and lists their count from flowlog file
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="protocol"></param>
        /// <param name="tagCounts"></param>
        /// <param name="tagDict"></param>
        public static void CountFlowlogTags(Tuple<string, string> tagPair, Dictionary<string, int> tagCounts, Dictionary<Tuple<string, string>, string> tagDict)
        {
            // Look up the {dst,protocol} in the tag dict for a count
            if (tagDict.ContainsKey(tagPair))
            {
                if (!tagCounts.ContainsKey(tagDict[tagPair]))
                    tagCounts.TryAdd(tagDict[tagPair], 1);
                else
                    tagCounts[tagDict[tagPair]]++;
            }
            else
            {
                // If not there mark as untagged
                if (!tagCounts.ContainsKey("Untagged"))
                    tagCounts.TryAdd("Untagged", 1);
                else
                    tagCounts["Untagged"]++;
            }

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

        private static void CountPortAndProtocolCombinations(List<Tuple<string, string>> portProtocolBucket, Dictionary<Tuple<string, string>, int> portProtCounts)
        {
            foreach (var item in portProtocolBucket)
            {
                var pair = Tuple.Create(item.Item1, item.Item2);
                if (!portProtCounts.ContainsKey(pair))
                    portProtCounts.TryAdd(pair, 1);
                else
                    portProtCounts[pair]++;

                // Console.WriteLine(item.Item1 + "," + item.Item2);
            }
        }
        public static void WriteTagCounts(Dictionary<string, int> tagCounts)
        {
            foreach (var item in tagCounts)
            {
                Console.WriteLine(item.Key.Trim() + "," + item.Value);
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
