using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softhand.Infrastructure.Services.Concrete;

public class NavigationContextService<T> : INavigationContextService<T> where T : class
{
    public T Payload { get; set; }
}
