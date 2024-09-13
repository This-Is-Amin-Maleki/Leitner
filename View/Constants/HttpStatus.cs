namespace ViewLeit.Constants
{
    public struct HttpStatus
    {
        public int Code { get; }
        public string Description { get; }

        public HttpStatus(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public static HttpStatus FromCode(int code)
        {
            return code switch
            {
                400 => new HttpStatus(400, "Bad Request!"),
                401 => new HttpStatus(401, "Unauthorized Access!"),
                403 => new HttpStatus(403, "Forbidden! Access Denied!"),
                404 => new HttpStatus(404, "File Not Found!"),
                405 => new HttpStatus(405, "Method Not Allowed!"),
                408 => new HttpStatus(408, "Request Timeout!"),
                409 => new HttpStatus(409, "Conflict! Data Mismatch!"),
                410 => new HttpStatus(410, "Gone! Resource Unavailable!"),
                413 => new HttpStatus(413, "Payload Too Large!"),
                415 => new HttpStatus(415, "Unsupported Media Type!"),
                429 => new HttpStatus(429, "Too Many Requests! Please Try Again Later!"),
                500 => new HttpStatus(500, "Internal Server Error!"),
                501 => new HttpStatus(501, "Not Implemented! Feature Unavailable!"),
                502 => new HttpStatus(502, "Bad Gateway!"),
                503 => new HttpStatus(503, "Service Unavailable! Please Try Again Later!"),
                504 => new HttpStatus(504, "Gateway Timeout!"),
                505 => new HttpStatus(505, "HTTP Version Not Supported!"),
                _ => new HttpStatus(code, "Unknown Status")
            };
        }
    }

}
