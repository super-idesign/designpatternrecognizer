﻿using System.Collections.Generic;

namespace IDesign.Tests.TestClasses.ObserverTest5
{
    internal class SubjectA : ISubject
    {
        private readonly List<IObserver> observers;

        public SubjectA()
        {
            observers = new List<IObserver>();
        }

        public void Add(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }
}
