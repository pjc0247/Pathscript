# Pathscript
Simple script to retrive value via its path

```cs
var foo = new Foo();
foo.bar = 1234;

// 1234
PScript.Eval(foo, "bar");
```
