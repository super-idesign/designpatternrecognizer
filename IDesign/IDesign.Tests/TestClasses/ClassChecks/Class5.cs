﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.ClassChecks
{
   public class Class5 : EClass5 { }

    public class EClass5 : E1Class5{ }

    public class E1Class5 : IClass5 { }
    
    public interface IClass5 { }
}
