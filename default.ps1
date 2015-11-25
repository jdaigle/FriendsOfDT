Framework "4.5.1x64"

properties {
    $deployURL = "https://friendsofdt-prod.scm.azurewebsites.net:443/MsDeploy.axd"
    $deployUsername = "`$friendsofdt-prod"
    $deployPassword = ""
    $dbdeployPassword = ""
}

$baseDir  = resolve-path .
$buildDir = "$baseDir\build"
$deployDir = "$baseDir\deploy"
$toolsDir = "$baseDir\tools"
$coreSlns = "$baseDir\friendsofdt.sln"
$packageProjects = "$baseDir\FODT\FODT.csproj"

include $toolsDir\psake\buildutils.ps1

task default -depends Build

task Clean {
    if (Test-Path $buildDir) {
        Delete-Directory $buildDir
    }
    if (Test-Path $deployDir) {
        Delete-Directory $deployDir
    }
    foreach ($slnFile in $coreSlns) {
        exec { msbuild $slnFile /v:minimal /nologo /p:Configuration=Debug /m /target:Clean }
        exec { msbuild $slnFile /v:minimal /nologo /p:Configuration=Release /m /target:Clean }
    }
}

task Init -depends Clean {
    echo "Creating build directory at the follwing path $buildDir"
    if (Test-Path $buildDir) {
        Delete-Directory $buildDir;
    }
    Create-Directory($buildDir);

    $currentDirectory = Resolve-Path .

    echo "Current Directory: $currentDirectory"
}
 
task Compile -depends Init {
    foreach ($slnFile in $coreSlns) {
        exec { msbuild $slnFile /v:minimal /nologo /p:Configuration=Release /m /p:AllowedReferenceRelatedFileExtensions=none /p:OutDir="$buildDir\" }
    }
}

task Package -depends Init {
    if (!(Test-Path $deployDir)) {
            Create-Directory $deployDir;
    }
    foreach ($proj in $packageProjects) {
        exec { msbuild $proj /v:minimal /nologo /p:Configuration=Release /m /p:AllowedReferenceRelatedFileExtensions=none /target:package /p:MvcBuildViews=false }
    }
}

task Build -depends Compile, Package {

}

task deploy-web {
    exec { & "$deployDir\FODT.deploy.cmd" /Y /M:$deployURL /U:$deployUsername /P:$deployPassword /A:basic }
    write-host -foregroundcolor Magenta "Be sure to check the output of the above command since msdeploy.exe already returns exit code 0!"
}

task deploy-database {
    exec { & $toolsDir\SqlMigrate.exe -m database -s friendsofdt-prod.database.windows.net -d fodt -u fodtadmin -p $dbdeployPassword -a -f }
}

task dbup {
    exec { & $toolsDir\SqlMigrate.exe -m database -s . -d fodt -i -f }
}