using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace NightFish.Restart
{
    public class RestartConfiguration : IRocketPluginConfiguration
    {
        public bool RestartOnShutdown = true;

        public void LoadDefaults()
        {
            RestartOnShutdown = true;
        }
    }
}
