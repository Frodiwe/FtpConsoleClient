using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Infrastructure
{
    /// <summary>
    /// Concrete FTP method creator
    /// </summary>
    /// <typeparam name="T">Name of class we create</typeparam>
    class FtpMethodCreator<T> : AbstractFtpMethodCreator where T: AbstractFtpMethod, new()
    {
        public override Methods.AbstractFtpMethod Create() { return new T(); }
    }
}
