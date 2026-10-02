using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Journey_of_faith.Domain.exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
