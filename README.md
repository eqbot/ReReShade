# ReReShade
Unhooks GShade, copies its shaders, and rehooks ReShade to them.

# WARNING
There's currently an issue where uninstalling GShade after this process removes ReShade and the copied shaders.
To work-around, copy dxgi.dll and the shaders folders somewhere else before uninstalling, then copy them back. I'll add uninstalling as a step in the process later so that you don't have to manually do this, but for now that's the work-around.
