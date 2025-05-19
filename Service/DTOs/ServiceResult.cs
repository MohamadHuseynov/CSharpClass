namespace Service.DTOs
{
    public class ServiceResult
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; }

        // سازنده‌های کمکی برای راحتی
        public static ServiceResult Success(string message = "عملیات با موفقیت انجام شد.")
        {
            return new ServiceResult { IsSuccess = true, Message = message };
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { IsSuccess = false, Message = message };
        }
    }

    // نسخه Generic برای زمانی که می‌خواهید داده‌ای هم همراه نتیجه برگردانید
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; private set; }

        public static ServiceResult<T> Success(T data, string message = "عملیات با موفقیت انجام شد.")
        {
            return new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };
        }

        // متد Fail را از کلاس پایه به ارث می‌برد یا می‌توانید بازنویسی کنید
        public new static ServiceResult<T> Fail(string message) // "new" برای پنهان کردن متد پایه
        {
            return new ServiceResult<T> { IsSuccess = false, Message = message, Data = default(T) };
        }
    }
}
