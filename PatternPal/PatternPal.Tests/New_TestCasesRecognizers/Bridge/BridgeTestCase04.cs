﻿using System;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Console = System.Console;


namespace PatternPal.Tests.New_TestCasesRecognizers.Bridge
{

    //This test is a possible bridge implementation.
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
     *         Concrete Implementation
     *            ✓  a) is an implementation of the Implementation interface or inherits from the 'Implementation' abstract class
     *               b) if Implementation is an abstract class it should override it's abstract methods
     *         Refined Abstraction:
     *               a) inherits from the Abstraction class
     *               b) has an method
     *         Client class: 
     *            ✓  a) uses a method in the Abstraction class
     *               b) creates a Concrete Implementation instance
     *               c) uses the field or property in Abstraction
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
        private Color _color;
        internal Shape(Color color)
        {
            _color = color;
        }

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
    /*
    file class Circle : Shape
    {
        internal Circle(Color color) : base(color)
        { }

        internal void drawColor()
        {
            this._color.draw();
        }
    }
    */

    // Client class
    file class Client
    {
        internal Client()
        {
            Shape shape = new Shape();
            shape.paintColor();
        }
    }
}
