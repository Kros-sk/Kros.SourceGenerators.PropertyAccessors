namespace Kros.SourceGenerators.PropertyAccessorsGenerator.Demo
{
    [PropertyValueProvider]
    internal partial class SimpleClass
    {
        public bool BoolProperty { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; } = "";
        public decimal DecimalProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
    }

    [PropertyValueProvider]
    internal partial class ComplexClassWithPartial
    {
        public bool BoolProperty { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; } = "";
        public decimal DecimalProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public NestedClassWithPartial? ClassProperty { get; set; }
    }

    [PropertyValueProvider]
    internal partial class ComplexClassWithoutPartial
    {
        public bool BoolProperty { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; } = "";
        public decimal DecimalProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public NestedClassWithoutPartial? ClassProperty { get; set; }
    }

    internal partial class NestedClassWithPartial
    {
        public bool BoolProperty { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; } = "";
        public decimal DecimalProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public NestedClassWithPartial? ClassProperty { get; set; }
    }

    internal class NestedClassWithoutPartial
    {
        public bool BoolProperty { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; } = "";
        public decimal DecimalProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public NestedClassWithPartial? ClassProperty { get; set; }
    }
}
