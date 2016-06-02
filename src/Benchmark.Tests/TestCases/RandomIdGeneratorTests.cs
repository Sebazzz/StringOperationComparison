namespace Benchmark.Tests.TestCases {
    using System.Collections.Generic;
    using System.Diagnostics;
    using Benchmark.TestCases;
    using NUnit.Framework;
    
    [TestFixture]
    public sealed class RandomIdGeneratorTest_UsingUnsafePointers : RandomIdGeneratorTestBase {
        protected override string GetStringRepresentation(RandomIdGenerator idGenerator) {
            return idGenerator.UsingUnsafePointers();
        }
    }

    [TestFixture]
    public sealed class RandomIdGeneratorTest_UsingStringConcatenation : RandomIdGeneratorTestBase {
        protected override string GetStringRepresentation(RandomIdGenerator idGenerator) {
            return idGenerator.UsingStringConcatenation();
        }
    }

    [TestFixture]
    public sealed class RandomIdGeneratorTest_UsingStringBuilderWithPredefinedCapacity : RandomIdGeneratorTestBase {
        protected override string GetStringRepresentation(RandomIdGenerator idGenerator) {
            return idGenerator.UsingStringBuilderWithPredefinedCapacity();
        }
    }

    [TestFixture]
    public sealed class RandomIdGeneratorTest_UsingStringBuilderWithoutPredefinedCapacity : RandomIdGeneratorTestBase {
        protected override string GetStringRepresentation(RandomIdGenerator idGenerator) {
            return idGenerator.UsingStringBuilderWithoutPredefinedCapacity();
        }
    }

    public abstract class RandomIdGeneratorTestBase {
        protected abstract string GetStringRepresentation(RandomIdGenerator idGenerator);

        [Test]
        [Repeat(10)]
        public void LogMessageId_GeneratedUniqueId_Sequentially() {
            // Given
            var idGenerator1 = new RandomIdGenerator();
            var idGenerator2 = new RandomIdGenerator();

            var value1 = this.GetStringRepresentation(idGenerator1);
            var value2 = this.GetStringRepresentation(idGenerator2);

            // When / Then
            Assert.That(() => value1 != value2, Is.True, "{0} == {1}", value1, value2);
        }

        [Test]
        public void LogMessageId_Generates16To32CharString_ToString() {
            // Given
            var idGenerator = new RandomIdGenerator();

            // When
            var stringValue = this.GetStringRepresentation(idGenerator);

            // Then
            Assert.That(stringValue.Length, Is.GreaterThanOrEqualTo(16).And.LessThanOrEqualTo(32),
                "Generated string '{0}' not of expected length", stringValue);

            Debug.WriteLine(stringValue);
        }

        [Test]
        [Repeat(5)]
        public void LogMessageId_IsFairlyUnique() {
            // Given
            const int sampleSize = 1500;

            // When
            var duplicates = new List<string>();
            var all = new HashSet<string>();

            for (var i = 0; i < sampleSize; i++) {
                var current = this.GetStringRepresentation(new RandomIdGenerator());

                if (!all.Add(current)) {
                    duplicates.Add(current);
                }
            }

            // Then
            CollectionAssert.IsEmpty(duplicates);
        }

        [Test]
        [Repeat(5)]
        public void LogMessageId_StringRepresentation_Is32CharLowercaseAlpha() {
            // Given
            var idGenerator = new RandomIdGenerator();

            // When
            var value = this.GetStringRepresentation(idGenerator);

            // When / Then
            Assert.That(value, Does.Match("[a-z0-9]{16}"));
        }
    }
}