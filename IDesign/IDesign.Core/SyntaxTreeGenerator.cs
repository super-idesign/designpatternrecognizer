﻿using System.Collections.Generic;
using IDesign.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Core
{
    public class SyntaxTreeGenerator
    {
        public List<ClassDeclarationSyntax> ClassDeclarationSyntaxList = new List<ClassDeclarationSyntax>();
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntaxList;
        public List<EntityNode> EntityNodes = new List<EntityNode>();
        public List<FieldDeclarationSyntax> FieldDeclarationSyntaxList;
        public List<InterfaceDeclarationSyntax> InterfaceDeclarationSyntaxList = new List<InterfaceDeclarationSyntax>();
        public List<MethodDeclarationSyntax> MethodDeclarationSyntaxList;
        public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxList;

        public List<UsingDirectiveSyntax> UsingDirectiveSyntaxList = new List<UsingDirectiveSyntax>();


        /// <summary>
        ///     Constructor of the SyntaxTreeGenerator class.
        ///     Genarates a syntaxtree from a string
        /// </summary>
        /// <param name="content"></param>s
        /// <param name="source"></param>
            /// <param name="entityNodes"></param>
        public SyntaxTreeGenerator(string content, string source, Dictionary<string, EntityNode> entityNodes)

        {
            File = source;
            Tree = CSharpSyntaxTree.ParseText(content);
            Root = Tree.GetCompilationUnitRoot();
            GetUsingsOfFile();
            GetAllClassesOfFile(entityNodes);
            GetAllInterfacesOfFile(entityNodes);
            GetAllConstructorsOfAClass(entityNodes);
            GetAllMethodsOfAClass(entityNodes);
            GetAllPropertiesOfAClass(entityNodes);
            GetAllFieldsOfAClass(entityNodes);
        }

        public string File { get; set; }
        public CompilationUnitSyntax Root { get; set; }
        public SyntaxTree Tree { get; }

        /// <summary>
        ///     Adds all usings of the syntaxtree to a list
        /// </summary>
        /// <returns></returns>
        private List<UsingDirectiveSyntax> GetUsingsOfFile()
        {
            foreach (var element in Root.Usings) UsingDirectiveSyntaxList.Add(element);
            return UsingDirectiveSyntaxList;
        }

        /// <summary>
        ///    Parent function of the recursive function that adds all ClassNodes of the file to the ClassDeclarationSyntaxList
        /// </summary>
        /// <param name="entityNodes"></param>
        private void GetAllClassesOfFile(Dictionary<string, EntityNode> entityNodes)
        {
            if (Root.Members != null)
                foreach (var member in Root.Members)
                    GetAllClassesOfFile(member, entityNodes);
        }

        /// <summary>
        ///     Recursive function that adds all ClassNodes of the file to the ClassDeclarationSyntaxList
        /// </summary>
        /// <param name="node"></param>
        /// <param name="entityNodes"></param>
        /// <returns>Returns a List with ClassDeclarationSyntaxes</returns>
        private List<ClassDeclarationSyntax> GetAllClassesOfFile(SyntaxNode node,
            Dictionary<string, EntityNode> entityNodes)
        {
            if (node.Kind() == SyntaxKind.ClassDeclaration)
            {
                var classNode = (ClassDeclarationSyntax) node;
                ClassDeclarationSyntaxList.Add(classNode);
                var nameSpaceKey = "";
                var nameSpace = classNode.Parent as NamespaceDeclarationSyntax;
                if (nameSpace != null) nameSpaceKey += nameSpace.Name.ToString();

                var keybinding = nameSpaceKey + "." + classNode.Identifier;
                if (!entityNodes.ContainsKey(keybinding))
                {
                    var entityNode = new EntityNode
                    {
                        InterfaceOrClassNode = classNode,
                        Name = classNode.Identifier.ToString(),
                        UsingDeclarationSyntaxList = new List<UsingDirectiveSyntax>(UsingDirectiveSyntaxList),
                        SourceFile = File,
                        NameSpace = nameSpaceKey
                    };
                    entityNodes.Add(keybinding, entityNode);
                }
            }

            if (node.ChildNodes() != null)
                foreach (var childNode in node.ChildNodes())
                    GetAllClassesOfFile(childNode, entityNodes);
            return ClassDeclarationSyntaxList;
        }

        /// <summary>
        ///     Parent function of the recursive function that adds all InterfaceNodes of the file to the
        /// </summary>
        /// <param name="entityNodes"></param>
        private void GetAllInterfacesOfFile(Dictionary<string, EntityNode> entityNodes)
        {
            if (Root.Members != null)
                foreach (var member in Root.Members)
                    GetAllInterfacesOfFile(member, entityNodes);
        }

        /// <summary>
        ///     Recursive function that adds all InterfaceNodes of the file to the InterfaceDeclarationSyntaxList
        /// </summary>
        /// <param name="node"></param>
        /// <param name="entityNodes"></param>
        /// <returns>
        ///     Returns a List with InterfaceDeclarationSyntaxes
        /// </returns>
        private List<InterfaceDeclarationSyntax> GetAllInterfacesOfFile(SyntaxNode node,
            Dictionary<string, EntityNode> entityNodes)
        {
            if (node.Kind() == SyntaxKind.InterfaceDeclaration)
            {
                var interfaceNode = (InterfaceDeclarationSyntax) node;
                InterfaceDeclarationSyntaxList.Add(interfaceNode);
                var nameSpaceKey = "";
                var nameSpace = interfaceNode.Parent as NamespaceDeclarationSyntax;
                if (nameSpace != null) nameSpaceKey += nameSpace.Name.ToString();

                var keybinding = nameSpaceKey + "." + interfaceNode.Identifier;
                if (!entityNodes.ContainsKey(keybinding))
                {
                    var entityNode = new EntityNode
                    {
                        InterfaceOrClassNode = interfaceNode,
                        Name = interfaceNode.Identifier.ToString(),
                        UsingDeclarationSyntaxList = new List<UsingDirectiveSyntax>(UsingDirectiveSyntaxList),
                        SourceFile = File,
                        NameSpace = nameSpaceKey
                    };
                    entityNodes.Add(keybinding, entityNode);
                }
            }

            if (node.ChildNodes() != null)
                foreach (var childNode in node.ChildNodes())
                    GetAllInterfacesOfFile(childNode, entityNodes);
            return InterfaceDeclarationSyntaxList;
        }

        /// <summary>
        ///     Parent function of the recursive function that searches for all constructors in a class
        /// </summary>
        /// <param name="entityNodes"></param>
        private void GetAllConstructorsOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
                foreach (var classElement in entityNodes)
                    classElement.Value.ConstructorDeclarationSyntaxList =
                        GetAllConstructorsOfAClass(classElement.Value.InterfaceOrClassNode);
        }

        /// <summary>
        ///     Recursive function that searches for all constructors in a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        ///     Returns a list with ConstructorDeclarationSyntaxes
        /// </returns>
        private List<ConstructorDeclarationSyntax> GetAllConstructorsOfAClass(SyntaxNode node)
        {
            ConstructorDeclarationSyntaxList = new List<ConstructorDeclarationSyntax>();
            if (node.ChildNodes() != null)
                foreach (var childNode in node.ChildNodes())
                    if (childNode.Kind() == SyntaxKind.ConstructorDeclaration)
                    {
                        var constructorNode = (ConstructorDeclarationSyntax) childNode;
                        ConstructorDeclarationSyntaxList.Add(constructorNode);
                    }

            return ConstructorDeclarationSyntaxList;
        }

        /// <summary>
        ///     Parent of recursive function that searches for all methodes of a class
        /// </summary>
        /// <param name="entityNodes"></param>
        private void GetAllMethodsOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
                foreach (var classElement in entityNodes)
                    classElement.Value.MethodDeclarationSyntaxList =
                        GetAllMethodsOfAClass(classElement.Value.InterfaceOrClassNode);
        }

        /// <summary>
        ///     Recursive function that searches for all methodes of a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        ///     Returns a list with MethodDeclarationSynates
        /// </returns>
        private List<MethodDeclarationSyntax> GetAllMethodsOfAClass(SyntaxNode node)
        {
            MethodDeclarationSyntaxList = new List<MethodDeclarationSyntax>();
            if (node.ChildNodes() != null)
                foreach (var childeNode in node.ChildNodes())
                    if (childeNode.Kind() == SyntaxKind.MethodDeclaration)
                    {
                        var methodNode = (MethodDeclarationSyntax) childeNode;
                        MethodDeclarationSyntaxList.Add(methodNode);
                    }

            return MethodDeclarationSyntaxList;
        }

        /// <summary>
        ///     Parent of recursive function that searches for all properties of a class
        /// </summary>
        /// <param name="entityNodes"></param>
        private void GetAllPropertiesOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
                foreach (var classElement in entityNodes)
                    classElement.Value.PropertyDeclarationSyntaxList =
                        GetAllPropertiesOfAClass(classElement.Value.InterfaceOrClassNode);
        }

        /// <summary>
        ///     Recursive function that searches for all properties of a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        ///     Returns a list with PropertyDeclarationSyntax
        /// </returns>
        private List<PropertyDeclarationSyntax> GetAllPropertiesOfAClass(SyntaxNode node)
        {
            PropertyDeclarationSyntaxList = new List<PropertyDeclarationSyntax>();
            if (node.ChildNodes() != null)
                foreach (var childNode in node.ChildNodes())
                    if (childNode.Kind() == SyntaxKind.PropertyDeclaration)
                    {
                        var propertyNode = (PropertyDeclarationSyntax) childNode;
                        PropertyDeclarationSyntaxList.Add(propertyNode);
                    }
            return PropertyDeclarationSyntaxList;
        }

        /// <summary>
        ///     Parent of recursive function that searches for all fields of a class
        /// </summary>
        /// <param name="entityNodes"></param>
        private void GetAllFieldsOfAClass(Dictionary<string, EntityNode> entityNodes)
        {
            if (EntityNodes != null)
                foreach (var classElement in entityNodes)
                    classElement.Value.FieldDeclarationSyntaxList =
                        GetAllFieldsOfAClass(classElement.Value.InterfaceOrClassNode);
        }

        /// <summary>
        ///     Recursive function that searches for all fields of a class
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        ///     Returns a list with FieldDeclarationSyntax
        /// </returns>
        private List<FieldDeclarationSyntax> GetAllFieldsOfAClass(SyntaxNode node)
        {
            FieldDeclarationSyntaxList = new List<FieldDeclarationSyntax>();
            if (node.ChildNodes() != null)
                foreach (var childNode in node.ChildNodes())
                    if (childNode.Kind() == SyntaxKind.FieldDeclaration)
                    {
                        var fieldNode = (FieldDeclarationSyntax) childNode;
                        FieldDeclarationSyntaxList.Add(fieldNode);
                    }

            return FieldDeclarationSyntaxList;
        }
    }
}