# LibNoise Designer

Recently I was working on a project using a c# port of the [LibNoise Library](http://libnoise.sourceforge.net/index.html). LibNoise itself is very straightforward coherent noise library and very easy to use, however I found testing and debugging to be a bit of a pain with a lot of trial and error, and I started looking for something to help with quickly testing settings and module options. Unable to find any better solution, I decided to build my own utility, and after a weekend of fiddling, LibNoise Designer was born!

![Libnoise Designer Interface](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview1.png)

## What does this appliction do: 
Gives the user a visual interface to interact with LibNoise. The user can save the script in an XML format for use with the designer, or export to a c# class file that can be dropped into the development environment. Preview images can be generated and saved as png's.

## What it doesn't do: 
Anything else.

## How to use the Designer:
LibNoise Designer is pretty simple. To add libnoise Modules, right click anywhere on the designer workspace, select "Create Libnoise Node", and select an operator or generator to place on the workspace. To link modules together, click and drag from the output links to any input link.

![Adding additional modules](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview2.png)

For any module diagram to validate and export/save properly, the nodes must eventually end at the  green "Final" node. This is the last step of any libnoise designer diagram. Nodes not connected to the final node will not be saved or exported.

You can double-click on a node to expand it, allowing you to enter a unique name for the module.

![Libnoise Designer Interface](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview3.png)
_Setting the name of a module by double-clicking the module node. You can trace the path from there too!_

You can select a single node by single left-clicking. Holding left-ctrl and dragging will select multiple nodes. Pressing the delete key with a node selected will remove it from the workspace. Otherwise, to delete a node or a connection, mouse over the node and highlight the little pair of scissors delete icon which pops up.

Zooming in or out can be done with the mouse wheel or the relevant buttons.

If you have more nodes then a workspace can fit, click the "resize workspace" button to fit the working area to your designer layout.

The Validate button will ensure the module and nodes connected to the "Final" node are valid and will generate working code.

Bugs: Many, I'm sure. It's a quick development tool that I thought others might get some use out of, but it is far from perfect and not meant for any serious commercial use.

## Future enhancements: 
Maybe a few things to add... export to additional programming languages where LibNoise is used, to start. Some additional utilities to aid in the diagrams, such as bookmarks and grouping. That sort of thing.

## Screenshots

![Libnoise Designer Interface](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview4.png)
Export to c# code.

![Libnoise Designer Interface](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview7.png)
The generated XML for use with LibNoise Designer

![Libnoise Designer Interface](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview5.png)
Preview from the "Detailed" LibNoise Example

![Libnoise Designer Interface](https://raw.githubusercontent.com/modopotato/LibnoiseDesigner/master/ExampleImages/lnd_preview6.png)
Styling a preview with the "World" style

## Libraries and Credits
LibNoise designer is built in c# and WPF, and utilizes the following packages:

NetworkView: A WPF custom control for visualizing and editing networks, graphs and flow-charts:
http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a

Though I've modified pieces of it, the designer itself started with Ashley Davis excellent WPF project on Code Project. The project is an excellent example and tutorial with WFP development in general, but more importantnly a pretty nice looking and easy to work with diagram control. This library is released under the The Code Project Open License (CPOL) 1.02 (http://www.codeproject.com/info/cpol10.aspx)

Extended WPF Toolkit:
http://wpftoolkit.codeplex.com/

XCeed's Extended WPF toolkit was used for the property grid panel primarily. It's an excellent batch of tools and controls to have for any WPF project though. I used the community edition, which is released under the Microsoft Public License (http://opensource.org/licenses/MS-PL)

LibNoise:
http://libnoise.sourceforge.net/index.html

Libnoise, originally developed for c++ and released under the GNU General Public License, version 2 (http://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html) From their website: libnoise is a portable C++ library that is used to generate coherent noise, a type of smoothly-changing noise. libnoise can generate Perlin noise, ridged multifractal noise, and other types of coherent-noise.

For use in this project, I used a c# port from ricardojmendez on GitHub (https://github.com/ricardojmendez/LibNoise.Unity) that was originally developed for use with Unity, and released under the GNU Lesser General Public License (https://www.gnu.org/licenses/lgpl.html).

LibNoise Designer is released under the LGPL 3.0 Licence (http://www.gnu.org/licenses/lgpl.html)

The application icon came from http://findicons.com/pack/1580/devine_icons_part_2 created by http://ipapun.deviantart.com/

LibNoise designer Copyright (C) 2015 Dylan Hemsworth