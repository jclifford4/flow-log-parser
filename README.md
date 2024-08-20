# Flow Log Parser

# Default Format
The default flow log format is the immutable standard format using v2. The available fields are shown below with some examples:

| Version | account-id | interface-id | srcaddr     | dstaddr     | srcport | dstport | protocol | packets | bytes | start               | end                 | action | log-status |
| ------- | ---------- | ------------ | ----------- | ----------- | ------- | ------- | -------- | ------- | ----- | ------------------- | ------------------- | ------ | ---------- |
| 2.0     | 12345      | eth0         | 192.168.1.1 | 192.168.1.2 | 1234    | 80      | TCP      | 100     | 5000  | 2024-08-01T12:00:00 | 2024-08-01T12:30:00 | ALLOW  | SUCCESS    |
| 2.0     | 12345      | eth1         | 10.0.0.1    | 10.0.0.2    | 5678    | 443     | TCP      | 200     | 15000 | 2024-08-01T13:00:00 | 2024-08-01T13:30:00 | BLOCK  | FAILURE    |
| 2.0     | 67890      | eth0         | 172.16.0.1  | 172.16.0.2  | 1234    | 22      | UDP      | 50      | 2000  | 2024-08-01T14:00:00 | 2024-08-01T14:30:00 | ALLOW  | SUCCESS    |

# Windows Download
- Download the win-x64.zip in Releases
- Unzip the files
- Inside will be:
  - `cd` into the unzipped folder
  - lookup-table.csv for mappings
  - protocol-numbers.csv for protocol codes
  - flow-log-parser
- Run `./flow-log-parser.exe`
  - an output.csv file will be created with desired output
 
---

# Linux Download
- Download the linux-x64.zip in Releases
- Unzip the files
- `cd` into the unzipped folder
- Inside will be:
  - lookup-table.csv for mappings
  - protocol-numbers.csv for protocol codes
  - flow-log-parser
- Do `chmod 777 ./flow-log-parser` so it can be ran
- Run `./flow-log-parser.exe`
  - an output.csv file will be created with desired output

---
