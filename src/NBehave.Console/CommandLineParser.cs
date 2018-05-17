using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NBehave.Extensions;
using NBehave.Internal;

namespace NBehave.Console
{
    public abstract class CommandLineOptions
    {
        public List<string> Parameters { get; set; }

        protected CommandLineOptions()
        {
            Parameters = new List<string>();
        }

        public virtual void ShowHelp()
        {
            var props = GetType().GetProperties().Where(_ => _.GetCustomAttributes(typeof(OptionAttribute), true).Any());
            var attribs = props.Select(_ => new
                                                {
                                                    Prop = _,
                                                    Attribute = (OptionAttribute)_.GetCustomAttributes(typeof(OptionAttribute), true)[0],
                                                    _.GetSetMethod(true).GetParameters()[0].ParameterType
                                                }).OrderBy(_ => _.Prop.Name);
            var longestName = attribs.OrderByDescending(_ => _.Prop.Name.Length).First();
            int padNameToLength = longestName.Prop.Name.Length + 7;
            foreach (var attrib in attribs)
            {
                var txt = string.Format("/{0}", FormatName(attrib.Prop));
                string param = "";
                if (attrib.ParameterType != typeof(bool))
                    param = "=STR";
                System.Console.Write("{0}{1}", (txt + param).PadRight(padNameToLength), attrib.Attribute.Description);
                if (!string.IsNullOrEmpty(attrib.Attribute.Short))
                    System.Console.Write(" (Short format: /{0}{1})", attrib.Attribute.Short, param);
                System.Console.WriteLine();
            }
        }

        private static string FormatName(PropertyInfo property)
        {
            return property.Name[0].ToString(CultureInfo.CurrentUICulture).ToLower() + property.Name.Substring(1);
        }

        public Exception Exception { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        public string Short { get; set; }
        public string Description { get; set; }
    }

    public class CommandLineParser<T>
        where T : CommandLineOptions, new()
    {
        private readonly T options;
        private readonly Queue<string> args;
        readonly TypeConverter typeConverter = new TypeConverter();


        private CommandLineParser(T options, Queue<string> args)
        {
            this.options = options;
            this.args = args;
        }

        public static T Parse(IEnumerable<string> args)
        {
            var options = new T();
            var p = new CommandLineParser<T>(options, new Queue<string>(args));
            return p.Parse();
        }

        private T Parse()
        {
            try
            {
                while (args.Any())
                {
                    var arg = args.Dequeue();
                    var separator = GetParameterStart(arg);
                    if (separator == null)
                        AddToDefaultParameters(arg);
                    else
                        AddParameter(arg);
                }
            }
            catch (Exception ex)
            {
                options.Exception = ex;
            }
            return options;
        }

        private void AddParameter(string arg)
        {
            var argName = GetArgName(arg);
            var argValue = GetArgValue(arg);
            var separator = GetParameterStart(arg);
            var property = FindProperty(argName);
            if (argValue == null && separator == "--" && property.PropertyType != typeof(bool) && args.Any())
                argValue = args.Dequeue();
            SetProperty(property, argValue);
        }

        private void SetProperty(PropertyInfo property, string argValue)
        {
            var p = GetParameterInfo(property);
            object value;
            if (p.ParameterType.IsArray || p.IsGenericIEnumerable())
            {
                var propValue = property.GetValue(options, null);
                var method = propValue.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                value = typeConverter.ChangeParameterType(argValue, method.GetParameters()[0]);

                method.Invoke(propValue, new[] { value });
                return;
            }
            value = p.ParameterType == typeof(bool) ? true : typeConverter.ChangeParameterType(argValue, p);
            property.SetValue(options, value, BindingFlags.NonPublic, null, null, CultureInfo.CurrentCulture);
        }

        private ParameterInfo GetParameterInfo(PropertyInfo property)
        {
            var parameters = property.GetSetMethod(true).GetParameters();
            ParameterInfo p = parameters[0];
            return p;
        }

        private PropertyInfo FindProperty(string argName)
        {
            var props = options.GetType().GetProperties();
            var p = props.FirstOrDefault(_ => _.Name.Equals(argName, StringComparison.InvariantCultureIgnoreCase));
            if (p != null)
                return p;

            var ps = props
                .Select(_ => new
                                 {
                                     Property = _,
                                     Attribute = ((OptionAttribute)_.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault())
                                 }).ToList();
            p = ps.Where(_ => _.Attribute != null && _.Attribute.Short != null && _.Attribute.Short.Equals(argName, StringComparison.InvariantCultureIgnoreCase))
                .Select(_ => _.Property)
                .FirstOrDefault();
            return p;
        }

        private string GetArgName(string arg)
        {
            return SplitArgNameAndValue(arg)[0];
        }

        private string GetArgValue(string arg)
        {
            return SplitArgNameAndValue(arg)[1];
        }

        private string[] SplitArgNameAndValue(string arg)
        {
            var separator = GetParameterStart(arg);
            var s = arg.TrimStart(separator.ToCharArray());
            var paramSeparator = s.IndexOf('=');
            paramSeparator = (paramSeparator == -1) ? s.IndexOf(':') : paramSeparator;

            if (paramSeparator == -1)
                return new[] { s, null };
            return new[] { s.Substring(0, paramSeparator), s.Substring(paramSeparator + 1) };
        }

        private void AddToDefaultParameters(string arg)
        {
            options.Parameters.Add(arg);
        }

        private string GetParameterStart(string arg)
        {
            var separators = new[] { "/", "--" };
            return separators.FirstOrDefault(arg.StartsWith);
        }
    }
}
