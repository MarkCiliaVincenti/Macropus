using ECS.Db.Storage.Impl;
using ECS.Db.Tests.Utils;
using ECS.Schema;
using ECS.Serialize;
using ECS.Tests.Schema;
using LinqToDB.Data;
using Macropus.ECS.Component.Storage.Impl.Changes;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Db.Tests;

public class ComponentsStorageTests : TestsWithDatabase
{
	public ComponentsStorageTests(ITestOutputHelper output) : base(output)
	{
		DataConnection.TurnTraceSwitchOn();
		DataConnection.WriteTraceLine = (s1, _, _) => Console.WriteLine(s1);
	}
	
	[Fact]
	public async Task GetGroup()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();
		var schema2 = builder.CreateSchema<DataSchemaTestTypeComponent2>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);
		await serializer.CreateTablesBySchema(schema2);

		var guid = Guid.NewGuid();
		await serializer.SerializeAsync(schema, guid, new DataSchemaTestTypeComponent());
		await serializer.SerializeAsync(schema2, guid, new DataSchemaTestTypeComponent2());

		var storageInDb = new ComponentsStorageInDb(DbConnection);

		var entities = storageInDb.GetEntities();
		
		Assert.NotEmpty(entities);
		Assert.Equal(1, entities.Count());
		Assert.Equal(guid, entities.First());
	}

	[Fact]
	public async Task HasComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);

		var guid = Guid.NewGuid();
		await serializer.SerializeAsync(schema, guid, new DataSchemaTestTypeComponent());

		var storageInDb = new ComponentsStorageInDb(DbConnection);
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));
		Assert.False(storageInDb.HasComponent<DataSchemaTestTypeComponent2>(guid));
	}

	[Fact]
	public async Task ReplaceComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);
		
		var storageInDb = new ComponentsStorageInDb(DbConnection);
		
		var guid = Guid.NewGuid();
		Assert.False(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));

		var oldComponent = DataSchemaRandomUtils.GetRandomDataSchemaTestTypeComponent();
		storageInDb.ReplaceComponent(guid, oldComponent);

		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));

		var newComponent = storageInDb.GetComponent<DataSchemaTestTypeComponent>(guid);

		DataSchemaUtils.CheckDeserializedComponent(newComponent, oldComponent);
	}

	[Fact]
	public async Task GetComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);
		
		var oldComponent = DataSchemaRandomUtils.GetRandomDataSchemaTestTypeComponent();

		var guid = Guid.NewGuid();
		await serializer.SerializeAsync(schema, guid, oldComponent);

		var storageInDb = new ComponentsStorageInDb(DbConnection);
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));

		var newComponent = storageInDb.GetComponent<DataSchemaTestTypeComponent>(guid);

		DataSchemaUtils.CheckDeserializedComponent(newComponent, oldComponent);
	}

	[Fact]
	public async Task RemoveComponent()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);
		
		var oldComponent = DataSchemaRandomUtils.GetRandomDataSchemaTestTypeComponent();

		var guid = Guid.NewGuid();
		await serializer.SerializeAsync(schema, guid, oldComponent);

		var storageInDb = new ComponentsStorageInDb(DbConnection);
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));

		var newComponent = storageInDb.GetComponent<DataSchemaTestTypeComponent>(guid);

		DataSchemaUtils.CheckDeserializedComponent(newComponent, oldComponent);

		storageInDb.RemoveComponent<DataSchemaTestTypeComponent>(guid);

		Assert.False(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));
	}
	
	[Fact]
	public async Task ApplyChanges()
	{
		var builder = new DataSchemaBuilder();
		var schema = builder.CreateSchema<DataSchemaTestTypeComponent>();
		var schema2 = builder.CreateSchema<DataSchemaTestTypeComponent2>();

		using var serializer = new ComponentSerializer(DbConnection);
		await serializer.CreateTablesBySchema(schema);
		await serializer.CreateTablesBySchema(schema2);

		var guid = Guid.NewGuid();
		
		var storageInDb = new ComponentsStorageInDb(DbConnection);
		storageInDb.ReplaceComponent(guid, new DataSchemaTestTypeComponent2());
		
		Assert.False(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));
		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent2>(guid));
		
		var changes = new ComponentsChangesStorageInMemory();
		
		var oldComponent = DataSchemaRandomUtils.GetRandomDataSchemaTestTypeComponent();
		changes.ReplaceComponent(guid, oldComponent);
		changes.RemoveComponent<DataSchemaTestTypeComponent2>(guid);
		storageInDb.Apply(changes);

		Assert.True(storageInDb.HasComponent<DataSchemaTestTypeComponent>(guid));
		Assert.False(storageInDb.HasComponent<DataSchemaTestTypeComponent2>(guid));

		var newComponent = storageInDb.GetComponent<DataSchemaTestTypeComponent>(guid);
		DataSchemaUtils.CheckDeserializedComponent(newComponent, oldComponent);
	}

}