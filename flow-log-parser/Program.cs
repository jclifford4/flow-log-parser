using FlowLog;


FlowLogParser.RemoveOutputFile();

var protocolDictonary = new Dictionary<string, string>(); // port , keyword
var tagDictionary = new Dictionary<Tuple<string, string>, string>();    // {protocol, keyword}, tag
var tagCounts = new Dictionary<string, int>();  // tag, count
var portProtCounts = new Dictionary<Tuple<string, string>, int>();  // {port, protocol}, count


FlowLogParser.InitializeProtocolLookup("../protocol-numbers.csv", protocolDictonary);
FlowLogParser.InitializeTagLookup("../lookup-table.csv", tagDictionary);

// Parse Flowlog
FlowLogParser.ReadFlowLog("../flowlog.txt", protocolDictonary, tagDictionary, tagCounts, portProtCounts);
