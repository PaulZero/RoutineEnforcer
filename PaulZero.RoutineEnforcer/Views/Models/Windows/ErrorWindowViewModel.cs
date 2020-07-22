using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace PaulZero.RoutineEnforcer.Views.Models.Windows
{
    public class ErrorWindowViewModel
    {
        public string ExceptionClass { get; }

        public string ExceptionMessage { get; }

        public string ExceptionStackTrace { get; }

        public bool HasException => !string.IsNullOrWhiteSpace(ExceptionClass);

        public bool HasMessage => !string.IsNullOrWhiteSpace(Message);

        public string Message { get; }

        public string Title { get; }

        public ErrorWindowViewModel()
            : this(
                "An Unhandled Error Occurred",
                "Something went horribly wrong but luckily it was caught so this could be displayed!",
                ExceptionHelper.Get()
            )
        {
        }

        public ErrorWindowViewModel(string title, string message, Exception exception)
        {
            Title = title;
            Message = message;

            ExceptionClass = exception?.GetType().Name;
            ExceptionMessage = exception?.Message;
            ExceptionStackTrace = FormatStackTrace(exception);
        }

        public string CreateClipboardSummary()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine("RoutineEnforcer Error Dump")
                .AppendLine()
                .AppendLine("Details")
                .AppendLine($" - Time: {DateTime.Now}")
                .AppendLine($" - Title: {Title}");

            if (HasMessage)
            {
                stringBuilder.AppendLine($" - Message: {Message}");
            }

            if (HasException)
            {
                stringBuilder
                    .AppendLine()
                    .AppendLine("Exception")
                    .AppendLine($" - Class: {ExceptionClass}")
                    .AppendLine($" - Message: {ExceptionMessage}")
                    .AppendLine()
                    .AppendLine("Stack Trace")
                    .AppendLine()
                    .AppendLine(ExceptionStackTrace);
            }

            return stringBuilder.ToString();
        }

        public string FormatStackTrace(Exception exception, int indent = 0)
        {
            if (exception == default)
            {
                return string.Empty;
            }

            var stackTrace = new StackTrace(exception, true);

            if (stackTrace.FrameCount == 0)
            {
                return string.Empty;
            }

            var traceBuilder = new StringBuilder();

            var index = 0;

            var paddingSize = index.ToString().Length + 2 + indent;
            var linePrefix = new string(' ', paddingSize);

            foreach (var frame in stackTrace.GetFrames().Reverse())
            {
                var method = frame.GetMethod();
                var paddedIndex = $"{new string(' ', indent)}#{index}".PadRight(paddingSize);

                traceBuilder.Append(paddedIndex);

                if (method == null)
                {
                    traceBuilder.AppendLine($"[unknown method]");
                }
                else
                {
                    var className = method.DeclaringType?.FullName ?? "[unknown type]";

                    if (className.Contains('+'))
                    {
                        var nestedClasses = className.Split('+');

                        className = $"{string.Join("][", nestedClasses)}";
                    }

                    traceBuilder.AppendLine($"[{className}]");

                    traceBuilder.Append(linePrefix);

                    if (method.IsPublic)
                    {
                        traceBuilder.Append("public ");
                    }
                    else if (method.IsPrivate)
                    {
                        traceBuilder.Append("private ");
                    }

                    if (method.IsStatic)
                    {
                        traceBuilder.Append("static ");
                    }
                    else if (method.IsAbstract)
                    {
                        traceBuilder.Append("abstract ");
                    }
                    else if (method.IsVirtual)
                    {
                        traceBuilder.Append("virtual ");
                    }
                    else if (method.IsFinal)
                    {
                        traceBuilder.Append("final ");
                    }

                    if (!method.IsConstructor)
                    {
                        if (method is MethodInfo methodInfo)
                        {
                            traceBuilder.Append($"{methodInfo.ReturnType.Name} ");
                        }
                        else
                        {
                            traceBuilder.Append("<unknown type> ");
                        }
                    }

                    var formattedParameters = new List<string>();

                    foreach (var parameter in method.GetParameters())
                    {
                        var defaultValue = string.Empty;
                        var parameterName = parameter.Name;
                        var parameterType = parameter.ParameterType.Name;

                        if (parameter.HasDefaultValue)
                        {
                            if (parameter.DefaultValue == null)
                            {
                                defaultValue = "null";
                            }
                            else
                            {
                                var defaultValueType = parameter.DefaultValue.GetType();

                                if (defaultValueType == typeof(string))
                                {
                                    defaultValue = $"\"{parameter.DefaultValue}\"";
                                }
                                else if (defaultValueType.IsEnum)
                                {
                                    var enumFieldName = Enum.GetName(defaultValueType, parameter.DefaultValue);

                                    defaultValue = $"{defaultValueType.Name}.{enumFieldName}";
                                }
                                else if (defaultValueType == typeof(bool))
                                {
                                    defaultValue = (bool)parameter.DefaultValue ? "true" : "false";
                                }
                                else if (defaultValueType.IsValueType)
                                {
                                    defaultValue = parameter.DefaultValue.ToString();
                                }
                                else
                                {
                                    defaultValue = $"[{defaultValueType.Name}]";
                                }
                            }
                        }

                        formattedParameters.Add($"{parameterType} {parameterName}{(string.IsNullOrWhiteSpace(defaultValue) ? "" : $" = {defaultValue}")}");
                    }

                    var methodParameters = method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}{(p.HasDefaultValue ? $" = {p.DefaultValue}" : "")}");

                    traceBuilder
                        .Append(method.Name)
                        .Append("(")
                        .Append(string.Join(", ", formattedParameters))
                        .AppendLine(")");
                }

                index++;

                var fileName = frame.GetFileName();

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var filePrefix = new string(' ', linePrefix.Length + 2);

                    traceBuilder
                        .AppendLine($"{filePrefix}File: {fileName}")
                        .AppendLine($"{filePrefix}Line: {frame.GetFileLineNumber()}");
                }
            }

            if (exception.InnerException != null)
            {
                traceBuilder
                    .AppendLine()
                    .AppendLine($"{linePrefix}[Inner Exception]")
                    .AppendLine()
                    .AppendLine($"{linePrefix} Type: {exception.InnerException.GetType().Name}")
                    .AppendLine($"{linePrefix} Message: {exception.InnerException.Message}")
                    .AppendLine()
                    .Append(FormatStackTrace(exception.InnerException, indent + 4));
            }

            return traceBuilder.ToString();
        }
    }

    public class ExceptionHelper
    {
        public static Exception Get()
        {
            try
            {
                Throw();

                return new Exception("This shouldn't happen.");
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        public static void Throw()
        {
            new ExceptionHelper().Generate();
        }

        public void Generate(float bastards = 3000.235f)
        {
            StepOne();
        }

        public void StepOne(string piss = "reth")
        {
            try
            {
                StepTwo();

            }
            catch (Exception exception)
            {
                throw new ArgumentException("aaaaah hell no", nameof(piss), exception);
            }
        }

        public void StepTwo(HttpStatusCode status = HttpStatusCode.OK)
        {
            StepThree();
        }

        public void StepThree(int badgers = 12, string poop = null)
        {
            InnerClass.StepFour();
        }

        public static class InnerClass
        {
            public static void StepFour()
            {
                AnotherClass.StepFive();
            }

            public static class AnotherClass
            {
                public static void StepFive()
                {
                    throw new Exception("This was thrown purely to create a stack trace, what a disgrace!");
                }
            }
        }
    }
}
