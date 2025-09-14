global using CommunityToolkit.Mvvm.Messaging;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Mvvm.Messaging.Messages;

global using Microsoft.Extensions.Logging;
global using Microsoft.Maui.Controls.Handlers.Compatibility;
global using Microsoft.Maui.Controls.Platform; 


global using pjsua2maui.pjsua2;

global using Softhand.Application.Controls; 
global using Softhand.Application.ViewModels;
global using Softhand.Application.Views;
global using Softhand.Domain.Abstractions;
global using Softhand.Domain.Models;
global using Softhand.Infrastructure.Messages;
global using Softhand.Infrastructure.Services.Abstract;
global using Softhand.Infrastructure.Services.Concrete;

global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Runtime.CompilerServices;

#if __ANDROID__
global using Softhand.Platforms.Android;
global using System.Runtime.InteropServices;
global using Android.App;
global using Android.Content;
global using Android.Graphics;
global using Android.Runtime;
global using Android.Views;
global using Android.Widget;
#elif __IOS__
global using Softhand.Platforms.iOS;
global using UIKit;
#endif

 