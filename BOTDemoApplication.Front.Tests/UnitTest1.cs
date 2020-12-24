using BOTDemoApplication.Front.BusinessLogic;
using System;
using Xunit;

namespace BOTDemoApplication.Front.Tests
{
    public class UnitTest1
    {

        //Should pass
        [Fact]
        public void Test1()
        {
            //Arrange
            string input = "test";

            //Act
            var result = new ValuesLogic().ForUnitTesting(input);

            //Assert
            Assert.Equal("test", result);
        }

        //Should not pass
        [Fact]
        public void Test2()
        {
            //Arrange
            string input = "test";

            //Act
            var result = new ValuesLogic().ForUnitTesting(input);

            //Assert
            Assert.Equal("test2", result);
        }
    }
}
