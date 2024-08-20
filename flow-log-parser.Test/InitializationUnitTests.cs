namespace flow_log_parser.Test;
using FlowLog;


public class InitializationUnitTests
{

    // var protocolDictonary = new Dictionary<string, string>(); // port , keyword
    //     var tagDictionary = new Dictionary<Tuple<string, string>, string>();    // {protocol, keyword}, tag
    //     var tagCounts = new Dictionary<string, int>();  // tag, count
    //     var portProtCounts = new Dictionary<Tuple<string, string>, int>();  // {port, protocol}, count


    [Fact]
    public void InitializeProtocolData_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var protocolDictonary = new Dictionary<string, string>(); // port , keyword

        // Act
        FlowLogParser.InitializeProtocolLookup("../protocol-numbers.csv", protocolDictonary);

        // Assert
        Assert.NotEmpty(protocolDictonary);


    }
    [Fact]
    public void InitializeTagData_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var tagDictionary = new Dictionary<Tuple<string, string>, string>();    // {protocol, keyword}, tag

        // Act
        FlowLogParser.InitializeTagLookup("../lookup-table.csv", tagDictionary);

        // Assert
        Assert.NotEmpty(tagDictionary);

    }
}
