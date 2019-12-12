﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StrategyTest2
{
    //this code is from https://refactoring.guru/design-patterns/strategy/csharp/example

    class ConcreteStrategyB : IStrategy
    {
        public object DoAlgorithm(object data)
        {
            var list = data as List<string>;
            list.Sort();
            list.Reverse();

            return list;
        }
    }
}