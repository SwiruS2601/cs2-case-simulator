using System.Net;

namespace Cs2CaseOpener.Exceptions;

public class DomainException : Exception
{

    public HttpStatusCode Status { get; set; }
    public string Error { get; set; } = nameof(DomainException);
    public DomainException(string message, HttpStatusCode status = HttpStatusCode.NotFound): base(message)
    {
        Status = status;
    }
}
