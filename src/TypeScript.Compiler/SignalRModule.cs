using System.IO;
using System.Linq;
using System.Reflection;

namespace TypeScript.Compiler
{
    public class SignalRModule
    {
        public void Create(string nameSpace, string path, string outPath)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var assembly = Assembly.LoadFrom(path);

                var dataTypes = new DataTypes();
                var serviceTypes = new ServiceTypes(dataTypes);                

                serviceTypes.AddHubsFromAssembly(assembly);

                var dec = "";
                dec += string.Format("module {0}{{\n\n", nameSpace);

                dec += "/*\n   Client interfaces:\n\n */ \n\n";
                foreach (var k in serviceTypes.ClientCache.Keys)
                {
                    var dt = serviceTypes.ClientCache[k];
                    if (dt.Declaration != "")
                    {
                        dec += dt.Declaration + "\n";
                    }
                }

                dec += "\n//Promise interface\n";
                dec += "interface IPromise<T> {\n";
                dec += "    done(cb: (result: T) => any): IPromise<T>;\n";
                dec += "    error(cb: (error: any) => any): IPromise<T>;\n";
                dec += "}\n";
                if (dataTypes.HasDictionary)
                {
                    dec += "\n//Generic dictionary interface\n";
                    dec += "interface IDictionary<T> {\n    [key: any]: T;\n}\n";
                }
                if (dataTypes.TupleTypesByCount.Any())
                {
                    dec += "\n// Tuple types\n";
                    dataTypes.TupleTypesByCount.ForEach(c =>
                    {
                        dec += "interface Tuple" + c + "<";
                        for (var i = 0; i < c; i++)
                        {
                            dec += "T" + i + (i == c - 1 ? "" : ", ");
                        }
                        dec += "> {\n";
                        for (var i = 0; i < c; i++)
                        {
                            dec += "    Item" + i + ": T" + i + ";\n";
                        }
                        dec += "}\n";
                    });
                }

                dec += "\n// Data interfaces \n";
                foreach (var k in dataTypes.Cache.Keys)
                {
                    var dt = dataTypes.Cache[k];
                    if (dt.Declaration != "")
                    {
                        dec += dt.Declaration + "\n";
                    }
                }

                dec += "\n// Hub interfaces \n";
                foreach (var k in serviceTypes.HubCache.Keys)
                {
                    var dt = serviceTypes.HubCache[k];
                    if (dt.Declaration != "")
                    {
                        dec += dt.Declaration + "\n";
                    }
                }

                dec += "\n// Generated proxies \n";

                foreach (var k in serviceTypes.HubCache.Keys)
                {
                    var dt = serviceTypes.HubCache[k];
                    var hubName = serviceTypes.HubAliasesCache[k];
                    var hubAttribute = serviceTypes.HubAliasesCache[k];

                    dec += "export interface " + dt.Name + "Proxy {\n";
                    dec += "     server: " + dt.Name + ";\n";
                    dec += "     client: " + dt.Name + "Client;\n";
                    dec += "}\n";
                }

                dec += "}";

                File.WriteAllText(outPath, dec);                
            }
        }
    }
}
