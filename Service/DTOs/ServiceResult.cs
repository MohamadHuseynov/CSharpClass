// Defines the namespace for Data Transfer Objects (DTOs) and related utility classes.
namespace Service.DTOs
{
    /// <summary>
    /// Represents the outcome of a service operation.
    /// It indicates whether the operation was successful and includes an optional message.
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// Gets a value indicating whether the service operation was successful.
        /// True if successful; otherwise, false.
        /// The setter is protected to control modification from outside the class hierarchy.
        /// </summary>
        public bool IsSuccess { get; protected set; }

        /// <summary>
        /// Gets a message providing details about the outcome of the service operation.
        /// This can be a success message or an error message.
        /// The setter is protected.
        /// </summary>
        public string Message { get; protected set; }

        // Static factory methods for creating ServiceResult instances.

        /// <summary>
        /// Creates a new ServiceResult instance representing a successful operation.
        /// </summary>
        /// <param name="message">An optional success message. If not provided, a default message is used.</param>
        /// <returns>A ServiceResult object with IsSuccess set to true.</returns>
        public static ServiceResult Success(string message = "Operation completed successfully.") // Default message changed to English
        {
            return new ServiceResult { IsSuccess = true, Message = message };
        }

        /// <summary>
        /// Creates a new ServiceResult instance representing a failed operation.
        /// </summary>
        /// <param name="message">A message describing the failure.</param>
        /// <returns>A ServiceResult object with IsSuccess set to false.</returns>
        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { IsSuccess = false, Message = message };
        }
    }

    /// <summary>
    /// Represents the outcome of a service operation that returns data.
    /// It extends ServiceResult to include a generic Data property for the operation's payload.
    /// </summary>
    /// <typeparam name="T">The type of data returned by the service operation.</typeparam>
    public class ServiceResult<T> : ServiceResult
    {
        /// <summary>
        /// Gets the data payload of a successful service operation.
        /// If the operation was not successful, this will typically be the default value for type T.
        /// The setter is private to ensure data is only set upon creation of a successful result.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Creates a new ServiceResult<T> instance representing a successful operation with data.
        /// </summary>
        /// <param name="data">The data payload returned by the operation.</param>
        /// <param name="message">An optional success message. If not provided, a default message is used.</param>
        /// <returns>A ServiceResult<T> object with IsSuccess set to true and Data populated.</returns>
        public static ServiceResult<T> Success(T data, string message = "Operation completed successfully.") // Default message changed to English
        {
            // Properties IsSuccess and Message are set by the base class logic,
            // but here we explicitly create ServiceResult<T> which inherits and can set them.
            return new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };
        }

        /// <summary>
        /// Creates a new ServiceResult<T> instance representing a failed operation.
        /// This 'new' static method hides the base class 'Fail' method to provide a typed result.
        /// </summary>
        /// <param name="message">A message describing the failure.</param>
        /// <returns>A ServiceResult<T> object with IsSuccess set to false and Data set to default(T).</returns>
        public new static ServiceResult<T> Fail(string message) // Using 'new' to hide the base ServiceResult.Fail if desired
        {
            // For a failed operation, Data is typically set to its default value (e.g., null for reference types).
            return new ServiceResult<T> { IsSuccess = false, Message = message, Data = default(T) };
        }
    }
}