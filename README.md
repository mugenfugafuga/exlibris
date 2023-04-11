# Exlibris

Exlibris is an Excel function library developed using C# and ExcelDna that provides the following functionalities:

1. JSON serialization and deserialization
2. Web API execution with support for GET, POST, PUT, DELETE, and PATCH methods. And Basic authentication and Bearer token authentication support.
3. WebSocket communication (under development)
4. Execution of C# code using reflection (under development)

The library is intended for use by Excel users who want to do more with Excel and developers who want to extend Excel's capabilities. The library has been developed using .NET 6 and has been tested to work on Windows 11 and Excel for Microsoft 365. Please note that this is a personal development project and the developer cannot guarantee its compatibility with other versions of Excel or operating systems.

## Installation

To install Exlibris, follow these steps:

1. Clone the repository to your local machine.
2. Open the project in Visual Studio.
3. Build the project.
4. Open Excel and load 'exlibris\Exlibris\bin\(Debug|Release)\net6.0-windows\publish\Exlibris-AddIn*-packed.xll'

## Usage

For example, to serialize a JSON object, you can use the following function:

1. Exlibris.JSON.JSONObject() function creates a JSON management object in memory, create JSON management object in memory.

```
=Exlibris.JSON.JSONObject({"name": "John", "age": 30})
```

2. By calling this function, you can obtain a JSON string. The Exlibris.JSON.Stringfy() function converts a JSON object to a string.

```
=Exlibris.JSON.Stringfy([A cell that calls a function in step 1.])
```

For more information on how to use the Exlibris functions, see Excel files in exlibris\samples.
