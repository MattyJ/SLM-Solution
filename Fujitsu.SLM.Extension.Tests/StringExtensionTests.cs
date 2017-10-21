using System;
using System.Diagnostics.CodeAnalysis;
using Fujitsu.SLM.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Extension.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class StringExtensionTests
    {
        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsInt()
        {
            const string source = "1";
            var converted = source.ConvertTo<int>();
            Assert.AreEqual(1, converted);
        }

        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsString()
        {
            const string source = "X";
            var converted = source.ConvertTo<string>();
            Assert.AreEqual("X", converted);
        }

        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsDateTime()
        {
            const string source = "2013/10/02";
            var converted = source.ConvertTo<DateTime>();
            Assert.AreEqual(new DateTime(2013, 10, 2), converted);
        }

        [TestMethod]
        public void StringExtension_ConvertTo_ReturnsDateTimeForAmericanTypeDate()
        {
            const string source = "2013/10/12";
            var converted = source.ConvertTo<DateTime>();
            Assert.AreEqual(new DateTime(2013, 10, 12), converted);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StringExtension_ConvertTo_ReturnsExceptionForInt()
        {
            const string source = "X";
            var converted = source.ConvertTo<int>();
        }

        [TestMethod]
        public void StringExtension_ConvertStringToGenericValue_StringIsNullTypeIsInt_ReturnsDefaultIntWhichIsZero()
        {
            var s = null as string;
            Assert.AreEqual(0, s.ConvertStringToGenericValue<int>());
        }

        [TestMethod]
        public void StringExtension_ConvertStringToGenericValue_StringIsNullTypeIsDateTime_ReturnsDateCorrectlyParsed()
        {
            const string s = "06/12/2014 13:12:12";
            var actual = s.ConvertStringToGenericValue<DateTime>();
            Assert.AreEqual(2014, actual.Year);
            Assert.AreEqual(12, actual.Month);
            Assert.AreEqual(6, actual.Day);
            Assert.AreEqual(13, actual.Hour);
            Assert.AreEqual(12, actual.Minute);
            Assert.AreEqual(12, actual.Second);
        }

        [TestMethod]
        public void StringExtension_ConvertStringToGenericValue_StringIsNullTypeIsDecimal_ReturnsDecimal()
        {
            const string s = "12.55";
            var actual = s.ConvertStringToGenericValue<Decimal>();
            Assert.AreEqual(12.55m, actual);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtension_Constructor_CropWholeWords_NoValue_ThrowsException()
        {
            string test = null;
            test.CropWholeWords(10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StringExtension_Constructor_CropWholeWords_NegativeLength_ThrowsException()
        {
            const string test = "Matthew Jordan";
            var dictionary = test.CropWholeWords(-1);
        }

        [TestMethod]
        public void StringExtension_CropWholeWords_CropsCorrectly()
        {
            const string s = "Matt Jordan Says Hello";
            var crop = s.CropWholeWords(10);
            Assert.AreEqual("Matt", crop);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtension_Constructor_CropWholeWordsIntoChunks_NoValue_ThrowsException()
        {
            string test = null;
            var dictionary = test.CropWholeWordsIntoChunks(1, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StringExtension_Constructor_CropWholeWordsIntoChunks_NegativeLength_ThrowsException()
        {
            const string test = "Matthew Jordan";
            var dictionary = test.CropWholeWordsIntoChunks(10, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StringExtension_Constructor_CropWholeWordsIntoChunks_ChunkSizeLessThanOne_ThrowsException()
        {
            const string test = "Matthew Jordan";
            var dictionary = test.CropWholeWordsIntoChunks(0, 10);
        }

        [TestMethod]
        public void StringExtension_CropWholeWordsIntoChunks_ChunksAndCropsTwice()
        {
            const string s = "Matt Jordan";
            var dictionary = s.CropWholeWordsIntoChunks(2, 10);
            Assert.AreEqual(dictionary.Count, 2);
            Assert.AreEqual("Matt", dictionary[1]);
            Assert.AreEqual("Jordan", dictionary[2]);
        }

        [TestMethod]
        public void StringExtension_CropWholeWordsIntoChunks_ChunksAndCropsThreeTimesWithEmptyEntry()
        {
            const string s = "Matt Jordan";
            var dictionary = s.CropWholeWordsIntoChunks(3, 10);
            Assert.AreEqual(dictionary.Count, 3);
            Assert.AreEqual("Matt", dictionary[1]);
            Assert.AreEqual("Jordan", dictionary[2]);
            Assert.AreEqual(String.Empty, dictionary[3]);
        }

        [TestMethod]
        public void StringExtension_CropWholeWordsIntoChunks_ChunksAndCropsCorrectlyWhenFirstWordInChunkIsGreaterThanLength()
        {
            const string s = "Matt Jordan";
            var dictionary = s.CropWholeWordsIntoChunks(3, 4);
            Assert.AreEqual(dictionary.Count, 3);
            Assert.AreEqual("Matt", dictionary[1]);
            Assert.AreEqual(String.Empty, dictionary[2]);
            Assert.AreEqual(String.Empty, dictionary[3]);
        }



    }

}
