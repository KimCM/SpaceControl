using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacksaar.SpaceControl.WP80.Space
{
    public interface IShell
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
        ConnectInfo RemoteConfig { set; }

        string ExecuteCommand(string command);
    }
}
