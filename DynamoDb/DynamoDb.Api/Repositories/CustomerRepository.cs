using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynamoDb.Api.Dtos;
using DynamoDb.Api.Models;
using System.Net;
using System.Text.Json;

namespace DynamoDb.Api.Repositories
{
    public sealed class CustomerRepository(IAmazonDynamoDB dynamoDb)
    {
        private const string TableName = "customers";
        public async Task<Guid> CreateAsync(CreateCustomerDto createCustomerDto)
        {
            var customer = new Customer()
            {
                Name = createCustomerDto.Name,
                Address = createCustomerDto.Address,
                Email = createCustomerDto.Email
            };
            var customerJson = JsonSerializer.Serialize(customer);
            var customerAttribute = Document.FromJson(customerJson).ToAttributeMap();
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = customerAttribute,
                ConditionExpression = "attribute_not_exists(pk) and attribute_not_exists(sk)"
            };
            var response = await dynamoDb.PutItemAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK ? customer.Id : throw new Exception("Failed to create customer");
        }
        public async Task<UpdateCustomerOutcome> UpdateAsync(UpdateCustomerDto updateCustomerDto)
        {
            var customer = new Customer()
            {
                Id = updateCustomerDto.Id,
                Name = updateCustomerDto.Name,
                Address = updateCustomerDto.Address,
                Email = updateCustomerDto.Email,
                UpdatedAt = DateTime.UtcNow
            };
            var customerJson = JsonSerializer.Serialize(customer);
            var customerAttribute = Document.FromJson(customerJson).ToAttributeMap();
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = customerAttribute,
                ConditionExpression = "attribute_exists(pk) and (attribute_not_exists(UpdatedAt) or UpdatedAt = :expectedUpdatedAt)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":expectedUpdatedAt",
                        updateCustomerDto.ExpectedUpdatedAt is { } expectedUpdatedAt
                            ? new AttributeValue { S = expectedUpdatedAt.ToString("O") }
                            : new AttributeValue { NULL = true }
                    }
                }
            };
            try
            {
                await dynamoDb.PutItemAsync(request);
                return new UpdateCustomerOutcome(UpdateCustomerResult.Success, customer.Id);
            }
            catch (ConditionalCheckFailedException)
            {
                var exists = await ExistsAsync(updateCustomerDto.Id);
                return exists
                    ? new UpdateCustomerOutcome(UpdateCustomerResult.Conflict, null)
                    : new UpdateCustomerOutcome(UpdateCustomerResult.NotFound, null);
            }
        }

        private async Task<bool> ExistsAsync(Guid id)
        {
            var response = await dynamoDb.GetItemAsync(new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "pk", new AttributeValue { S = id.ToString() } },
                    { "sk", new AttributeValue { S = id.ToString() } }
                },
                ProjectionExpression = "pk"
            });
            return response.Item is { Count: > 0 };
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "pk", new AttributeValue { S = id.ToString() } },
                    { "sk", new AttributeValue { S = id.ToString() } }
                },
                ReturnValues = ReturnValue.ALL_OLD
            };
            var response = await dynamoDb.DeleteItemAsync(request);
            return response.Attributes is not null;
        }
        public async Task<IReadOnlyList<CustomerDto>> GetAsync()
        {
            var scanRequest = new ScanRequest
            {
                TableName = TableName
            };
            var response = await dynamoDb.ScanAsync(scanRequest);
            var items = response.Items.Select(x =>
            {
                var json = Document.FromAttributeMap(x).ToJson();
                return JsonSerializer.Deserialize<CustomerDto>(json);
            });
            return items.ToList();
        }
        public async Task<CustomerDto?> GetByEmailAsync(string email)
        {
            var request = new QueryRequest
            {
                TableName = TableName,
                IndexName = "email-id-index",
                KeyConditionExpression = "Email = :email",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":email", new AttributeValue { S = email } }
                }
            };

            var response = await dynamoDb.QueryAsync(request);
            if (response.Items.Count == 0)
            {
                return null;
            }
            var json = Document.FromAttributeMap(response.Items[0]).ToJson();
            return JsonSerializer.Deserialize<CustomerDto>(json);
        }
    }
}
