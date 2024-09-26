using System.Net;

namespace HopFrame.Api.Logic;

public interface ILogicResult {
    HttpStatusCode State { get; set; }

    string Message { get; set; }

    bool IsSuccessful { get; }
}

public interface ILogicResult<T> {
    HttpStatusCode State { get; set; }

    T Data { get; set; }

    string Message { get; set; }

    bool IsSuccessful { get; }
}