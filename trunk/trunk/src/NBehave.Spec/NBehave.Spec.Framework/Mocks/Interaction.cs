using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;

namespace NBehave.Spec.Framework
{
    public class Interaction
    {
        public Interaction()
        {
        }

        public T CreateStrictMock<T>() where T : class
        {
            return CreateStrictMock<T>(null);
        }

        public T CreateStrictMock<T>(params object[] ctorArgs) where T : class
        {
            Type mockType = typeof (T);

            CodeCompileUnit ccu = new CodeCompileUnit();

            AddNamespace(mockType, ccu);

            CSharpCodeProvider provider = new CSharpCodeProvider();

            #region Enabled for debugging the codedom.

            string fileName = "__Proxy__" + mockType.Name + ".cs";

            IndentedTextWriter writer = new IndentedTextWriter(new StreamWriter(fileName, false));

            provider.GenerateCodeFromCompileUnit(ccu, writer, new CodeGeneratorOptions());

            writer.Close();

            #endregion

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.OutputAssembly = "ProxyAssembly." + mockType.Name;
            parameters.ReferencedAssemblies.Add(mockType.Assembly.Location);

            CompilerResults results = provider.CompileAssemblyFromDom(parameters, ccu);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError c in results.Errors)
                {
                    Console.WriteLine(c.ToString());
                    throw new ApplicationException("Could not create mock.");
                }
            }
            Assembly compiledAssembly = results.CompiledAssembly;

            T mock = null;

            try
            {
                mock = Activator.CreateInstance(compiledAssembly.GetExportedTypes()[0], ctorArgs) as T;
            }
            catch (MissingMethodException e)
            {
                throw new NoMatchingConstructorException("Cannot find a constructor that matches the arguments.", e);
            }

            return mock;
        }

        public T CreateDynamicMock<T>()
        {
            return default(T);
        }

        public T CreateDynamicMock<T>(params object[] ctorArgs)
        {
            return default(T);
        }

        public T CreatePartialMock<T>()
        {
            return default(T);
        }

        public T CreatePartialMock<T>(params object[] ctorArgs)
        {
            return default(T);
        }

        internal void RecordingFinished()
        {
        }

        private static void AddNamespace(Type mockType, CodeCompileUnit ccu)
        {
            CodeNamespace cns = new CodeNamespace(mockType.Namespace);

            AddType(mockType, cns);

            ccu.Namespaces.Add(cns);
        }

        private static void AddType(Type mockType, CodeNamespace cns)
        {
            CodeTypeDeclaration ctd = new CodeTypeDeclaration("__Proxy__" + mockType.Name);
            ctd.IsClass = true;
            ctd.Attributes = MemberAttributes.Public;
            ctd.BaseTypes.Add(mockType);

            if (mockType.IsInterface)
                AddDefaultConstructor(mockType, ctd);
            else
                AddConstructors(mockType, ctd);

            AddMethods(mockType, ctd);

            cns.Types.Add(ctd);
        }

        private static void AddMethods(Type mockType, CodeTypeDeclaration ctd)
        {
            foreach (MethodInfo mi in mockType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (mi.IsVirtual || mi.IsAbstract || mockType.IsInterface)
                {
                    CodeMemberMethod method = new CodeMemberMethod();
                    method.Name = mi.Name;

                    if (mockType.IsInterface)
                        method.Attributes = MemberAttributes.Public;
                    else
                        method.Attributes = MemberAttributes.Public | MemberAttributes.Override;

                    method.ReturnType = new CodeTypeReference(mi.ReturnType);

                    foreach (ParameterInfo pi in mi.GetParameters())
                    {
                        CodeParameterDeclarationExpression param = null;

                        if (pi.IsOut)
                        {
                            param = new CodeParameterDeclarationExpression(pi.ParameterType.GetElementType(), pi.Name);
                            param.Direction = FieldDirection.Out;
                            CodeVariableReferenceExpression variable = new CodeVariableReferenceExpression(param.Name);
                            CodeTypeReference typeRef = new CodeTypeReference(mi.ReturnType);
                            CodeDefaultValueExpression val = new CodeDefaultValueExpression(typeRef);
                            CodeAssignStatement assignment = new CodeAssignStatement(variable, val);
                            method.Statements.Add(assignment);
                        }
                        else
                        {
                            param = new CodeParameterDeclarationExpression(pi.ParameterType, pi.Name);
                        }
                        method.Parameters.Add(param);
                    }

                    if (mi.ReturnType != typeof (void))
                    {
                        CodeTypeReference typeRef = new CodeTypeReference(mi.ReturnType);
                        CodeMethodReturnStatement returnStatement =
                            new CodeMethodReturnStatement(new CodeDefaultValueExpression(typeRef));
                        method.Statements.Add(returnStatement);
                    }

                    ctd.Members.Add(method);
                }
            }
        }

        private static void AddDefaultConstructor(Type mockType, CodeTypeDeclaration ctd)
        {
            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;

            ctd.Members.Add(ctor);
        }

        private static void AddConstructors(Type mockType, CodeTypeDeclaration ctd)
        {
            foreach (
                ConstructorInfo ci in
                    mockType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                CodeConstructor ctor = new CodeConstructor();
                ctor.Attributes = MemberAttributes.Public;

                foreach (ParameterInfo parameter in ci.GetParameters())
                {
                    CodeParameterDeclarationExpression codeParam =
                        new CodeParameterDeclarationExpression(parameter.ParameterType, parameter.Name);
                    ctor.Parameters.Add(codeParam);
                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(codeParam.Name));
                }

                ctd.Members.Add(ctor);
            }
        }
    }
}