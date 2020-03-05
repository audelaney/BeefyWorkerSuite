using System;
using AppConfig.Models;

namespace AppConfig.Accessors
{
    public interface IAppConfigAccessor
    {
        ConfigModel GetConfig();
    }
}