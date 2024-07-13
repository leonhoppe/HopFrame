namespace HopFrame.Api.Models;

public struct SingleValueResult<T>(T value) {
    public T Value { get; set; } = value;

    public static implicit operator T(SingleValueResult<T> v) {
        return v.Value;
    }

    public static implicit operator SingleValueResult<T>(T v) {
        return new SingleValueResult<T>(v);
    }
}