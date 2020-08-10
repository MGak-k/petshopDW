using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Data;

namespace BusinessLogic.Helpers
{
    public class Logger
    {
        public string ConnectionString;
        public const string LogTableName = "Logs";
        public ColumnOptions ColumnOptions;
        public Serilog.Core.Logger _Log;
        public string Module;

        public Logger()
        {
            SetupLogger();
        }

        public Logger(string moduleName)
        {
            Module = moduleName;
            SetupLogger();
        }

        private void SetupLogger()
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AuthDb"].ConnectionString;
            ColumnOptions = new ColumnOptions()
            {
                AdditionalDataColumns = new List<DataColumn>
                {
                    new DataColumn { DataType = typeof(string), ColumnName = "StackTrace" },
                    new DataColumn { DataType = typeof(string), ColumnName = "Module" },
                }
            };

            _Log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.MSSqlServer(ConnectionString, LogTableName, columnOptions: ColumnOptions)
                .CreateLogger();

            Log.Logger = _Log;
        }

        public void Verbose(string Title, dynamic Message, string User = "")
        {
            Log.ForContext("Module", Module)
               .ForContext("UserName", User)
               .Verbose("{Title}: {Message}", Title, Message);
        }

        public void Exception(Exception ex, string User = "")
        {
            Log.ForContext("Module", Module)
               .ForContext("UserName", User)
               .ForContext("StackTrace", ex.StackTrace)
               .Error(ex, "{Message}", ex.Message);
        }

        public void Close()
        {
            Log.CloseAndFlush();
        }
    }
}

