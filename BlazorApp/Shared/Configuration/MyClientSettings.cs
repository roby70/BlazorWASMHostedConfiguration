using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared.Configuration;
public class MyClientSettings {
    public string HelloMessage { get; set; } = "Hello, world from MyClientSettings class default!";

    public string? Setting0 { get; set; } = "from MyClientSettings default";
    public string? Setting1 { get; set; } = null;
    public string? Setting2 { get; set; } = null;
    public string? Setting3 { get; set; } = null;
}
