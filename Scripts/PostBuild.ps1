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
$OutputLocation = "$($ProjectPath)\bin\$($ConfigurationName)\"
if (-not (Test-Path "$OutputLocation")) {
    Write-Error "Output Location path '$OutputLocation' does not exist"
    Exit 2
}

Add-Type -Path "$($NeosPath)\Neos_Data\Managed\PostX.dll"

#Copy Required dlls from Neos to OutputLocation 
Copy-Item -Path "$($NeosPath)\nml_libs\0Harmony.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\FrooxEngine.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\BaseX.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\CodeX.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\CloudX.Shared.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\Newtonsoft.Json.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\System.Text.Json.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\System.IO.Compression.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\System.Threading.Tasks.Dataflow.dll" -Destination $OutputLocation -ErrorAction Continue
Copy-Item -Path "$($NeosPath)\Neos_Data\Managed\System.Threading.Tasks.Extensions.dll" -Destination $OutputLocation -ErrorAction Continue


#Run PostX Process
[PostX.NeosAssemblyPostProcessor].GetMethod("Process").Invoke($null, @("$OutputLocation\ResonitePackageExporter.dll", "$OutputLocation"))