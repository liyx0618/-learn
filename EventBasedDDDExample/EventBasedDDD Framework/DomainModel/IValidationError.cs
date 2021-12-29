using System.Collections.Generic;

namespace EventBasedDDD
{
    public interface IValidationError
    {
        IEnumerable<ValidationErrorItem> GetErrors();
        IValidationError AddError(string errorKey);
        IValidationError AddError(string errorKey, params object[] parameters);
        IValidationError AddError(string errorKey, IList<object> parameters);
        bool IsValid { get; }
    }
}
