namespace HackF5.BookFoundry.Service.WebApi.GraphQL;

using System.Reflection;

using HackF5.BookFoundry.Service.WebApi.Data;

using HotChocolate.Execution.Configuration;

using Microsoft.Extensions.DependencyInjection;

public static class HackF5BookFoundryServiceWebApiGraphQLRequestBuilderExtensions
{
    public static IRequestExecutorBuilder BuildDefaultSchema(this IRequestExecutorBuilder builder) => builder
        .RegisterServices()
        .AllowIntrospection(true)
        .AddCoreQueryAndMutationTypes()
        .AddSorting()
        .AddCoreFiltering()
        .AddCoreDataComponents<ApplicationDbContext>()
        .AddCoreValidation(Assembly.GetExecutingAssembly())
        .AddCoreTypeExtensions(Assembly.GetExecutingAssembly())
        .AddCoreObjectTypes(Assembly.GetExecutingAssembly())
        .AddCoreDiagnosticComponents();

    private static IRequestExecutorBuilder RegisterServices(this IRequestExecutorBuilder builder)
    {
        builder.BindRuntimeType<uint, UnsignedIntType>();

        return builder;
    }
}