using System;

namespace SilentPackage.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class NotFoundViewModel
    {
        public NotFoundViewModel(string requestId, string error)
        {
            RequestId = requestId;
            Error = error;
        }

        public string RequestId { get; set; }
        public string Error { get; set; }
    }
}

