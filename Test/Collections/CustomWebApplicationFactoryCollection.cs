using Test.Fixtures;

namespace Test.Collections;

[CollectionDefinition("CustomWebApplicationFactoryTests")]
public class CustomWebApplicationFactoryCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
{

}