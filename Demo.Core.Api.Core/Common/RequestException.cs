using System;

namespace Demo.Core.Api.Core.Common
{
    public class RequestException:Exception
    {
        public RequestException(string msg)
        {
            _ErrorMessage = msg;
        }

        public RequestException(int status, string msg)
        {
            _ErrorMessage = msg;
            _ErrorStatus = status;
        }
        public RequestException(string msg, Exception innerException)
        {
            _ErrorMessage = msg;
            this._InnerException = innerException;
        }
        string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
        }

        Exception _InnerException;
        public new Exception InnerException
        {
            get
            {
                return _InnerException;
            }
        }

        int _ErrorStatus= 0;
        public int ErrorStatus
        {
            get
            {
                return _ErrorStatus;
            }
        }
    } 
}