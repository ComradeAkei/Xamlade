using System;
using System.Linq;
using Metalama.Framework;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Fabrics;

namespace Xamlade.Extensions.Atributes;

internal class TESTAttribure : TypeAspect
{
    [Introduce]
    public static int TEST { get; set; }
}

[TESTAttribure]
public class A
{
    
}