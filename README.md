# CSNS - CS Not Source
## Walkthrough
### First Steps
Add references in the main project to CSNS csproj.
### Implementations
Set a string "path" with the path where your files will stay, or the file itself to be loaded, then we can call the below static methods:

```csharp
var assembly = await CSNSMemory.LoadAssemblyAsync(path, "MyAssembly");
```
to load an path or a file and return the assembly at the same time we will store the result assembly in a "ram" schema to be available in another time.

```csharp
var assembly = CSNSMemory.GetAssembly("MyAssembly");
```

with the next operations, we can load an static method from assembly who be loaded from the script folder (or the itself file)

```csharp
var Soma = CSNSStaticLoader.LoadMethod(assembly, "Matematica.Mate.Soma");
```

then we execute the loaded method by calling the delegated loaded above

```csharp
var result = Soma(2, 3);
Console.WriteLine($"Static method Soma result: {result.ToString()}");
```

So if we want to load and construct a class defined in the CS Scripts?
Lets do this

```csharp
var classe = CSNSStaticLoader.InstanceClass(assembly, "Matematica.Matema", new object[] { 2 });
var resultado = classe.Soma(4);
Console.WriteLine($"Method Soma from instanciated class result: {resultado.ToString()}");
await classe.SubtractAsync();
```

So we instantiate an object based on a class written in scripts folder, and manipulate the object using the own methods defined before.

We can insert a static void Task() method inside a class, so we can execute automation scripts after load the file.
