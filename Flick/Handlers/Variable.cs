using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flick.Handlers
{
    public static class Variable
    {
        public enum Types
        {
            String = 0x73,
            Integer = 0x69,
            Float = 0x66
        }

        public static Dictionary<string, VariableInfo> Variables = new Dictionary<string, VariableInfo>();
        public static Stack<VariableInfo> VariableStack = new Stack<VariableInfo>();
        public static Stack<VariableInfo> ResultStack = new Stack<VariableInfo>();

        /// <summary>
        ///     Creates a variable.
        /// </summary>
        /// <param name="reader">Reader</param>
        public static void CreateVariable(ref BinaryReader reader)
        {
            var variableName = reader.ReadString();
            var valueType = reader.ReadByte();

            if (Enum.GetValues(typeof(Types)).Cast<Types>().All(x => ((byte) x) != valueType))
            {
                // Invalid type!
                Console.WriteLine("Flick encountered a critical error. Invalid variable type byte header.");
                Environment.Exit(-1);
            }

            object value = null;

            switch ((Types) valueType)
            {
                case Types.String:
                    value = reader.ReadString();
                    break;
                case Types.Integer:
                    value = reader.ReadInt32();
                    break;
                case Types.Float:
                    value = reader.ReadSingle();
                    break;
            }

            Variables[variableName] = new VariableInfo((Types) valueType, value);
            Console.WriteLine("Added Variable: {0}, {1}, {2}", variableName, (Types) valueType, value);
        }

        /// <summary>
        ///     Pushes a variable to the stack, which is for arguements.
        /// </summary>
        /// <param name="reader">Binary Readeer</param>
        public static void PushVariable(ref BinaryReader reader)
        {
            var variableName = reader.ReadString();

            VariableInfo variableInfo;

            if (!Variables.TryGetValue(variableName, out variableInfo))
            {
                Console.WriteLine("Flick encountered a critical error. Can't push non-existing variable to stack!");
                Environment.Exit(-1);
            }

            VariableStack.Push(variableInfo);
        }

        /// <summary>
        ///     Pops a value from the result stack, and pushes it to the variable stack.
        /// </summary>
        /// <param name="reader"></param>
        public static void PushResultToStack(ref BinaryReader reader)
        {
            VariableStack.Push(ResultStack.Pop());
        }

        /// <summary>
        ///     Adds 2 variables, and pushes the result to the result stack.
        /// </summary>
        /// <param name="reader"></param>
        public static void Add(ref BinaryReader reader)
        {
            // Get first 2 values
            var variableName = reader.ReadString();
            var variableName2 = reader.ReadString();

            if (!Variables.ContainsKey(variableName) || !Variables.ContainsKey(variableName2))
            {
                Console.WriteLine("Flick encountered a critical error. Variables to add don't exist!");
                Environment.Exit(-1);
            }

            var variable = Variables[variableName];
            var variable2 = Variables[variableName2];

            if (!variable.IsNumber() || !variable2.IsNumber())
            {
                Console.WriteLine("Flick encountered a critical error. Variables are not both numbers!");
                Environment.Exit(-1);
            }

            var result = variable.Type == Types.Integer
                ? (int) variable.Value
                : variable.Type == Types.Float
                    ? (float) variable.Value
                    : 0 + variable2.Type == Types.Integer
                        ? (int) variable2.Value
                        : variable2.Type == Types.Float ? (float) variable2.Value : 0;

            ResultStack.Push(new VariableInfo(Types.Float, result));
        }

        public static bool IsNumber(this VariableInfo variable)
        {
            return variable.Type == Types.Float || variable.Type == Types.Integer;
        }

        public class VariableInfo
        {
            public Types Type;
            public object Value;

            public VariableInfo(Types type, object value)
            {
                Type = type;
                Value = value;
            }
        }
    }
}