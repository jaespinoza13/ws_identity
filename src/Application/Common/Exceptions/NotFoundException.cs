namespace Application.Common.Exceptions;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly
public class NotFoundException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
{
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
