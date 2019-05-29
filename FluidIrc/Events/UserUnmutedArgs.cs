using IrcDotNet;

namespace FluidIrc.Events
{
    public class UserUnmutedArgs
    {
        public UserUnmutedArgs(IrcUser user)
        {
            User = user;
        }

        public IrcUser User { get; }

    }
}
