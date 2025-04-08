
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Project_2025_Web.Services
    {
        public class Response<T>
        {
            public bool IsSucess { get; set; }
            public string? Message { get; set; }
            public List<string>? Errors { get; set; }
            public T? Result { get; set; }

        }
    }
