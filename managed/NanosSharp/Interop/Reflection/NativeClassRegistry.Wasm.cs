#if NANOSSHARP_WASM
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace NanosSharp.Interop.Reflection;

internal static unsafe partial class NativeClassRegistry
{
    private const int InitialSchemaCapacity = 64 * 1024;

    [LibraryImport("nanos", EntryPoint = "schema-dump")]
    [WasmImportLinkage]
    private static partial int SchemaDump(byte* buffer, int capacity);

    private static void LoadFromSchema()
    {
        var capacity = InitialSchemaCapacity;
        while (true)
        {
            var buffer = new byte[capacity];
            int needed;
            fixed (byte* pointer = buffer)
            {
                needed = SchemaDump(pointer, capacity);
            }

            if (needed <= capacity)
            {
                Parse(new ReadOnlySpan<byte>(buffer, 0, needed));
                return;
            }

            capacity = needed;
        }
    }

    private static void Parse(ReadOnlySpan<byte> blob)
    {
        var cursor = new SchemaCursor(blob);

        var classCount = cursor.ReadUInt32();
        for (var index = 0u; index < classCount; index++)
        {
            Register(ReadClass(ref cursor));
        }

        var functionCount = cursor.ReadUInt32();
        for (var index = 0u; index < functionCount; index++)
        {
            Register(ReadMethod(ref cursor));
        }
    }

    private static NativeClass ReadClass(ref SchemaCursor cursor)
    {
        var id = cursor.ReadUInt64();
        var name = cursor.ReadString();

        var constructorHandle = cursor.ReadInt32();
        NativeMethod? constructor = null;
        if (constructorHandle != 0)
        {
            constructor = new NativeMethod(constructorHandle, cursor.ReadString());
        }

        var methods = ReadMethods(ref cursor);
        var staticMethods = ReadMethods(ref cursor);
        var properties = ReadProperties(ref cursor);
        return new NativeClass(id, name, constructor, methods, staticMethods, properties);
    }

    private static Dictionary<string, NativeMethod> ReadMethods(ref SchemaCursor cursor)
    {
        var count = cursor.ReadUInt16();
        var methods = new Dictionary<string, NativeMethod>(count, StringComparer.Ordinal);
        for (var index = 0; index < count; index++)
        {
            var method = ReadMethod(ref cursor);
            methods.TryAdd(method.Name, method);
        }

        return methods;
    }

    private static Dictionary<string, NativeProperty> ReadProperties(ref SchemaCursor cursor)
    {
        var count = cursor.ReadUInt16();
        var properties = new Dictionary<string, NativeProperty>(count, StringComparer.Ordinal);
        for (var index = 0; index < count; index++)
        {
            var handle = cursor.ReadInt32();
            var property = new NativeProperty(handle, cursor.ReadString());
            properties.TryAdd(property.Name, property);
        }

        return properties;
    }

    private static NativeMethod ReadMethod(ref SchemaCursor cursor)
    {
        var handle = cursor.ReadInt32();
        return new NativeMethod(handle, cursor.ReadString());
    }

    private ref struct SchemaCursor(ReadOnlySpan<byte> data)
    {
        private readonly ReadOnlySpan<byte> _data = data;
        private int _offset = 0;

        public ushort ReadUInt16()
        {
            var value = BinaryPrimitives.ReadUInt16LittleEndian(_data[_offset..]);
            _offset += sizeof(ushort);
            return value;
        }

        public int ReadInt32()
        {
            var value = BinaryPrimitives.ReadInt32LittleEndian(_data[_offset..]);
            _offset += sizeof(int);
            return value;
        }

        public uint ReadUInt32()
        {
            var value = BinaryPrimitives.ReadUInt32LittleEndian(_data[_offset..]);
            _offset += sizeof(uint);
            return value;
        }

        public ulong ReadUInt64()
        {
            var value = BinaryPrimitives.ReadUInt64LittleEndian(_data[_offset..]);
            _offset += sizeof(ulong);
            return value;
        }

        public string ReadString()
        {
            var length = ReadUInt16();
            var text = Encoding.UTF8.GetString(_data.Slice(_offset, length));
            _offset += length;
            return text;
        }
    }
}
#endif
