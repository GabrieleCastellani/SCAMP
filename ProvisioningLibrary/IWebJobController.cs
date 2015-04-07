using System;

namespace ProvisioningLibrary
{
    public interface IWebJobController
    {
        Guid SubmitActionInQueue(string  resourceId, ProvisioningLibrary.ResourceAction  action);
        void SubmitActionInQueue(string resourceId, string actionname);
    }
}