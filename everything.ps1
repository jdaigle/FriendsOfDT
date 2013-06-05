properties {
	$TargetFramework = "net-4.0"
}

$baseDir  = resolve-path .
$buildBase = "$baseDir\build\"
$testResultsBase = "$baseDir\test-results\"
$deployBase = "$baseDir\deploy\"
$coreSlns = "$baseDir\friendsofdt.sln"
$packageProjects = "$baseDir\FODT\FODT.csproj"
$toolsDir = "tools"
$nunitexec = "tools\nunit\nunit-console.exe"
$script:nunitTargetFramework = "/framework=4.0";

include buildutils.ps1

task InitEnvironment {
	if($script:isEnvironmentInitialized -ne $true){
		if ($TargetFramework -eq "net-4.0") {
			$netfxInstallroot =""
			$netfxInstallroot =	Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot'
			$netfxCurrent = $netfxInstallroot + "v4.0.30319"
			$script:msBuild = $netfxCurrent + "\msbuild.exe"
			echo ".Net 4.0 build requested - $script:msBuild"
			$script:ilmergeTargetFramework  = "/targetplatform:v4," + $netfxCurrent	
			$script:msBuildTargetFramework ="/p:TargetFrameworkVersion=v4.0 /ToolsVersion:4.0"	
			$script:nunitTargetFramework = "/framework=4.0";
			$script:isEnvironmentInitialized = $true
		}	
	}
}
 
task Clean -depends InitEnvironment {
  if (Test-Path $buildBase) {    
    Delete-Directory $buildBase		
  }
  if (Test-Path $testResultsBase) {
    Delete-Directory $testResultsBase		
  }
  foreach ($slnFile in $coreSlns) {
      exec { &$script:msBuild $slnFile /m /target:Clean }
  }
}

task Init -depends InitEnvironment, Clean {

  echo "Creating build directory at the follwing path $buildBase"       
  if (Test-Path $buildBase) {    
    Delete-Directory $buildBase;
  }
  Create-Directory($buildBase);

  echo "Creating test-results directory at the follwing path $testResultsBase"
  if (Test-Path $testResultsBase) {
    Delete-Directory $testResultsBase;
  }
  Create-Directory($testResultsBase);

  echo "Creating deploy directory at the follwing path $deployBase"
  if (Test-Path $deployBase) {
    Delete-Directory $deployBase;
  }
  Create-Directory $deployBase

  $currentDirectory = Resolve-Path .

  echo "Current Directory: $currentDirectory" 
}
  
task Compile -depends Init -description "A build script CompileMain " {
    # Remove Web Project obj directories before compiling
    Remove-Item -Force -Recurse $buildBase\FODT\obj -ErrorAction SilentlyContinue
    foreach ($slnFile in $coreSlns) {
        exec { &$script:msBuild $slnFile /p:Configuration=Release /p:OutDir="$buildBase\" }
    }
}

task Tests -depends Compile {
    #$testAsms = "assembly.dll"
    #foreach ($testAsm in $testAsms) {
    #    exec {&$nunitexec "$buildBase\$testAsm" $script:nunitTargetFramework /xml="$testResultsBase\$testAsm.xml"} 
    #}
}

task Package -depends InitEnvironment {
    if (!(Test-Path $deployBase)) {
      Create-Directory $deployBase;
    }
    foreach ($proj in $packageProjects) {
    exec { &$script:msBuild $proj /m /p:Configuration=Release /target:package /p:MvcBuildViews=false }
    }
    Copy-Item $baseDir\tools\SqlDeploy.exe -Destination $deployBase -Recurse -Force
    Copy-Item $baseDir\database -Destination $deployBase -Recurse -Force    
}

task Build -depends Compile, Tests, Package {

}

task Deploy -depends Build {
  exec {&$baseDir\deploy.cmd}
}

task dev.dbup {
    # Step 1 - Database Upgrade Scripts
    exec {&$baseDir\tools\SqlDeploy.exe -m $baseDir\database -s localhost -d fodt -i -a -f}
}