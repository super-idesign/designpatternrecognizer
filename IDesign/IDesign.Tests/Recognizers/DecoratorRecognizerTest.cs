﻿using IDesign.Core;
using IDesign.Recognizers;
using IDesign.Tests.Utils;
using NUnit.Framework;

namespace IDesign.Tests.Recognizers
{
    public class DecoratorRecognizerTest
    {
        [TestCase("DecoratorTest1", "Decorator", 80, 100)]
        [TestCase("DecoratorTest2", "Decorator", 0, 79)]
        [TestCase("DecoratorTest3", "Decorator", 0, 79)]
        [TestCase("DecoratorTest4", "Decorator", 0, 79)]
        [TestCase("DecoratorTest5", "Decorator", 0, 79)]
        public void DecoratorRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        )
        {
            var decorator = new DecoratorRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"IDesign.Tests.TestClasses.{directory}";
            var entityNodes = EntityNodeUtils.CreateEntityNodeGraph(filesAsString);
            var createRelation = new DetermineRelations(entityNodes);
            createRelation.CreateEdgesOfEntityNode();
            var result = decorator.Recognize(entityNodes[nameSpaceName + "." + filename]);

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }
    }
}
