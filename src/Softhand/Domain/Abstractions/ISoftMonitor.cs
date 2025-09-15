using Softhand.Domain.Models;
using System;
namespace Softhand.Domain.Abstractions;

/* Interface to separate UI & engine a bit better */
public interface ISoftMonitor
{
    void NotifyRegState(int code, string reason, long expiration);
    void NotifyIncomingCall(SoftCall call);
    void NotifyCallState(SoftCall call);
    void NotifyCallMediaState(SoftCall call);
    void NotifyBuddyState(SoftBuddy buddy);
    void NotifyChangeNetwork();
    void NotifyOnBuddyEvSubState(SoftBuddy buddy, OnBuddyEvSubStateParam param);
}

