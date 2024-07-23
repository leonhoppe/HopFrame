namespace HopFrame.Api.Models;

public struct SingleValueResult<TValue>(TValue value) {
    public TValue Value { get; set; } = value;

    public static implicit operator TValue(SingleValueResult<TValue> v) {
        return v.Value;
    }

    public static implicit operator SingleValueResult<TValue>(TValue v) {
        return new SingleValueResult<TValue>(v);
    }
}