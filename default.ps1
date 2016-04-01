Framework "4.6.1"

properties {
    $deployURL = "https://friendsofdt-prod.scm.azurewebsites.net:443/MsDeploy.axd"
    $deployUsername = "`$friendsofdt-prod"
    $deployPassword = "SECRET!"
    $dbdeployPassword = "SECRET!"

    $baseDir  = resolve-path .
    $buildDir = "$baseDir\build"
    $artifactsDir = "$baseDir\artifacts"
    $toolsDir = "$baseDir\tools"
    $global:msbuildconfig = "debug"
    $slnFiles = "$baseDir\src\friendsofdt.sln"
    $packageProjects = "$baseDir\src\FODT\FODT.csproj"
}

task default -depends local
task local -depends init, compile, package
task ci -depends clean, release, compile, package

task release {
    $global:msbuildconfig = "release"
}

task clean {
    remove-item "$buildDir" -recurse -force  -ErrorAction SilentlyContinue | out-null
    remove-item "$artifactsDir" -recurse -force  -ErrorAction SilentlyContinue | out-null

    foreach ($slnFile in $slnFiles) {
        exec { msbuild $slnFile /v:m /nologo /p:Configuration=Debug /m /target:Clean }
        exec { msbuild $slnFile /v:m /nologo /p:Configuration=Release /m /target:Clean }
    }
}

task init {
    New-Item $buildDir -ItemType Directory -Force | Out-Null
    New-Item $artifactsDir -ItemType Directory -Force | Out-Null
    $currentDirectory = Resolve-Path .
    echo "Current Directory: $currentDirectory"
}
 
task compile -depends init {
    foreach ($slnFile in $slnFiles) {
        exec { msbuild $slnFile /v:n /nologo /p:Configuration=$msbuildconfig /m /p:AllowedReferenceRelatedFileExtensions=none /p:OutDir="$buildDir\" }
    }
}

task package -depends init {
    foreach ($proj in $packageProjects) {
        exec { msbuild $proj /v:n /nologo /p:Configuration=$msbuildconfig /m /target:package /p:GenerateBuildInfoConfigFile=false /p:MvcBuildViews=false /p:DesktopBuildPackageLocation="$artifactsDir\fodt.zip" /p:_PackageTempDir="c:\temp\package\" }
    }
}

task deploy-website {
    exec { & "$artifactsDir\FODT.deploy.cmd" /Y /M:$deployURL /U:$deployUsername /P:$deployPassword /A:basic }
    write-host -foregroundcolor Magenta "Be sure to check the output of the above command since msdeploy.exe already returns exit code 0!"
}

task redeploy-website -depends Package, deploy-website {
}

task deploy-database {
    exec { & $toolsDir\SqlMigrate.exe -m database -s friendsofdt-prod.database.windows.net -d fodt -u fodtadmin -p $dbdeployPassword -a -f }
}

task dbup {
    exec { & $toolsDir\SqlMigrate.exe -m database -s . -d fodt -i -f }
}