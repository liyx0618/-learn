using System;
using System.Collections.Generic;

namespace EventBasedDDD
{
    [Serializable]
    public class ErrorItem
    {
        public ErrorItem() { Parameters = new List<object>(); }
        public string Key { get; set; }
        public List<object> Parameters { get; set; }
    }

    [Serializable]
    public class ErrorState
    {
        public ErrorState() { ErrorItems = new List<ErrorItem>(); }
        public List<ErrorItem> ErrorItems { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
