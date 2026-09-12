namespace DynamoDb.Api.Repositories
{
    public readonly record struct UpdateCustomerOutcome(UpdateCustomerResult Result, Guid? Id);
}
