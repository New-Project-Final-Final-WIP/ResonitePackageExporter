#If Args are not provided, set them to defaults
$NeosPath = if ($args[0] -ne $null) { $args[0] } else {"C:\Program Files (x86)\Steam\steamapps\common\NeosVR"} #This is a fallback path incase one is not provided as an arg, ie by directly running this script
$ConfigurationName = if ($args[1] -ne $null) { $args[1] } else {"AutoPostX"} #Build output path under \bin\
#If run from the Scripts folder directly, go up one folder to the project directory
if ($PWD.Path.EndsWith("Scripts")) {
    $ProjectPath = Split-Path $PWD.Path -Parent
} else {
    $ProjectPath = $PWD.Path
}

#Verify output location exists
$OutputLocation = "$($ProjectPath)\bin\$($ConfigurationName)\net472\"
if (-not (Test-Path "$OutputLocation")) {
    Write-Error "Output Location path '$OutputLocation' does not exist"
    Exit 2
}

Add-Type -Path "$($NeosPath)\Neos_Data\Managed\PostX.dll"

#Run PostX Process
[PostX.NeosAssemblyPostProcessor].GetMethod("Process").Invoke($null, @("$OutputLocation\ResonitePackageExporter.dll", "$OutputLocation"))