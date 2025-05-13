using System.Runtime.Serialization;

namespace TableauLambda.Validators.Exceptions
{

    [Serializable]
    public class ValidationFailedException : SystemException
    {
        public List<ValidatorError> Errors { get; }

        public ValidationFailedException(List<ValidatorError> errors)
        {
            Errors = errors;
        }

        public ValidationFailedException(ValidatorError error)
        {
            Errors = new List<ValidatorError> { error };
        }

        public ValidationFailedException()
        {
        }

        public ValidationFailedException(string message) : base(message)
        {
        }

        public ValidationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValidationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
