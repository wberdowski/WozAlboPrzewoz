using System;
using System.Net;

namespace WozAlboPrzewoz
{
    public class WebExceptionEventArgs : EventArgs
    {
        public WebException Exception { get; set; }

        public WebExceptionEventArgs()
        {

        }

        public WebExceptionEventArgs(WebException exception)
        {
            Exception = exception;
        }
    }
}