using System;
using Amazon.DynamoDBv2.DataModel;
using Paramore.Brighter.Outbox.DynamoDB;
using Xunit;

namespace Paramore.Brighter.Tests.DynamoDbExtensions
{
    public class DynamoDbFactoryMissingTableAttributeTests 
    {
        [Fact]
        public void When_Creating_A_Table_From_A_Class_Missing_Table_Attribute()
        {
            //arrange, act, assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                var tableRequestFactory = new DynamoDbTableFactory();
                tableRequestFactory.GenerateCreateTableMapper<DynamoDbEntity>();
            });
        }
        
        private class DynamoDbEntity{}
    }
}
