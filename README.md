An uninstaller for ClickOnce applications
===========================

### Introduction

This project is forked from Wunder.ClickOnceUninstaller converted into .NET Class Library project. Custom action for WiX is discontinued.

### About ClickOnceUninStaller

Apparently, ClickOnce installations can't be removed silently. The ClickOnce uninstaller always shows a "Maintainance" dialog, requiring user interaction. We wanted this switch to be integrated into our new installer, invisible to the user.

The ClickOnceUninstaller uninstaller imitates the actions performed by the ClickOnce uninstaller, removing files, registry entries, start menu and desktop links for a given application.

It automatically resolves dependencies between installed components and removes all of the applications's components which are not required by other installed ClickOnce applications.

### Usage

##### .NET

    var uninstallInfo = UninstallInfo.Find("Application Name");
    if (uninstallInfo == null)
    {
        var uninstaller = new Uninstaller();
        uninstaller.Uninstall(uninstallInfo);
    }

### Changes

Few improvement and tweaking done on the original project as follow:
- Handle application with no suite folder defined. 

## License

The source code is available under the [MIT license](http://opensource.org/licenses/mit-license.php).
