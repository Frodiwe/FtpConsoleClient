using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Infrastructure
{
    public abstract class AbstractFtpMethodCreator
    {
        public abstract AbstractFtpMethod Create();
    }
}
