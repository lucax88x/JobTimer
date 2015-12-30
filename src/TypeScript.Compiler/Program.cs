using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TypeLite;
using TypeLite.Net4;

namespace TypeScript.Compiler
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("Writing WebApi interfaces");

                const string hubsPathDll =
                    @"..\..\..\JobTimer.WebApplication.Hubs\bin\debug\JobTimer.WebApplication.Hubs.dll";
                const string hubsOutPath = @"..\..\..\JobTimer.WebApplication\src\hubs\hubs.ts";

                const string webApiPathDll =
                    @"..\..\..\JobTimer.WebApplication.ViewModels\bin\debug\JobTimer.WebApplication.ViewModels.dll";
                const string typeScriptPathDll =
                    @"..\..\..\JobTimer.WebApplication.TypeScript\bin\debug\JobTimer.WebApplication.TypeScript.dll";

                const string webapiGeneratedOutputPath =
                    @"..\..\..\JobTimer.WebApplication\src\generated\webapi.d.ts";
                const string constantsGeneratedOutputPath =
                    @"..\..\..\JobTimer.WebApplication\src\generated\constants.ts";
                const string enumsGeneratedOutputPath =
                    @"..\..\..\JobTimer.WebApplication\src\generated\enums.ts";


                Assembly.LoadFrom(webApiPathDll);
                Assembly.LoadFrom(typeScriptPathDll);

                
                var definitions = TypeLite.TypeScript.Definitions().ForLoadedAssemblies();
                definitions.WithReference("constants.ts");

                var properties = definitions.Generate(TsGeneratorOutput.Properties);                
                var constants = definitions.Generate(TsGeneratorOutput.Constants);
                var enums = definitions.Generate(TsGeneratorOutput.Enums);

                File.WriteAllText(webapiGeneratedOutputPath, properties);
                File.WriteAllText(constantsGeneratedOutputPath, constants);
                File.WriteAllText(enumsGeneratedOutputPath, enums);

                Console.WriteLine("WebApi interfaces written!");

                Console.WriteLine("Writing Hubs Interfaces");

                var signalRModule = new SignalRModule();

                signalRModule.Create("JobTimer.Hubs", hubsPathDll, hubsOutPath);
                Console.WriteLine("Hubs Interfaces written!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return 0;
        }
    }

    //public static class TypeLiteExtensions
    //{
    //    private static readonly Regex _regex = new Regex("module (\\S+) {(.*?})\r\n}", RegexOptions.Singleline | RegexOptions.CultureInvariant);

    //    public static string CleanModules(string typeScript)
    //    {
    //        var sb = new StringBuilder();

    //        var matchCollection = _regex.Matches(typeScript);
    //        foreach (Match match in matchCollection)
    //        {
    //            if (match.Groups.Count == 3)
    //            {
    //                string moduleName = match.Groups[1].Value;
    //                string moduleContent = match.Groups[2].Value;
    //                if (!string.IsNullOrEmpty(moduleContent))
    //                {
    //                    sb.AppendFormat("module {0} {{{1}}}", moduleName, moduleContent);
    //                }
    //            }
    //        }
    //        return sb.ToString();
    //    }
    //}
}
