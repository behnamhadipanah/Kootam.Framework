using System.Collections.Concurrent;
using FluentValidation.Results;

namespace Kootam.Cqrs.Exceptions;

public class ValidationException : ApplicationException
{
    public IDictionary<string, string[]> Errors { get; set; }
    public ValidationException() : base("one or more validation failures have occurred")
    {
        Errors = new ConcurrentDictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures.GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(failuresGroup => failuresGroup.Key, failuresGroup => failuresGroup.ToArray());
    }
}