using System;

namespace SimpleBank.Application.Exceptions
{
    public class CustomerAccountNotFoundException : Exception
    {
        public CustomerAccountNotFoundException()
        {
        }

        public CustomerAccountNotFoundException(string message)
            : base(message)
        {
        }

        public CustomerAccountNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
