using System;

namespace BusinessLayer.Exceptions
{
    public class DatabaseException : Exception
    {
        public const string DefaultMessage = "A database error occurred.";
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public const string DefaultMessage = "A conflict occurred while processing the request.";
        public ConflictException(string message) : base(message) { }
    }

    public class CompanyServiceException : Exception
    {
        public const string DefaultMessage = "An error occurred in the Company Service.";
        public CompanyServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
