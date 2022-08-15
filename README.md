# Kros.SourceGenerators.PropertyAccessors
This package contains [C# source generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) which enhances targeted classes with methods for obtaining values of their properties via their names, without the need for reflection.

## When to use this source generator?
When you need to obtain class object properties via their names and you do not want to use reflection.

## How it works?
You mark the target class as `partial` (without this modifier source generator cannot enhance the class). Then you decorate the class with `[PropertyValueProvider]` attribute. Source generator automatically generates methods `GetPropertyValue` and `GetAllPropertyValues`.
- `GetPropertyValue` accepts property name as its parameter and returns property value (if property was found) or `null`
- `GetAllPropertyValues` returns dictionary with property names as keys and their values as values

The source generator **also supports nesting**. If the target class contains one or more reference-value properties, source generator attempts to enhance their type-defining classes as well. The only requirement is that the property's type defining class must also be marked as `partial` and that it is contained within the same namespace (in order for the source generator to access its definition). The type-defining class of the property doesn't even have to be marked with `[PropertyValueProvider]` attribute (but it is recommended).

## Examples
Retrieving property value:
```csharp
[PropertyValueProvider]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var newPerson = new Person() { Name = "John Doe", Age = 38 };
Console.WriteLine($"{newPerson.GetPropertyValue("Name")} is {newPerson.GetPropertyValue("Age")} years old.");
// "John Doe is 38 years old."
```

Retrieving all property values:
```csharp
[PropertyValueProvider]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var newPerson = new Person() { Name = "John Doe", Age = 38 };
foreach (var property in newPerson.GetAllPropertyValues())
{
    Console.WriteLine($"{property.Key} = {property.Value}");
}
// "Name = John Doe"
// "Age = 38"
```

Retrieving nested property value:
```csharp
[PropertyValueProvider]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

public partial class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

var newPerson = new Person()
{
    Name = "John Doe",
    Age = 38,
    Address = new Address() { Street = "5th Avenue", City = "New York" }
};
Console.WriteLine($"{newPerson.GetPropertyValue("Name")} lives in {newPerson.GetPropertyValue("Address.City")} on {newPerson.GetPropertyValue("Address.Street")}.");
// "John Doe lives in New York on 5th Avenue."
```

Retrieving nested property value also supports custom hierarchy delimiter:
```csharp
[PropertyValueProvider]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

public partial class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

var newPerson = new Person()
{
    Name = "John Doe",
    Age = 38,
    Address = new Address() { Street = "5th Avenue", City = "New York" }
};
Console.WriteLine($"{newPerson.GetPropertyValue("Name")} lives in {newPerson.GetPropertyValue("Address/City", '/')} on {newPerson.GetPropertyValue("Address/Street", '/')}.");
// "John Doe lives in New York on 5th Avenue."
```
test
