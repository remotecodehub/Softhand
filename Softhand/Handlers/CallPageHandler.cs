using System;
namespace Softhand.Handlers
{
	public partial class CallPageHandler
	{
        public static void Handle()
        {
            PageHandle();
        }
        public static partial void PageHandle();
        public static partial void PageHandle()
        {
#if __IOS__
            Softhand.Platforms.iOS.CallPageHandler.PageHandle();
#endif
        }
    }
}

