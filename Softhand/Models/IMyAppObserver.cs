using System;
namespace Softhand.Models;

/* Interface to separate UI & engine a bit better */
public interface MyAppObserver
{
    void notifyRegState(int code, String reason, long expiration);
    void notifyIncomingCall(MyCall call);
    void notifyCallState(MyCall call);
    void notifyCallMediaState(MyCall call);
    void notifyBuddyState(MyBuddy buddy);
    void notifyChangeNetwork();
}

