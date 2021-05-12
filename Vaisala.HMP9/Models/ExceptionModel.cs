using System;

namespace Vaisala.HMP.Models
{
    public class ExceptionModel
    {
        public ExceptionModel(string message) {
            Time = DateTime.Now;
            Message = message;
        }

        public DateTime Time { get; private set; }
        public string Message { get; private set; }
    }
}
