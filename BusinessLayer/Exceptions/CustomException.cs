using System;

namespace BusinessLayer.Exceptions
{
    public class CustomException : Exception
    {
        public const string DefaultMessage = "An error occurred in the CompanyService.";

        public CustomException(string message = DefaultMessage, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }

    public class DatabaseException : Exception
    {
        public const string DefaultMessage = "A database error occurred.";

        public DatabaseException(string message = DefaultMessage, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }


    public class NotFoundException : Exception
    {
        public const string DefaultMessage = "The requested resource was not found.";

        public NotFoundException(string message = DefaultMessage)
            : base(message)
        {
        }
    }

    public class ConflictException : Exception
    {
        public const string DefaultMessage = "A conflict occurred with the existing resource.";

        public ConflictException(string message = DefaultMessage)
            : base(message)
        {
        }
    }
}
