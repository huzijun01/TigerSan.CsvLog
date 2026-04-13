# 1. Description:
A class library for logging in CSV format.

# 2. Configuration File:
Before using it, a file named CsvLog.config needs to be created in the root directory of the main program.

If this file is not created, the program will automatically create a default configuration file.

# 3. Console:
While writing the logs to the CSV file, they will also be output in the console.

```txt
2025-06-21 15:38:54
Type: "LOG"
FilePath: "D:\0 File\0 Object\0 C#\TigerSan.CsvLog\Test\LogTest.cs"
MemberName: "WriteLog"
LineNumber: 14
Log: "Log test."
```

# 4. Classes:
## LogData:
Log data.

`_timeFormat`: Timestamp format.

`Time`: Timestamp.

`Type`: Log type.

`MemberName`: Member name.

`FilePath`: File path.

`LineNumber`: Line number.

`Log`: Log content.

## LogHelper:
Log help class.

### Fields:
`_logDir`: Log directory.

`_fileName`: File name.

`_appStartupPath`: Application startup path.

### Properties:
`Instance`: LogHelper instance.

### Methods:
`HelperError`: Internal error of LogHelper.

`Log`: Normal log.

`Warning`: Warning log.

`Error`: Error log.

`DeleteLog`: Delete the log of the day.

`DeleteFolder`: Delete the log folder.

### Additional Methods:
`IsNull`: $"The {name} is null!".

`IsEmpty`: $"The {name} is empty!".

`IsNullOrEmpty`: $"The {name} is null or empty!".

`IsOutOfRange`: $"The {name} is out of range!".

`IsNotContain`: $"The {listName} does not contain {itemName}!".

`ColorWriteLine`: Print colored text.
