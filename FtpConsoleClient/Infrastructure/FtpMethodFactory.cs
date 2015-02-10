using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Infrastructure
{
    /// <summary>
    /// Factory of FTP methods. Typed to AbstractFtpMethod class
    /// </summary>
    public class FtpMethodFactory
    {
        /// <value name="factoryStorage">
        /// Storage of all factory products (read: classes)
        /// </value>       
        protected Dictionary<string, AbstractFtpMethodCreator> factoryStorage = new Dictionary<string,AbstractFtpMethodCreator>();

        /// <summary>
        /// Adds new class inherited from AbstractFtpMethod to factory storage
        /// by FtpMethodCreater - implementation of AbstractFtpMethodCreator.
        /// </summary>
        /// <typeparam name="T">Name of class we add to storage</typeparam>
        /// <param name="alias">Alias of this class to add in associative array (read: factory storage)</param>
        public void Register<T>(string alias) where T : AbstractFtpMethod, new()
        {
            if (!factoryStorage.ContainsKey(alias))
                factoryStorage.Add(alias, new FtpMethodCreator<T>());
        }

        /// <summary>
        /// Creates new object of type appropriates key 
        /// </summary>
        /// <param name="alias">Alias we use to create needed class</param>
        /// <returns>Implementation of class with this alias</returns>
        public AbstractFtpMethod GetInstance(string alias)
        {
            if (factoryStorage.ContainsKey(alias))
                return factoryStorage[alias].Create();
            else
                return null;
        }
    }
}
