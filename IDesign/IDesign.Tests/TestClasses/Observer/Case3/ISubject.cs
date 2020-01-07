﻿namespace IDesign.Tests.TestClasses.Observer.Case3
{
    interface ISubject
    {
        void Add(IObserver observer);
        void Remove(IObserver observer);
        void Notify();
    }
}
