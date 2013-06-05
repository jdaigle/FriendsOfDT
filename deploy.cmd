@echo off
editv64 -b -o -p "Enter username to connect to the server: " deploy_username
editv64 -b -o -m -p "Enter password: " deploy_password

echo Begin Deploy
"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -source:package="deploy\FODT.zip" -dest:auto,computerName="https://cridion.com:8172/msdeploy.axd?site=friendsofdt.org",userName=%deploy_username%,password=%deploy_password%,authtype="basic",includeAcls="False" -verb:sync -disableLink:AppPoolExtension -disableLink:ContentExtension -disableLink:CertificateExtension -setParamFile:"deploy\FODT.SetParameters.xml" -allowuntrusted -enableRule:AppOffline
echo End Deploy