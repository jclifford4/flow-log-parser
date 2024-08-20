
namespace FlowLog
{
    public static class FlowLogParser
    {
        /// <summary>
        /// Reads flow log and counts the tags and the {port, protocol} combinations
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

            // Count the Tags.
            foreach (var item in portProtocolBucket)
            {
                var pair = new Tuple<string, string>(item.Item1.ToLower(), item.Item2.ToLower());
                CountFlowlogTags(pair, tagCounts, tagDict);
            }

            // Output the tag results.
            OutputFlowlogTagCountsToFile("Tag Counts", "Tag,Count", tagCounts);

            // Count and output the Port results
            CountPortAndProtocolCombinations(portProtocolBucket, portProtCounts);
            OutputFlowlogPortAndProtocolCountsToFile("Port/Protocol Combination Counts", "Port,Protocol,Count", portProtCounts);

        }

        /// <summary>
        /// Outputs the summary counts from the flowlog
        /// </summary>
        /// <param name="countDict"></param>
        private static void OutputFlowlogTagCountsToFile(string title, string csv, Dictionary<string, int> countDict)
        {
            string filePath = "output.csv";

            if (!File.Exists(filePath))
            {
                File.AppendAllText(filePath, title + ":\n");
                File.AppendAllText(filePath, csv + "\n");
            }

            foreach (var tag in countDict)
            {
                File.AppendAllText(filePath,
                tag.Key.Trim().ToLower() +
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
            string filePath = "output.csv";

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
            string filePath = "output.csv";

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
                        protocolDict.TryAdd(protocolDecimal, lineItems[1].ToLower());
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

                    tagDict.TryAdd(new Tuple<string, string>(port.ToLower(), protocol.ToLower()), tag);
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

                    portProtocol.Add(Tuple.Create(dstport.ToLower(), protocolKeyword.ToLower()));

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

        /// <summary>
        /// Adds the count of port and protocol to its respective map
        /// </summary>
        /// <param name="portProtocolBucket"></param>
        /// <param name="portProtCounts"></param>
        private static void CountPortAndProtocolCombinations(List<Tuple<string, string>> portProtocolBucket, Dictionary<Tuple<string, string>, int> portProtCounts)
        {
            foreach (var item in portProtocolBucket)
            {
                var pair = Tuple.Create(item.Item1.ToLower(), item.Item2.ToLower());
                if (!portProtCounts.ContainsKey(pair))
                    portProtCounts.TryAdd(pair, 1);
                else
                    portProtCounts[pair]++;
            }
        }
    }
}
