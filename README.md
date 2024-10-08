# Flow Log Parser

# Default Format
The default flow log format is the immutable standard format using v2. The available fields are shown below with some examples:

| Version | account-id | interface-id | srcaddr     | dstaddr     | srcport | dstport | protocol | packets | bytes | start               | end                 | action | log-status |
| ------- | ---------- | ------------ | ----------- | ----------- | ------- | ------- | -------- | ------- | ----- | ------------------- | ------------------- | ------ | ---------- |
| 2       | 12345      | eth0         | 192.168.1.1 | 192.168.1.2 | 1234    | 80      | TCP      | 100     | 5000  | 2024-08-01T12:00:00 | 2024-08-01T12:30:00 | ALLOW  | SUCCESS    |
| 2       | 12345      | eth1         | 10.0.0.1    | 10.0.0.2    | 5678    | 443     | TCP      | 200     | 15000 | 2024-08-01T13:00:00 | 2024-08-01T13:30:00 | BLOCK  | FAILURE    |
| 2       | 67890      | eth0         | 172.16.0.1  | 172.16.0.2  | 1234    | 22      | UDP      | 50      | 2000  | 2024-08-01T14:00:00 | 2024-08-01T14:30:00 | ALLOW  | SUCCESS    |

# Windows Download
- Download the win-x64.zip in Releases
- Grab the README.md as well
- Unzip the files
- `cd` into the unzipped folder
- Inside will be:
  - lookup-table.csv for mappings
  - protocol-numbers.csv for protocol codes
  - flow-log-parser.exe to run the program
- Run `./flow-log-parser.exe`
  - an output.csv file will be created with desired output
 
 # If you don't want to run the exe:
 - You will need a few things to build the project
 - [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) on your machine.
 - VS Code plus:
    - [.Net Install Tool](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscode-dotnet-runtime)
    - [C\# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
    - You may have to restart VS Code to allow changes to take place
 - Clone the repo to desired location
 - `cd flow-log-parser` and `code .` for full project
    -  then `dotnet build`
    -  then `cd flow-log-parser` again
    -  then `dotnet run`
    -  The `output.csv` will be appear inside the project

# Linux Download
- Download the linux-x64.zip in Releases
- Grab the README.md as well
- Unzip the files
- `cd` into the unzipped folder
- Inside will be:
  - lookup-table.csv for mappings
  - protocol-numbers.csv for protocol codes
  - flow-log-parser to run the program
- Do `chmod 777 ./flow-log-parser` so it can be ran
- Run `./flow-log-parser`
  - an output.csv file will be created with desired output

# Assumptions
- All flow logs are to be default and v2.
- Flow log file can be <= 10 MB.
- Lookup file can have <= 10,000 mappings.
- The lookup table will be a csv file and provided on your end with your specified mappings.
- Tags can be mapped more than one way.


# Testing
- Light testing using xUnit on initialization of maps and counts.

# Analysis
- Parsing protocol-numbers.csv and grabbing the each 0th and 1st indices:
  - O(n * m) time, where n is number of lines in the file, m is the length of delimited line.
- Parsing lookup-table.csv for mappings:
  - O(n * m) time, where n is lines in file, m is length of line.
- Parsing flowlog.txt:
  - O((n * m) + c + t + p + o) time where n is lines in file, m is length of delimited line, c is counting tags, t is outputting count to file, p is counting port and protocol counts, and o is outputting port counts to file. 
  - This can be reduced to O(n^2 + 4n) == O(n^2). For the entire parse.
