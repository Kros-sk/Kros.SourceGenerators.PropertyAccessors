using Kros.SourceGenerators.PropertyAccessorsGenerator.Demo;

var classA = new SimpleClass()
{
    BoolProperty = true,
    IntProperty = 42,
    StringProperty = "Hello World",
    DecimalProperty = -123.456M,
    DateTimeProperty = DateTime.Now
};

Console.WriteLine("SIMPLE CLASS EXAMPLES:");
Console.WriteLine("######################");
Console.WriteLine("Accessing value-type properties by name:");
Console.WriteLine("-----------------------------------------");
Console.WriteLine($"Value of {nameof(SimpleClass.BoolProperty)} is {classA.GetPropertyValue(nameof(SimpleClass.BoolProperty))}.");
Console.WriteLine($"Value of {nameof(SimpleClass.IntProperty)} is {classA.GetPropertyValue(nameof(SimpleClass.IntProperty))}.");
Console.WriteLine($"Value of {nameof(SimpleClass.StringProperty)} is {classA.GetPropertyValue(nameof(SimpleClass.StringProperty))}.");
Console.WriteLine($"Value of {nameof(SimpleClass.DecimalProperty)} is {classA.GetPropertyValue(nameof(SimpleClass.DecimalProperty))}.");
Console.WriteLine($"Value of {nameof(SimpleClass.DateTimeProperty)} is {classA.GetPropertyValue(nameof(SimpleClass.DateTimeProperty))}.");
Console.WriteLine();
Console.WriteLine("Accessing non-existing property:");
Console.WriteLine("--------------------------------");
Console.WriteLine($"Value of property without name is {classA.GetPropertyValue(string.Empty) ?? "null"}.");
Console.WriteLine($"Value of NonExistingProperty is {classA.GetPropertyValue("NonExistingProperty") ?? "null"}.");
Console.WriteLine();
Console.WriteLine("Accessing properties via dictionary:");
Console.WriteLine("-------------------------------------------");
foreach (var property in classA.GetAllPropertyValues())
{
    Console.WriteLine($"Key = {property.Key}; Value = {property.Value}");
}
Console.WriteLine();
Console.WriteLine();

var classB = new ComplexClassWithPartial()
{
    BoolProperty = false,
    IntProperty = 81,
    StringProperty = "Hello Galaxy",
    DecimalProperty = 596.789M,
    DateTimeProperty = DateTime.Now,
    ClassProperty = new()
    {
        BoolProperty = true,
        IntProperty = -8616,
        StringProperty = "Hello Friend",
        DecimalProperty = -79.48M,
        DateTimeProperty = DateTime.Now + TimeSpan.FromDays(1)
    }
};

Console.WriteLine("COMPLEX CLASS EXAMPLES:");
Console.WriteLine("#######################");
Console.WriteLine("Accessing reference-type property by name:");
Console.WriteLine("------------------------------------------");
Console.WriteLine($"Value of {nameof(ComplexClassWithPartial.ClassProperty)} is {classB.GetPropertyValue(nameof(ComplexClassWithPartial.ClassProperty))}.");
Console.WriteLine();
Console.WriteLine("Accessing nested property:");
Console.WriteLine("--------------------------");
string nestedPropertyName = $"{nameof(ComplexClassWithPartial.ClassProperty)}.{nameof(NestedClassWithPartial.IntProperty)}";
Console.WriteLine($"Value of {nestedPropertyName} is {classB.GetPropertyValue(nestedPropertyName)}.");
Console.WriteLine();
Console.WriteLine("Accessing nested property with incorrectly ended hierarchy:");
Console.WriteLine("-----------------------------------------------------------");
nestedPropertyName = $"{nameof(ComplexClassWithPartial.ClassProperty)}.";
Console.WriteLine($"Value of {nestedPropertyName} is {classB.GetPropertyValue(nestedPropertyName)}.");
Console.WriteLine();
Console.WriteLine("Accessing nested property with custom hierarchy separator:");
Console.WriteLine("----------------------------------------------------------");
nestedPropertyName = $"{nameof(ComplexClassWithPartial.ClassProperty)}/{nameof(NestedClassWithPartial.IntProperty)}";
Console.WriteLine($"Value of {nestedPropertyName} is {classB.GetPropertyValue(nestedPropertyName, '/')}.");
Console.WriteLine();
Console.WriteLine("Accessing non-existing nested properties:");
Console.WriteLine("-----------------------------------------");
string[] nonExistingProperties = new[]
{
    $"",
    string.Empty,
    $".",
    $"....",
    $"NonExistingProperty.{nameof(NestedClassWithPartial.IntProperty)}",
    $"{nameof(ComplexClassWithPartial.ClassProperty)}.NonExistingProperty"
};
foreach (var property in nonExistingProperties)
{
    Console.WriteLine($"Value of {property} is {classB.GetPropertyValue(property) ?? "null"}.");
}
Console.WriteLine();
Console.WriteLine();

var classC = new ComplexClassWithoutPartial()
{
    BoolProperty = true,
    IntProperty = -819,
    StringProperty = "Hello People",
    DecimalProperty = 6615.489M,
    DateTimeProperty = DateTime.Now,
    ClassProperty = new()
    {
        BoolProperty = false,
        IntProperty = 4615,
        StringProperty = "Hello Country",
        DecimalProperty = 53.59M,
        DateTimeProperty = DateTime.Now + TimeSpan.FromDays(2)
    }
};

Console.WriteLine("COMPLEX CLASS WITH NESTED CLASS WITHOUT PROPERTY METHODS EXAMPLES:");
Console.WriteLine("##################################################################");
Console.WriteLine("Accessing reference-type property by name:");
Console.WriteLine("------------------------------------------");
Console.WriteLine($"Value of {nameof(ComplexClassWithoutPartial.ClassProperty)} is {classC.GetPropertyValue(nameof(ComplexClassWithoutPartial.ClassProperty))}.");
Console.WriteLine();
Console.WriteLine("Accessing nested property returns null, because hierarchy could not be supported:");
Console.WriteLine("---------------------------------------------------------------------------------");
nestedPropertyName = $"{nameof(ComplexClassWithoutPartial.ClassProperty)}.{nameof(NestedClassWithoutPartial.IntProperty)}";
Console.WriteLine($"Value of {nestedPropertyName} is {classC.GetPropertyValue(nestedPropertyName) ?? "null"}.");