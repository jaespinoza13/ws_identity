namespace Application.Common.Exceptions;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly
public class ForbiddenAccessException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
{
    public ForbiddenAccessException() : base() { }
}
