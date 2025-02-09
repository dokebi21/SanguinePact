using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using ProjectM;

namespace SanguinePact.Common.Utils;

//#pragma warning disable CS8500
public static class ECSExtensions
{
	public unsafe static void Write<T>(this Entity entity, T componentData) where T : struct
	{
		// Get the ComponentType for T
		var ct = new ComponentType(Il2CppType.Of<T>());

		// Marshal the component data to a byte array
		byte[] byteArray = StructureToByteArray(componentData);

		// Get the size of T
		int size = Marshal.SizeOf<T>();

		// Create a pointer to the byte array
		fixed (byte* p = byteArray)
		{
			// Set the component data
			Core.EntityManager.SetComponentDataRaw(entity, ct.TypeIndex, p, size);
		}
	}
	public delegate void ActionRef<T>(ref T item);

	public static void With<T>(this Entity entity, ActionRef<T> action) where T : struct
	{
		T item = entity.ReadRW<T>();
		action(ref item);
		Core.EntityManager.SetComponentData(entity, item);
	}
	public unsafe static T ReadRW<T>(this Entity entity) where T : struct
	{
		var ct = new ComponentType(Il2CppType.Of<T>());
		void* componentDataRawRW = Core.EntityManager.GetComponentDataRawRW(entity, ct.TypeIndex);
		T componentData = Marshal.PtrToStructure<T>(new IntPtr(componentDataRawRW));
		return componentData;
	}
	// Helper function to marshal a struct to a byte array
	public static byte[] StructureToByteArray<T>(T structure) where T : struct
	{
		int size = Marshal.SizeOf(structure);
		byte[] byteArray = new byte[size];
		IntPtr ptr = Marshal.AllocHGlobal(size);

		Marshal.StructureToPtr(structure, ptr, true);
		Marshal.Copy(ptr, byteArray, 0, size);
		Marshal.FreeHGlobal(ptr);

		return byteArray;
	}

	public unsafe static T Read<T>(this Entity entity) where T : struct
	{
		// Get the ComponentType for T
		var ct = new ComponentType(Il2CppType.Of<T>());

		// Get a pointer to the raw component data
		void* rawPointer = Core.EntityManager.GetComponentDataRawRO(entity, ct.TypeIndex);

		// Marshal the raw data to a T struct
		T componentData = Marshal.PtrToStructure<T>(new IntPtr(rawPointer));

		return componentData;
	}
	public static DynamicBuffer<T> ReadBuffer<T>(this Entity entity) where T : struct
	{
		return Core.Server.EntityManager.GetBuffer<T>(entity);
	}
	public static bool Has<T>(this Entity entity)
	{
		var ct = new ComponentType(Il2CppType.Of<T>());
		return Core.EntityManager.HasComponent(entity, ct);
	}

	public static string LookupName(this PrefabGUID prefabGuid)
	{
		var prefabCollectionSystem = Core.Server.GetExistingSystemManaged<PrefabCollectionSystem>();
		return (prefabCollectionSystem.PrefabGuidToNameDictionary.ContainsKey(prefabGuid)
			? prefabCollectionSystem.PrefabGuidToNameDictionary[prefabGuid] : "GUID Not Found").ToString();
	}

	public static string GetPrefabName(this PrefabGUID prefabGuid)
	{
		return Core.PrefabCollectionSystem.PrefabGuidToNameDictionary.TryGetValue(prefabGuid, out string prefabName) ? $"{prefabName}" : "String.Empty";
	}

	public static void Add<T>(this Entity entity)
	{
		var ct = new ComponentType(Il2CppType.Of<T>());
		Core.EntityManager.AddComponent(entity, ct);
	}

	public static void Remove<T>(this Entity entity)
	{
		var ct = new ComponentType(Il2CppType.Of<T>());
		Core.EntityManager.RemoveComponent(entity, ct);
	}

}
//#pragma warning restore CS8500
