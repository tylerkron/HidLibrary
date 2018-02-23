namespace HidLibrary
{
    public class HidAsyncState
    {
        public object CallerDelegate { get; }
        public object CallbackDelegate { get; }

        public HidAsyncState(object callerDelegate, object callbackDelegate)
        {
            CallerDelegate = callerDelegate;
            CallbackDelegate = callbackDelegate;
        }
    }
}
