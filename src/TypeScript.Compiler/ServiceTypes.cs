using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeScript.Compiler
{
    public class ServiceTypes
    {
        private readonly DataTypes _dataTypes;

        readonly Dictionary<string, TypeScriptType> _hubCache;
        readonly Dictionary<string, string> _hubAliasesCache;
        readonly Dictionary<string, TypeScriptType> _clientCache;

        public Dictionary<string, TypeScriptType> HubCache { get { return _hubCache; } }
        public Dictionary<string, TypeScriptType> ClientCache { get { return _clientCache; } }
        public Dictionary<string, string> HubAliasesCache { get { return _hubAliasesCache; } }

        public ServiceTypes(DataTypes dataTypes)
        {
            _dataTypes = dataTypes;
            _hubCache = new Dictionary<string, TypeScriptType>();
            _clientCache = new Dictionary<string, TypeScriptType>();
            _hubAliasesCache = new Dictionary<string, string>();
        }

        string CamelCase(string s)
        {
            return s[0].ToString().ToLower() + s.Substring(1);
        }

        TypeScriptType MakeHubInterface(Type hubType)
        {
            var name = "I" + hubType.Name;
            var cName = name + "Client";

            var serverDeclaration = "interface " + name + " {\n";
            var clientDeclaration = "interface " + cName + " {\n";
            var count = 0;
            var sep = "";
            var hubAttribute = "";
            hubType.GetMethods()
                .Where(mi => mi.DeclaringType.Name == hubType.Name).ToList()
                .ForEach(mi =>
                {
                    serverDeclaration += "    " + CamelCase(mi.Name) + "(";
                    clientDeclaration += "    " + CamelCase(mi.Name) + "(";


                    var retTS = _dataTypes.GetTypeScriptType(mi.ReturnType);
                    var retServerType = retTS.Name == "System.Void" ? "void" : "IPromise<" + retTS.Name + ">";
                    var retClientType = retTS.Name == "System.Void" ? "void" : "" + retTS.Name + "";
                    mi.GetParameters().ToList()
                    .ForEach((pi) =>
                    {
                        sep = (count != 0 ? ", " : "");
                        count++;
                        var tst = _dataTypes.GetTypeScriptType(pi.ParameterType);
                        serverDeclaration += sep + pi.Name + ": " + tst.Name;
                        clientDeclaration += sep + pi.Name + ": " + tst.Name;
                    });

                    serverDeclaration += "): " + retServerType + ";\n";
                    clientDeclaration += "): " + retClientType + ";\n";
                    count = 0;
                });

            serverDeclaration += "}";
            clientDeclaration += "}";


            hubAttribute = hubType.CustomAttributes
                .Where(ad => ad.AttributeType.Name == "HubNameAttribute")
                .Select(ad => ad.ConstructorArguments.FirstOrDefault().Value.ToString())
                .FirstOrDefault();

            var ret = new TypeScriptType
            {
                Name = name,
                Declaration = serverDeclaration
            };

            _hubCache.Add(name, ret);

            _hubAliasesCache.Add(name, hubAttribute);

            _clientCache.Add(cName, new TypeScriptType
            {
                Name = cName,
                Declaration = clientDeclaration
            });

            return ret;
        }

        /*
         declare var client: IClient // To be filled by the user...
         declare var server: IServer
         
         */

        public void AddHubsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();

            var hubs = types.Where(t => t.BaseType != null && t.BaseType.Name == "BaseHub`1").ToList();

            hubs.ForEach(t => MakeHubInterface(t));
        }
    }
}

