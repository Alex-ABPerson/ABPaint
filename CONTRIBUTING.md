# Overview

Welcome! These are the rules of the Quality Control Program of all of ABSoftware - if you memorize these rules, you memorize the rules for the whole of ABSoftware...

Code that doesn't follow these rules, will not be accepted. This was added because the code base of ABPaint has become unmanageable and has to change. For now, all the rules will be listed here, until they can be transferred to the website.

Here is a summary of all the rules, these are explained in more depth below:
- Always use "var" everywhere possible.
- Always use reverse logic. Reverse logic must have the "return" on a new line to make it clear that it is reverse logic (single-line "if" is fine everywhere else).
- Every one or two lines ***must*** be commented - you cannot have three lines on their own without any comments.
- In Extension, All methods ***must*** have at least one comment - and must be documented (with a \<summary>)
- When defining an object, ***always*** use the "object initialiser syntax" (explained below)
- Always use "!bool" or "bool", never use "== true".
- Don't use C# Type Names.
- Use PascalCasing and camelCasing when needed.
- Always write a summary of the commit into the comment.
- The commit name must follow this rule: If you put `When applied this will` at the start of the commit, it should make sense; if it doesn't, rephrase it to make sense.

If you see any ***code*** that does not follow the above rules - adjust it (don't worry about the commits - just make sure *you* follow those rules)

To clarify, here is each one in more detail, if you are still confused, please speak to the staff at the discord server.

### Using Var

Always use `var` when you can.

`var` just replaces the need to specify a type ***twice*** - if you don't know what `var` is, I suggest you look it up right now.

### Reverse Logic

In order to make code easy to read through, you should ***always*** use reverse logic.

Reverse Logic is where you take an "if" *that normally prevents the method from doing anything if it is false* and reverse it (== becomes != etc). And, afterwards, you add "return;" inside it, to make it so, if that "if" is true, the method exits.

Here is an example of a function that converts an object to a string ***without any checking***:

```cs
public string ObjToString(object obj) {
	var str = obj.ToString();

	// ...Do Other Stuff...
	
	return str;
}
```

Here is the same function but with some basic checking:

```cs
public string ObjToString(object obj) {
    if (obj != null) {
        var str = obj.ToString();

        // ...Do Other Stuff...
        
        return str;
    }
    return null;
}
```
Now, one problem with that code is that first of all, it is quite confusing to figure out *when* it will return null. It takes a while to figure out that it only returns null when `obj` is null. 

But, the main problem with this code is that, if you had to check about 5 or 6 things ***like that***, the code would become insanely messy and really hard to follow, "if" nested inside of "if" is always something you want to *avoid*, and it's not necessary here.

The solution is to take the logic of that "if", reverse it, and put a `return` inside of it instead:
```cs
public string ObjToString(object obj) {
    if (obj == null)
        return null;

    var str = obj.ToString();

    // ...Do Other Stuff...
    
    return str;
}
```

What you'll notice now is that it's really easy to see that if the object is null, it just won't bother. Unlike the code before, if we have lots of these, we can just arrange them one after another. Also, because we're putting the `return null` onto a new line, it is very clear that it is reversed logic.

**Note:** Obviously, that code would normally have comments all over it, but just to keep it simple, there are none.

### Commenting

This may sound like a *horrible* rule at first, but it's actually really important, and when you see the code that you make, it will actually feel really rewarding and the code will be *insanely* easy to understand and debug.

All code must have comments. You cannot have more than two lines without putting in a comment.

Here is some code that is taken from a WPF Explorer Program, this is what the code looks like without commenting:
```cs
private void Window_Loaded(object sender, RoutedEventArgs e) {
        var logicalDrives = Directory.GetLogicalDrives();

        for (int i = 0; i < logicalDrives.Length; i++)
        {
            TreeViewItem item = new TreeViewItem()
            {
                Header = logicalDrives[i],
                Tag = logicalDrives[i]
            };
            item.Items.Add(null);
            item.Expanded += Item_Expanded;
            MainTreeView.Items.Add(item);
        }
}
```

It may seem *fairly* easy to read, but imagine a function with about 150 lines in it... (like ABPaint), you would get lost quickly there and spend way to much time just figuring out where everything is (trust me, I know).

There is a much better way of just writing code overall, and it's using comments, take a look at the edited version of this code - now not only is it obvious what the code is doing, but it's also obvious ***why*** it is being done:
```cs
private void Window_Loaded(object sender, RoutedEventArgs e) {
     // Gets all the drives
     var logicalDrives = Directory.GetLogicalDrives();

     for (int i = 0; i < logicalDrives.Length; i++)
    {
        // Create the item, with the current drive as the "Header" and "Tag"
        TreeViewItem item = new TreeViewItem()
        {
            Header = logicalDrives[i],
            Tag = logicalDrives[i]
        };

        // Add a null object, to allow the item to actually be expanded
        item.Items.Add(null);

        // Add a expanded event
        item.Expanded += Item_Expanded;

        // Add the new item to the main TreeView
        MainTreeView.Items.Add(item);
    }

}
```

As you can tell, when you look through that, you know exactly what's going on, and, I don't know if it's just me... but it really does look ***much*** better.

***Every*** function must have at least one comment in it... ***unless it is just returning something, and not doing ANYTHING else (like performing some maths).***

### Object Initializer Syntax

This is another key part of writing simple and easy to read code - and was actually used in the example above from comments.

So, the "Object Initializer Syntax" is a better method of defining a certain object - take a look at this code; in this code, the class "Foo" has a `Property1` and a `Property2`,  and we need to set them.
```cs
// Create the item
var obj = new Foo();

obj.Property1 = "Hello, life!"
obj.Property2 = 42;
```

The first problem is... if we do it like that, how are we meant to comment it - are you just going to add "// Setting Property1 of obj", because, that's a bit stupid considering that's obvious.

But, the main problem is that the code setting those properties begins to blend in with the other code... and so, it's hard to tell what is just setting the data in the object and what is actually doing the important code we're interested in. If you use the "Object Initialiser Syntax", it will be really clear what code is setting the properties and what code is doing other things.

So, the "Object Initialiser Syntax" is really simple - where you are saying "new"... just add some curly braces after it, and place in each of the properties with their values in there, use commas instead of semicolons. Here is the same code but with that:
```cs
// Create the item, with the below values
var obj = new Foo() {
	Property1 = "Hello, life!",
	Property2 = 42
};
```

The rule about comments every two lines does not apply inside the "Object Initialiser Syntax" but if something *confusing* is being done where it isn't *clear* what it is actually setting it to, you must add something to the comment ***above*** it, like "Create the item, with the path in the 'Header' and 'Tag'".

Also, make sure that you don't leave trailing commas, that *could* lead to possibly it not being accepted (it doesn't throw an error in that situation). 

