# LogicResults
LogicResults provide another layer of abstraction above the ActionResults.
They help you sending the right `HttpStatusCode` with the right data.

## Usage
1. Create an endpoint that returns an `ActionResult`:

    ```csharp
    [HttpGet("hello")]
    public ActionResult<string> Hello() {
        return new ActionResult<string>("Hello, World!");
    }
    ```
2. Now instead of directly returning the `ActionResult`, return a `LogicResult`

    ```csharp
    [HttpGet("hello")]
    public ActionResult<string> Hello() {
        return LogicResult<string>.Ok("Hello, World!");
    }
    ```
3. This allows you to very easily change the return type by simply calling the right function

    ```csharp
    [HttpGet("hello")]
    public ActionResult<string> Hello() {
        if (!Auth.IsLoggedIn)
            return LogicResult<string>.Forbidden();

        return LogicResult<string>.Ok("Hello, World!");
    }
    ```
    **Hint:** You can also provide an error message for status codes that are not in the 200 range.