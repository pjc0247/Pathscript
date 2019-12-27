# Pathscript
Simple script to retrive value via its path

```cs
var foo = new Foo();
foo.bar = 1234;

// 1234
PScript.Eval(foo, "bar");
// also this can be accepted
foo.Eval<int>("bar");
```


__array__
```cs
PScript.Eval(foo, "player.inventory.items[1].name");
```
