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
- Unzip the files
- `cd` into the unzipped folder
- Inside will be:
  - lookup-table.csv for mappings
  - protocol-numbers.csv for protocol codes
  - flow-log-parser.exe to run the program
- Run `./flow-log-parser.exe`
  - an output.csv file will be created with desired output
 

# Linux Download
- Download the linux-x64.zip in Releases
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
  - O((n * m) + c + t + p + o) time where n is lines in file, m is length of delimited line, c is counting tags, t is outputing count to file, p is counting port and protocol counts, and o is outputng port counts to file. 
  - This can be reduced to O(n^2 + 4n) == O(n^2). For the entire parse.
