using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace HopFrame.Api.Logic;

public class LogicResult : ILogicResult {
    public HttpStatusCode State { get; set; }

    public string Message { get; set; }

    public bool IsSuccessful => State == HttpStatusCode.OK;

    public static LogicResult Ok() {
        return new LogicResult() {
            State = HttpStatusCode.OK
        };
    }

    public static LogicResult BadRequest() {
        return new LogicResult() {
            State = HttpStatusCode.BadRequest
        };
    }

    public static LogicResult BadRequest(string message) {
        return new LogicResult() {
            State = HttpStatusCode.BadRequest,
            Message = message
        };
    }

    public static LogicResult Forbidden() {
        return new LogicResult() {
            State = HttpStatusCode.Forbidden
        };
    }

    public static LogicResult Forbidden(string message) {
        return new LogicResult() {
            State = HttpStatusCode.Forbidden,
            Message = message
        };
    }

    public static LogicResult NotFound() {
        return new LogicResult() {
            State = HttpStatusCode.NotFound
        };
    }

    public static LogicResult NotFound(string message) {
        return new LogicResult() {
            State = HttpStatusCode.NotFound,
            Message = message
        };
    }

    public static LogicResult Conflict() {
        return new LogicResult() {
            State = HttpStatusCode.Conflict
        };
    }

    public static LogicResult Conflict(string message) {
        return new LogicResult() {
            State = HttpStatusCode.Conflict,
            Message = message
        };
    }

    public static LogicResult Forward(LogicResult result) {
        return new LogicResult() {
            State = result.State,
            Message = result.Message
        };
    }

    public static LogicResult Forward<T>(ILogicResult<T> result) {
        return new LogicResult() {
            State = result.State,
            Message = result.Message
        };
    }

    public static implicit operator ActionResult(LogicResult v) {
        if (v.State == HttpStatusCode.OK) return new OkResult();

        return new ObjectResult(v.Message) {
            StatusCode = (int)v.State
        };
    }
}

public class LogicResult<T> : ILogicResult<T> {
    public HttpStatusCode State { get; set; }

    public T Data { get; set; }

    public string Message { get; set; }

    public bool IsSuccessful => State == HttpStatusCode.OK;

    public static LogicResult<T> Ok() {
        return new LogicResult<T>() {
            State = HttpStatusCode.OK
        };
    }

    public static LogicResult<T> Ok(T result) {
        return new LogicResult<T>() {
            State = HttpStatusCode.OK,
            Data = result
        };
    }

    public static LogicResult<T> BadRequest() {
        return new LogicResult<T>() {
            State = HttpStatusCode.BadRequest
        };
    }

    public static LogicResult<T> BadRequest(string message) {
        return new LogicResult<T>() {
            State = HttpStatusCode.BadRequest,
            Message = message
        };
    }

    public static LogicResult<T> Forbidden() {
        return new LogicResult<T>() {
            State = HttpStatusCode.Forbidden
        };
    }

    public static LogicResult<T> Forbidden(string message) {
        return new LogicResult<T>() {
            State = HttpStatusCode.Forbidden,
            Message = message
        };
    }

    public static LogicResult<T> NotFound() {
        return new LogicResult<T>() {
            State = HttpStatusCode.NotFound
        };
    }

    public static LogicResult<T> NotFound(string message) {
        return new LogicResult<T>() {
            State = HttpStatusCode.NotFound,
            Message = message
        };
    }

    public static LogicResult<T> Conflict() {
        return new LogicResult<T>() {
            State = HttpStatusCode.Conflict
        };
    }

    public static LogicResult<T> Conflict(string message) {
        return new LogicResult<T>() {
            State = HttpStatusCode.Conflict,
            Message = message
        };
    }

    public static LogicResult<T> Forward(ILogicResult result) {
        return new LogicResult<T>() {
            State = result.State,
            Message = result.Message
        };
    }

    public static LogicResult<T> Forward<T2>(ILogicResult<T2> result) {
        return new LogicResult<T>() {
            State = result.State,
            Message = result.Message
        };
    }

    public static implicit operator ActionResult<T>(LogicResult<T> v) {
        if (v.State == HttpStatusCode.OK) return new OkObjectResult(v.Data);

        return new ObjectResult(v.Message) {
            StatusCode = (int)v.State
        };
    }
}