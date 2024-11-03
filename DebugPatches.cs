/*using System;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using HarmonyLib;
using BaseX;
using System.Collections.Generic;
using System.Dynamic;

namespace ResonitePackageExporter
{
    public static class DebugPatches
    {
        public static void Patch(Harmony harmony)
        {
            var target = Type.GetType("System.Text.Json.JsonClassInfo, System.Text.Json").GetMethod("AddProperty", BindingFlags.Public | BindingFlags.Static);
            var patch = typeof(DebugPatches).GetMethod(nameof(TargetPatch), BindingFlags.Public | BindingFlags.Static);

            harmony.Patch(target, postfix: new(patch));
        }
        public static Dictionary<object,ExpandoObject> TEST_VALUES = new();

        public static void TargetPatch(object __result, MemberInfo memberInfo, Type memberType, Type parentClassType, JsonNumberHandling? parentTypeNumberHandling, JsonSerializerOptions options)
        {
            if (parentClassType != typeof(float3)) return;
            if (memberInfo.Name != "ElementType" && memberInfo.Name != "X" && memberInfo.Name != "Y" && memberInfo.Name != "Z") return;

            dynamic expando = new ExpandoObject();
            expando.memberInfo = memberInfo;
            expando.memberType = memberType;
            expando.parentClassType = parentClassType;
            expando.parentTypeNumberHandling = parentTypeNumberHandling;
            expando.options = options;

            TEST_VALUES.Add(__result, expando);
            //Options:\n\t\t\tAllowTrailingCommas: {options.AllowTrailingCommas}\n\t\t\tAllowTrailingCommas: {options.}
            Logger.Log(
                $"\n\t\tMember: {memberInfo.Name}\n\t\t" +
                $"Type: {memberType}\n\t\t" +
                $"Parent Class: {parentClassType}\n\t\t" +
                $"Parent Type Number Handling: {parentTypeNumberHandling}"
                );
            Logger.Log($"\n\t\t{JsonPropertyInfoToString(__result)}\n");
        }
        public static string JsonPropertyInfoToString(object info) => $"DeclaredPropertyType: {(Type)GetValue(info, "DeclaredPropertyType")}\n\t\t" +
                $"HasGetter: {(bool)GetValue(info, "HasGetter")}\n\t\t" +
                $"HasSetter: {(bool)GetValue(info, "HasSetter")}\n\t\t" +
                $"IgnoreDefaultValuesOnRead: {(bool)GetValue(info, "IgnoreDefaultValuesOnRead")}\n\t\t" +
                $"IgnoreDefaultValuesOnWrite: {(bool)GetValue(info, "IgnoreDefaultValuesOnWrite")}\n\t\t" +
                $"IsForClassInfo: {(bool)GetValue(info, "IsForClassInfo")}\n\t\t" +
                $"IsIgnored: {(bool)GetValue(info, "IsIgnored")}\n\t\t" +
                $"NameAsString: {(string)GetValue(info, "NameAsString")}\n\t\t" +
                $"NumberHandling: {(JsonNumberHandling?)GetValue(info, "NumberHandling")}\n\t\t" +
                $"ParentClassType: {(Type)GetValue(info, "ParentClassType")}\n\t\t" +
                $"PropertyTypeCanBeNull: {(bool)GetValue(info, "PropertyTypeCanBeNull")}\n\t\t" +
                $"ShouldDeserialize: {(bool)GetValue(info, "ShouldDeserialize")}\n\t\t" +
                $"ShouldSerialize: {(bool)GetValue(info, "ShouldSerialize")}\n\t\t";
        public static object GetValue(object instance, string name) => instance.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(instance);
        
    }
}
*/