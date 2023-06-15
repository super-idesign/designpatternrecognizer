﻿using System;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Console = System.Console;


namespace PatternPal.Tests.New_TestCasesRecognizers.Bridge
{

    //This test is a possible "perfect" bridge implementation.
    /* Pattern:              Bridge
     * Original code source:
     *
     * Requirements to fullfill the pattern:
     *         Implementation interface or abstract class:
     *            ✓  a) is an interface or abstract class <br/>
     *            ✓  b) has at least one (abstract) method <br/>
     *         Abstraction class:
     *            ✓  a) has a private/protected field or property with the type of the Implementation interface or abstract class
     *            ✓  b) has a method
     *            ✓  c) has a method that calls a method in the Implementation interface or abstract class
     *            ✓  d) has either,
     *            ✓     1) the property option as described in a, or has
     *                  2) a constructor with a parameter with the Implementation type and that uses the field as described in a, or has
     *                  3) a method with a parameter with the Implementation type and that uses the field as described in a
     *         Concrete Implementation
     *            ✓  a) is an implementation of the Implementation interface or inherits from the 'Implementation' abstract class
     *               b) if Implementation is an abstract class it should override it's abstract methods
     *         Refined Abstraction:
     *            ✓  a) inherits from the Abstraction class
     *            ✓  b) has an method
     *         Client class: 
     *            ✓  a) uses a method in the Abstraction class
     *            ✓  b) creates a Concrete Implementation instance
     *            ✓  c) sets the field or property in Abstraction, either through
     *            ✓     1) it is a property and it sets this, or through
     *                  2) a constructor as described in Abstraction d2
     *                  3) a method as described in Abstraction d3
     */

    // Implementation class
    file interface Color
    {
        internal void paint();
        internal void draw();
    }

    // Abstraction class
    file class Shape
    {
        protected internal Color _color { set; get; }

        internal Shape(){}

        internal void paintColor()
        {
            _color.paint();
        }

    }

    // Concrete implementation
    file class Red : Color
    {
        public void paint()
        {
            Console.WriteLine("Paint with Red");
        }
        public void draw()
        {

            Console.WriteLine("Draw with Red");
        }
    }

    // Refined Abstraction
    file class Circle : Shape
    {
        internal Circle ()
        { }

        internal void drawColor()
        {
            this._color.draw();
        }
    }

    // Client class
    file class Client
    {
        internal Client()
        {
            this.goPaint();
            Circle circle = new Circle
            {
                _color = new Red()
            };
            circle.drawColor();
        }

        internal void goPaint()
        {
            Shape shape = new Shape();
            shape._color = new Red();
            shape.paintColor();
            
        }


    }
}
