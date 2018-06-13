# Testing



This is a guide on how to test **ABPaint**. Very commonly, **ABPaint** gets tested slightly... however... sometiems, it isn't tested enough and certain things fail to be tested.

Here are is the summary of things that should be tested when testing **ABPaint**:

***(More information will be available below the summary)***

- **Every Tool** - Every Tool should be tested, properties and drawing - ***make sure you can draw the same tool rapidy and more than once.***
- **Moving** - Make sure you can *move* elements, this means dragging them around - ***must be done for every tool.***
- **Resizing** - Make sure you can *resize* elements, from all corners - ***must be done for every tool.***
- **Deleting** - Make sure you can delete elements - ***must be done for every tool.***
- **Interactivity** - Make sure that maximizing and minimizing or anything similar does not have any effect on the program.
- **New Image** - Make sure you can create a new image.
- **Opening/Saving** - The Open/Save/Save As should all be tested.
- **Importing/Exporting** - Importing and exporting must be tested and the final result of the export must also be tested.
- **Cut/Copy/Paste** - Make sure you can copy/paste (and cut)
- **Keyboard Shortcuts** - Make sure that keyboard shortcuts work.

### Every Tool

Make sure you go through every tool - and do the following things to it:

- **Draw** - Make sure you can draw the tool, do this five times and *rapidly*, make sure it does not crash.
- **Move** - Make sure you can move the element the tool produced around.
- **Resize** - Make sure you can change the size of the element produced by the tool.
- **Delete** - Make sure you can delete the element that this element produces.

### Interactivity

The window can be resized and things like that, it is important to make sure that the application continues running no matter what happens to the window.

It is also important to check the ***about window***, make sure the about window scales properly (or can't be scaled) and works just as you would expect it to.

Also, check the icons... Make sure every window has the *ABPaint* icon.

### File Communications

Make sure that opening/saving and importing/exporting work as expected. A open/save dialog box should open up and you should be able to open/save a file.

Even if that works, make sure of a few things: Make sure that the "ComboBox" (the thing that lets you choose what to save it as) is, by default, correct. If you are *opening/saving* it should default to *.abpt*. If you are *importing* it should be *All Files*. And, if you are *exporting*, it should be *.png*.

It is also important to to make sure ***creating a new image*** works. You should be able to create a new image, with your own size and the old image should disappear. Make sure all elements are definately gone by attempting to ***select*** where they were.

### Clipboard

Make sure you can copy/paste elements. If you go to Edit -> Copy, the element should be copied into the clipboard. And make sure that you can paste the element back in. Also, make sure you can paste it into an external program - it should paste as an image.

Also, make sure you can paste an image from the clipboard into *ABPaint*.
