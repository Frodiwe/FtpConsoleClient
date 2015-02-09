using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Infrastructure
{
    class FtpMethodCreator<T> : AbstractFtpMethodCreator where T: AbstractFtpMethod, new()
    {
        public override Methods.AbstractFtpMethod Create() { return new T(); }
    }
}
