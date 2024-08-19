using FlowLog;

var protocolDictonary = new Dictionary<string, string>();

FlowLogParser.InitializeProtocolLookup("../protocol-numbers.csv", protocolDictonary);

FlowLogParser.ReadFlowLog("../flowlog.txt", protocolDictonary);
