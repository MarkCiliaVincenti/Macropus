﻿using System.Data;
using ECS.Schema;

namespace ECS.Serialize.Deserialize.State;

public interface IDeserializeState : IState
{
	Task Read(IDbConnection dbConnection);

	bool HasRefs();
	IDeserializeState PopSomeRefs();
	void AddRef(DataSchemaElement target, object? obj);
	
	object? Create();
}