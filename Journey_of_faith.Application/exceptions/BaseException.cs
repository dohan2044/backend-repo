using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;

namespace Journey_of_faith.Application.exceptions
{
    public class BaseException : Exception 
    {
        public HttpStatusCode StatusCode;
        public BaseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    }

    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
    }

    public class ConfictException : BaseException
    {
        public ConfictException(string message) : base(HttpStatusCode.Conflict, message) { }
    }

    public class UnauthorizationException : BaseException
    {
        public UnauthorizationException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }

    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message) { }
    }

    public class UnprocessableEntityException : BaseException
    {
        public IDictionary<string, string[]> Errors { get; }
        public UnprocessableEntityException(string message) : base(HttpStatusCode.UnprocessableEntity, message) { }
        public UnprocessableEntityException(string message, IDictionary<string, string[]> errors) : base(HttpStatusCode.UnprocessableEntity, message)
        {
            Errors = errors;
        }
    }
}
