﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.Decorator.DecoratorTestCase1
{
    public class ConcreteComponent : IComponent
    {
        public ConcreteComponent() { }

        public int Operation()
        {
            return 1;
        }
    }
}