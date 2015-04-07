using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningLibrary
{
    public enum VirtualMachineAction {Stop=0, Start = 1};
    public enum ResourceAction {Undefined=-1, Stop = 0, Start = 1 , Create =2, Delete=3};
}
