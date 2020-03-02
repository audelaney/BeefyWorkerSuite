using System;

namespace AppConfig
{
    public interface IConfigWatcher
    {
        /// <summary>
        /// Notifies the object watching the config of changes
        /// </summary>
        void Notify();
    }
}