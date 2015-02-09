using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Infrastructure
{
    /// <summary>
    /// Abstract FTP methods factory creator
    /// </summary>
    public abstract class AbstractFtpMethodCreator
    {
        /// <summary>
        /// Creates class T
        /// </summary>
        /// <returns>Instance of class T</returns>
        public abstract AbstractFtpMethod Create();
    }
}
