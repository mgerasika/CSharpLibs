namespace MvcEx
{
    public class SuccessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public SuccessResult()
        {
            this.Success = true;
        }
    }

    public class FailedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public FailedResult()
        {
            this.Success = false;
        }
    }
}
