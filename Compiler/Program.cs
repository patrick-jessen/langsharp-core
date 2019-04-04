using System.Collections.Generic;

class Program
{
    public Dictionary<string, Module> modules = new Dictionary<string, Module>();

    public Module GetModule(string name)
    {
        if(!modules.ContainsKey(name))
            modules[name] = new Module(name);
        
        return modules[name];
    }
}