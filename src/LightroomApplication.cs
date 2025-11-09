namespace Loupedeck.LightroomPlugin
{
    using System;


    public class LightroomApplication : ClientApplication
    {
        public LightroomApplication()
        {
        }

        protected override String GetProcessName() => "";

        protected override String GetBundleName() => "";

        public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Unknown;
    }
}
