using Softhand.Domain.Models;
using System;
namespace Softhand.Domain.Abstractions;

/* Interface to separate UI & engine a bit better */
public interface ISoftMonitor
{
    void notifyRegState(int code, string reason, long expiration);
    void notifyIncomingCall(SoftCall call);
    void notifyCallState(SoftCall call);
    void notifyCallMediaState(SoftCall call);
    void notifyBuddyState(SoftBuddy buddy);
    void notifyChangeNetwork();
}

