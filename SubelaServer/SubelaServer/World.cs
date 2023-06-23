using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubelaServer
{
    public class World
    {
        public static World Instance;

        public void Init() 
        { 
            Instance = this;
            Network.Instance.ServerStart();
        }
    }
}