### Boolean Checking

Booleans cannot be checked using `== false` or `== true`, you must use things such as `(!bool)` or `(bool)`.

### C# Type Names

You can't use things such as `String`, `Byte` or `Boolean`. You must use `string`, `byte` or `bool`. This applies to anything like this.

Integers are different - if you want to define an `Int32`, you ***must*** use `int`, however, if you want to use define a 16-bit or 64-bit integer, that's fine.

### Casing

All public fields/properties ***must*** be PascalCase - PascalCase is where the first letter of each word must have a capital letter - for example, `CurrentlyDragging`.

All private or local fields/properties, on the other hand, must have camelCase, which is where every word has a capital letter at the start, ***except the first one.***

Finally, anything else should be PascalCase - classes, methods etc. Do not name a class using camelCase - like this: `public class cropTool`; instead, say `public class CropTool`, whether public or not. 

For example, don't name a method `paintPreview` - instead, name it `PaintPreview`.

### Commits

Commits ***must*** be titled using a certain rule. You should be able to take `When applied this will` and place it at the start of the commit and it should still make sense.

This means that a name like `Added CropTool` is not good enough. You get `When applied this will Added CropTool`, which doesn't make any sense. Instead, you should say: `Add CropTool`, now it makes sense with `When applied this will`.

-------

There you go! That's it, alright, ***now*** you are ready to start contributing to ABSoftware! As long as you follow these rules, your code should be clean, clear and readable