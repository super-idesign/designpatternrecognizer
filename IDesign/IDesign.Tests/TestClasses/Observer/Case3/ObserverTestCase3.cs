﻿namespace IDesign.Tests.TestClasses.Observer.Case3
{
    class ObserverTestCase3
    {
        SubjectA subject;

        public ObserverTestCase3()
        {
            subject = new SubjectA();
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
            subject.Add(new ObserverA());
        }
    }
}