using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Infrastructure
{
    public class FtpMethodFactory
    {
        protected Dictionary<string, AbstractFtpMethodCreator> factoryStorage;

        public void Add<T>(string implementationId) where T : AbstractFtpMethod, new()
        {
            if (!factoryStorage.ContainsKey(implementationId))
                factoryStorage.Add(implementationId, new FtpMethodCreator<T>());
        }

        public AbstractFtpMethod Create(string implementationId)
        {
            if (factoryStorage.ContainsKey(implementationId))
                return factoryStorage[implementationId].Create();
            else
                return null;
        }
    }
}
