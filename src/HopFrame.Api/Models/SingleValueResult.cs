namespace HopFrame.Api.Models;

/// <summary>
/// Useful for endpoints that only return a single int or string
/// </summary>
/// <param name="value">The value of the result</param>
/// <typeparam name="TValue">The type of the result</typeparam>
public struct SingleValueResult<TValue>(TValue value) {
    public TValue Value { get; set; } = value;

    public static implicit operator TValue(SingleValueResult<TValue> v) {
        return v.Value;
    }

    public static implicit operator SingleValueResult<TValue>(TValue v) {
        return new SingleValueResult<TValue>(v);
    }
}