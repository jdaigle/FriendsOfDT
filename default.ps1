Framework "4.6.1"

properties {
    $deployURL = "https://friendsofdt-prod.scm.azurewebsites.net:443/MsDeploy.axd"
    $deploy_username = $null
    $deploy_password = $null
    $dbdeployPassword = $null

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
    if ($deploy_username.Length -eq 0 -or $deploy_password.Length -eq 0) {
        $cred = Get-Credential -Message "Enter Deployment Credentials"
        $deploy_username = $cred.GetNetworkCredential().UserName
        $deploy_password = $cred.GetNetworkCredential().Password
    }
    exec { & "$artifactsDir\FODT.deploy.cmd" /Y /M:$deployURL /U:$deploy_username /P:$deploy_password /A:basic }
    write-host -foregroundcolor Magenta "Be sure to check the output of the above command since msdeploy.exe already returns exit code 0!"
}

task redeploy-website -depends release, package, deploy-website {
}

task deploy-database {
    if ($deploy_username.Length -eq 0) {
        $cred = Get-Credential -Message "Enter Deployment Credentials"
        $dbdeployPassword = $cred.GetNetworkCredential().Password
    }
    exec { & $toolsDir\horton.exe -m database\fodt -s friendsofdt-prod.database.windows.net -u fodtadmin -p $dbdeployPassword -U UPDATE }
}

task dbup {
    exec { & $toolsDir\horton.exe -m database\fodt -s localhost -U UPDATE }
}